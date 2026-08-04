using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using System.Collections;
using Exception = System.Exception;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoBox = Rhino.Geometry.Box;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoGeometryBase = Rhino.Geometry.GeometryBase;
using RhinoLine = Rhino.Geometry.Line;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoRectangle3d = Rhino.Geometry.Rectangle3d;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that bakes objects to AutoCAD's model space with object
/// tracking. Works like the Create components: it bakes whenever its inputs change,
/// deletes previously baked objects when Replace mode is enabled, and persists the
/// connection to baked objects across sessions.
/// </summary>
[ComponentVersion(introduced: "1.3.0", updated: "1.3.2")]
public class TrackedBakeComponent : RhinoInsideAutocad_CreateComponentBase, IBakingComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("A1847178-E9D4-4556-AC9F-91E50B6CE199");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBakeTrackedComponent;

    /// <inheritdoc />
    public int OutputParamTargetIndex => 0;

    // Number of asynchronous brep conversions still in flight from the last bake.
    // Used to suppress re-bakes while ids have not yet arrived via AppendDataList
    // (all-brep bakes have no tracked ids yet, so TryReuseLastCreated alone cannot
    // prevent duplicate conversion enqueues from event-driven re-solves).
    private int _pendingAsyncBakeCount;
    private string? _lastBakedSignature;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackedBakeComponent"/> class.
    /// </summary>
    public TrackedBakeComponent()
        : base("Tracked Bake to AutoCAD", "AC-TBake",
            "Bakes objects to AutoCAD's model space and tracks them, replacing previously baked objects when the inputs change",
            "AutoCAD", "Baking")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "The AutoCAD document to bake to. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddGenericParameter("Objects", "O",
            "The objects to bake to AutoCAD (curves, points, meshes, solids)",
            GH_ParamAccess.list);

        pManager.AddParameter(new Param_BakeSettings(GH_ParamAccess.item), "Settings",
            "S", "Optional bake settings (layer, linetype, color)", GH_ParamAccess.item);
        pManager[2].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.list), "ObjectIds",
            "Ids", "The ObjectIds of the baked objects", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        // Skip solve if undo/redo deferral is active (see base class documentation)
        if (this.ShouldSkipSolve())
            return;

        AutocadDocument? autocadDocument = null;
        DA.GetData(0, ref autocadDocument);

        var document = this.GetDocumentOrDefault(autocadDocument);

        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
            return;
        }

        var objects = new List<object>();
        if (!DA.GetDataList(1, objects) || objects.Count == 0)
            return;

        GH_BakeSettings? settingsGoo = null;
        DA.GetData(2, ref settingsGoo);

        var settings = settingsGoo?.Value;

        var converterFactory = new RhinoConvertibleFactory();
        var bakeableExtractor = new BakeableExtractor(converterFactory);

        var bakeables = new List<IAutocadBakeable>();
        foreach (var obj in objects)
        {
            var bakeable = bakeableExtractor.ExtractBakeable(obj);
            if (bakeable != null)
            {
                bakeables.Add(bakeable);
            }
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    $"Object of type {obj?.GetType().Name ?? "null"} is not bakeable");
            }
        }

        if (bakeables.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No bakeable objects provided");
            return;
        }

        // Build input signature for change detection
        var signature = this.BuildInputSignature(objects, settings, document);

        // Check for reuse to prevent infinite loops (bake -> AutoCAD change events ->
        // downstream Get components expire -> this component expires again)
        if (this.TryReuseLastCreated(signature))
        {
            var trackedIds = this.GetTrackedObjectIds();
            DA.SetDataList(0, trackedIds.Select(id => new GH_AutocadObjectId(id)));
            return;
        }

        // Asynchronous brep conversions from the last bake may still be in flight;
        // don't enqueue duplicates while waiting for their ids to arrive.
        if (_pendingAsyncBakeCount > 0 && signature == _lastBakedSignature)
            return;

        // Delete previous objects now (if replace enabled)
        this.DeleteTrackedObjectsIfReplaceEnabled();

        _lastBakedSignature = signature;
        _pendingAsyncBakeCount = bakeables.Count(bakeable => bakeable is GH_AutocadBrepProxy);

        var bakedIds = new List<GH_AutocadObjectId>();

        var transactionManagerWrapper = document.CreateTransactionManager();

        _ = transactionManagerWrapper.PerformTask(() =>
         {
             foreach (var bakeable in bakeables)
             {
                 try
                 {
                     var objectIds = bakeable.BakeToAutocad(transactionManagerWrapper, this, settings);

                     foreach (var objectId in objectIds)
                     {
                         // Track created object for replace-on-rebake functionality
                         this.TrackCreatedObject(objectId.Unwrap(), document);

                         bakedIds.Add(new GH_AutocadObjectId(objectId));
                     }
                 }
                 catch (Exception ex)
                 {
                     this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                         $"Failed to bake object: {ex.Message}");
                 }
             }

             return true;
         });

        DA.SetDataList(0, bakedIds);
    }

    /// <summary>
    /// Builds a signature from the input objects, bake settings and target document,
    /// used to detect input changes between solves. The geometry normalization mirrors
    /// <see cref="BakeableExtractor.ExtractBakeable"/> so that anything affecting the
    /// baked output contributes to the signature.
    /// </summary>
    private string BuildInputSignature(List<object> objects, IBakeSettings? settings, IAutocadDocument document)
    {
        var builder = new InputSignatureBuilder();

        foreach (var obj in objects)
        {
            var value = obj;

            if (obj is IGH_Goo goo)
            {
                var valueProperty = goo.GetType().GetProperty("Value");
                value = valueProperty?.GetValue(goo) ?? obj;
            }

            switch (value)
            {
                case RhinoLine line:
                    builder.AddCurve(new RhinoLineCurve(line));
                    break;
                case RhinoArc arc:
                    builder.AddCurve(new RhinoArcCurve(arc));
                    break;
                case RhinoCircle circle:
                    builder.AddCurve(new RhinoArcCurve(circle));
                    break;
                case RhinoRectangle3d rectangle:
                    builder.AddCurve(rectangle.ToNurbsCurve());
                    break;
                case RhinoPoint3d point:
                    builder.AddPoint(point);
                    break;
                case RhinoBox box:
                    builder.AddGeometry(box.ToBrep());
                    break;
                case RhinoGeometryBase geometry:
                    builder.AddGeometry(geometry);
                    break;
                default:
                    builder.Add(value?.ToString());
                    break;
            }
        }

        builder.Add(settings?.Layer?.Id);
        builder.Add(settings?.LineType?.Id);
        builder.AddColor(settings?.Color);

        var linetypeScaleText = settings?.LinetypeScale?.ToString("F6");

        builder.Add(linetypeScaleText);
        builder.Add(document.FileMetadata.FileName);

        return builder.Build();
    }

    /// <inheritdoc />
    public void AddWarningMessage(string message)
    {
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, message);
    }

    /// <inheritdoc />
    public bool AppendDataList(IEnumerable list)
    {
        // Materialize once: the incoming list is typically a deferred LINQ projection
        var items = list.Cast<object>().ToList();

        // Track the asynchronously baked ids (brep conversions) as they arrive
        foreach (var item in items)
        {
            if (item is GH_AutocadObjectId { Value: not null } objectIdGoo)
            {
                // TrackCreatedObject resolves the owning document per-handle on deletion,
                // so the active-document fallback is sufficient here.
                this.TrackCreatedObject(objectIdGoo.Value.Unwrap(), this.GetActiveDocumentFallback()!);
            }
        }

        if (_pendingAsyncBakeCount > 0)
        {
            _pendingAsyncBakeCount--;
        }

        var ghParam = this.Params.Output[this.OutputParamTargetIndex];

        var path = new GH_Path(0);

        if (ghParam.VolatileData.PathCount > 0)
        {
            var lastPath = ghParam.VolatileData.Paths[ghParam.VolatileData.PathCount - 1];

            var indices = lastPath.Indices;

            indices[indices.Length - 1]++;

            path = new GH_Path(indices);
        }

        var result = ghParam.AddVolatileDataList(path, items);

        if (result)
        {
            ghParam.ExpireSolution(false);

            this.OnPingDocument()?.NewSolution(false);
        }

        return result;
    }
}
