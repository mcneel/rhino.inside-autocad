using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using System.Collections;
using Exception = System.Exception;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that bakes objects to AutoCAD's model space with object
/// tracking. Works like the Create components: it bakes whenever its inputs change,
/// deletes previously baked objects when Replace mode is enabled, and persists the
/// connection to baked objects across sessions.
/// </summary>
[ComponentVersion(introduced: "1.3.0", updated: "1.3.11")]
public class TrackedBakeComponent : RhinoInsideAutocad_CreateComponentBase, IBakingComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("A1847178-E9D4-4556-AC9F-91E50B6CE199");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBakeTrackedComponent;

    /// <inheritdoc />
    public int OutputParamTargetIndex => 0;

    private const int SettingsParamIndex = 2;

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

        pManager.AddParameter(new Param_BakeSettings(GH_ParamAccess.list), "Settings",
            "S", BakeSettingsMatcher.SettingsDescription, GH_ParamAccess.list);
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

        var settingsGoos = new List<GH_BakeSettings?>();
        DA.GetDataList(SettingsParamIndex, settingsGoos);

        var settings = settingsGoos.Select(goo => goo?.Value).ToList();

        if (BakeSettingsMatcher.IsMismatched(objects.Count, settings))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                BakeSettingsMatcher.MismatchMessage(objects.Count, settings));
        }

        var converterFactory = new RhinoConvertibleFactory();
        var bakeableExtractor = new BakeableExtractor(converterFactory);

        // Settings are paired by the object's original index, so an unbakeable object
        // does not shift the settings of the objects after it.
        var bakeables = new List<(IAutocadBakeable Bakeable, IBakeSettings? Settings)>();
        for (var i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            var bakeable = bakeableExtractor.ExtractBakeable(obj);
            if (bakeable != null)
            {
                var objectSettings = BakeSettingsMatcher.ForIndex(objects.Count, settings, i);
                bakeables.Add((bakeable, objectSettings));
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
        var signature = this.BuildInputSignature(bakeables, document);

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
        _pendingAsyncBakeCount = bakeables.Count(pair => pair.Bakeable is GH_AutocadBrepProxy);

        var bakedIds = new List<GH_AutocadObjectId>();

        var transactionManagerWrapper = document.CreateTransactionManager();

        _ = transactionManagerWrapper.PerformTask(() =>
         {
             foreach (var (bakeable, objectSettings) in bakeables)
             {
                 try
                 {
                     var objectIds = bakeable.BakeToAutocad(transactionManagerWrapper, this, objectSettings);

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
    /// Builds a signature from the bakeables, the bake settings matched to each of them
    /// and the target document, used to detect input changes between solves. Each
    /// bakeable fingerprints the same geometry it bakes, so anything affecting the baked
    /// output contributes to the signature.
    /// </summary>
    private string BuildInputSignature(List<(IAutocadBakeable Bakeable, IBakeSettings? Settings)> bakeables,
        IAutocadDocument document)
    {
        var builder = new InputSignatureBuilder();

        foreach (var (bakeable, settings) in bakeables)
        {
            bakeable.AppendInputSignature(builder);

            builder.Add(settings?.Layer?.Id);
            builder.Add(settings?.LineType?.Id);
            builder.AddColor(settings?.Color);

            var linetypeScaleText = settings?.LinetypeScale?.ToString("F6");

            builder.Add(linetypeScaleText);
        }

        builder.Add(document.FileMetadata.FileName);

        return builder.Build();
    }

    /// <inheritdoc />
    public override bool Read(GH_IReader reader)
    {
        if (!base.Read(reader))
            return false;

        // Files written before 1.3.11 store Settings as item access.
        BakeSettingsMatcher.UpgradeSettingsParam(this.Params.Input[SettingsParamIndex]);

        return true;
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
