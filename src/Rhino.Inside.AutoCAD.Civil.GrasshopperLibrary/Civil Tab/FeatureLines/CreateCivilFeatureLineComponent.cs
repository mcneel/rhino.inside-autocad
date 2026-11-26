using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.Civil.Interop.Constants;
using Rhino.Inside.AutoCAD.Civil.Interop.Naming;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;
using CivilDocument = Autodesk.Civil.ApplicationServices.CivilDocument;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates a standalone Civil 3D Feature Line from a Rhino curve.
/// </summary>
[ComponentVersion(introduced: "1.3.12")]
public class CreateCivilFeatureLineComponent : RhinoInsideAutocad_CreateComponentBase
{
    private string _errorMessage = string.Empty;
    private const string GhPrefix = CivilConstants.GhPrefix;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("2F6C9A1B-D4E7-4B3A-8C5D-6E0F1A2B3C4D");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateCivilFeatureLineComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCivilFeatureLineComponent"/> class.
    /// </summary>
    public CreateCivilFeatureLineComponent()
        : base("Create Civil3d Feature Line", "CVL-CreateFL",
            "Creates a standalone Civil 3D Feature Line from a Rhino curve. Arcs are kept as arcs in plan, and the elevation at each PI is taken from the curve.",
            "Civil3d", "Feature Lines")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddCurveParameter("Curve", "C",
            "The Rhino curve to create the feature line from.", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "N",
            "The name for the Feature Line. If not provided, a unique name will be auto-generated (e.g., Gh_FeatureLine_001).",
            GH_ParamAccess.item);
        pManager[2].Optional = true;

        pManager.AddParameter(new Param_CivilSite(GH_ParamAccess.item), "Site", "Site",
            "The site for the feature line. If not provided, a siteless feature line will be created.", GH_ParamAccess.item);
        pManager[3].Optional = true;

        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.item), "Layer", "L",
            "The layer for the feature line. Uses the current layer if not provided.", GH_ParamAccess.item);
        pManager[4].Optional = true;

        pManager.AddTextParameter("Style", "S",
            "The name of the feature line style to apply. Uses the Civil 3D default if not provided.",
            GH_ParamAccess.item);
        pManager[5].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilFeatureLine(), "Feature Line", "FL",
            "The created Feature Line.", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "N",
            "The name of the created feature line (useful when auto-generated).", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The ObjectId of the created feature line.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        if (ShouldSkipSolve()) return;

        // 1. Read all inputs first
        AutocadDocument? autocadDocument = null;
        RhinoCurve? curve = null;
        var featureLineName = string.Empty;
        GH_CivilSite? siteGoo = null;
        GH_AutocadLayer? layerGoo = null;
        var styleName = string.Empty;

        DA.GetData(0, ref autocadDocument);

        var document = this.GetDocumentOrDefault(autocadDocument);

        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
            return;
        }

        if (!DA.GetData(1, ref curve) || curve is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No curve provided");
            return;
        }

        DA.GetData(2, ref featureLineName);
        DA.GetData(3, ref siteGoo);
        DA.GetData(4, ref layerGoo);
        DA.GetData(5, ref styleName);

        // 2. Build input signature for change detection
        var signature = new InputSignatureBuilder()
            .AddCurve(curve)
            .Add(featureLineName)
            .Add(siteGoo?.Value?.Id)
            .Add(layerGoo?.Value?.Id)
            .Add(styleName)
            .Build();

        // 3. Check for reuse to prevent infinite loops
        if (TryReuseLastCreated(signature))
        {
            var trackedId = GetFirstTrackedObjectId();
            if (trackedId != null)
            {
                var featureLine = RetrieveTrackedObject<FeatureLine>(trackedId, document);
                if (featureLine != null)
                {
                    DA.SetData(0, new GH_CivilFeatureLine(featureLine));
                    DA.SetData(1, featureLine.Name);
                    DA.SetData(2, new GH_AutocadObjectId(trackedId));
                    return;
                }
            }
            // Fall through to create if retrieval failed
        }

        // 4. Delete previous objects now (if replace enabled)
        DeleteTrackedObjectsIfReplaceEnabled();

        _errorMessage = string.Empty;

        var transactionManager = document.CreateTransactionManager();

        var result = transactionManager.PerformTask(() =>
        {
            try
            {
                var database = transactionManager.AutocadDatabase.Unwrap();
                var civilDoc = CivilDocument.GetCivilDocument(database);

                // Generate unique name if not provided
                var finalName = string.IsNullOrWhiteSpace(featureLineName)
                    ? AutoNamer.GenerateUniqueFeatureLineName(transactionManager, GhPrefix)
                    : featureLineName;

                // Get site ObjectId (Null for siteless feature line)
                var siteId = siteGoo?.Value?.Id.Unwrap() ?? ObjectId.Null;

                // Get layer ObjectId (current layer if not provided)
                var layerId = layerGoo?.Value?.Id.Unwrap() ?? database.Clayer;

                // Resolve style by name (Civil 3D default if not provided)
                var styleId = ObjectId.Null;
                if (!string.IsNullOrWhiteSpace(styleName))
                {
                    var styles = civilDoc.Styles.FeatureLineStyles;

                    if (!styles.Contains(styleName))
                    {
                        _errorMessage = $"Feature line style '{styleName}' does not exist in the drawing.";
                        return (null, string.Empty, ObjectId.Null);
                    }

                    styleId = styles[styleName];
                }

                var featureLine = FeatureLineCreator.Create(
                    transactionManager,
                    curve,
                    finalName,
                    siteId,
                    layerId,
                    styleId);

                return ((FeatureLine?)featureLine, finalName, featureLine.Id);
            }
            catch (Autodesk.Civil.CivilException ex)
            {
                _errorMessage = $"Civil 3D error: {ex.Message}";
                return (null, string.Empty, ObjectId.Null);
            }
            catch (System.Exception ex)
            {
                _errorMessage = $"Failed to create feature line: {ex.Message}";
                return (null, string.Empty, ObjectId.Null);
            }
        });

        if (!string.IsNullOrEmpty(_errorMessage))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, _errorMessage);
            return;
        }

        var (createdFeatureLine, createdName, objectId) = result;

        if (createdFeatureLine == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to create feature line");
            return;
        }

        // Track the created object for potential replacement
        this.TrackCreatedObject(objectId, document);

        // Set outputs
        DA.SetData(0, new GH_CivilFeatureLine(createdFeatureLine));
        DA.SetData(1, createdName);
        DA.SetData(2, new GH_AutocadObjectId(new AutocadObjectIdWrapper(objectId)));
    }
}
