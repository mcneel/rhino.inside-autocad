using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that sets the elevations of a Civil 3D Feature Line from a surface.
/// </summary>
/// <remarks>
/// The feature line is modified in place, which can expire upstream components that read it.
/// To avoid a solve loop, the elevations are only assigned again when the inputs differ from
/// the previous solve; otherwise the feature line is just read.
/// </remarks>
[ComponentVersion(introduced: "1.3.12")]
public class CivilFeatureLineElevationsFromSurfaceComponent : RhinoInsideAutocad_ComponentBase
{
    private HashSet<string> _previousSignatures = new();
    private HashSet<string> _currentSignatures = new();

    /// <inheritdoc />
    public override Guid ComponentGuid => new("B8E4D1A7-3C52-4F96-9A0E-6D2F7C1B5E83");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CivilFeatureLineElevationsFromSurfaceComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CivilFeatureLineElevationsFromSurfaceComponent"/> class.
    /// </summary>
    public CivilFeatureLineElevationsFromSurfaceComponent()
        : base("Civil3d Feature Line Elevations From Surface", "CVL-FLSrfElev",
            "Sets the elevations of a Civil 3D Feature Line from a TIN surface, the same as Civil 3D's Elevations From Surface command. The elevations are not linked to the surface.",
            "Civil3d", "Feature Lines")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilFeatureLine(), "Feature Line", "FL",
            "The Civil3d Feature Line to update.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilTinSurface(), "Surface", "Srf",
            "The TIN surface to take elevations from. Must be in the same drawing as the feature line.",
            GH_ParamAccess.item);

        pManager.AddBooleanParameter("Insert Grade Breaks", "GB",
            "True to insert elevation points (grade breaks) where the feature line crosses the surface's triangle edges.",
            GH_ParamAccess.item, true);
        pManager[2].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilFeatureLine(), "Feature Line", "FL",
            "The updated Feature Line.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilFeatureLineProperties(GH_ParamAccess.item), "Properties", "Props",
            "Feature Line properties (use Feature Line Properties component to extract values).", GH_ParamAccess.item);

        pManager.AddCurveParameter("Curve", "Crv",
            "The updated feature line as a Rhino curve.", GH_ParamAccess.item);

        pManager.AddPointParameter("PI Points", "PI",
            "The PI points, with elevations from the surface.", GH_ParamAccess.list);

        pManager.AddPointParameter("Elevation Points", "EP",
            "The elevation points, including any grade breaks inserted at surface crossings.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void BeforeSolveInstance()
    {
        base.BeforeSolveInstance();

        _previousSignatures = _currentSignatures;
        _currentSignatures = new HashSet<string>();
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_CivilFeatureLine? featureLineGoo = null;
        GH_CivilTinSurface? surfaceGoo = null;
        var insertGradeBreaks = true;

        if (!DA.GetData(0, ref featureLineGoo) || featureLineGoo is null) return;
        if (!DA.GetData(1, ref surfaceGoo) || surfaceGoo is null) return;
        DA.GetData(2, ref insertGradeBreaks);

        var featureLineId = featureLineGoo.Reference.ObjectId;
        var surfaceId = surfaceGoo.Reference.ObjectId;

        var document = this.GetDocumentForObjectId(featureLineId);
        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
            return;
        }

        var signatureBuilder = new InputSignatureBuilder()
            .Add(featureLineId)
            .Add(surfaceId)
            .Add(insertGradeBreaks ? 1 : 0);
        surfaceGoo.AppendInputSignature(signatureBuilder);
        var signature = signatureBuilder.Build();

        _currentSignatures.Add(signature);
        var assign = !_previousSignatures.Contains(signature);

        var transactionManager = document.CreateTransactionManager();
        GH_CivilFeatureLine? outputGoo = null;

        FeatureLineGooResult result;

        try
        {
            result = transactionManager.PerformTask(() =>
            {
                var featureLine = assign
                    ? FeatureLineSurfaceElevations.Assign(transactionManager, featureLineId, surfaceId, insertGradeBreaks)
                    : transactionManager.Unwrap().GetObject(featureLineId.Unwrap(), OpenMode.ForRead) as FeatureLine;

                if (featureLine == null)
                    return FeatureLineGooResult.Failed;

                outputGoo = new GH_CivilFeatureLine(featureLine);

                var wrapper = new CivilFeatureLineWrapper(featureLine);

                return new FeatureLineGooResult(
                    new GH_CivilFeatureLineProperties(wrapper.Properties),
                    wrapper.GetCurve(),
                    wrapper.GetPIPoints(),
                    wrapper.GetElevationPoints());
            });
        }
        catch (Autodesk.Civil.CivilException ex)
        {
            _currentSignatures.Remove(signature);
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Civil 3D error: {ex.Message}");
            return;
        }
        catch (Exception ex)
        {
            _currentSignatures.Remove(signature);
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to set elevations from surface: {ex.Message}");
            return;
        }

        if (result.IsSuccess == false)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to read Feature Line");
            return;
        }

        DA.SetData(0, outputGoo);
        DA.SetData(1, result.PropertiesGoo);
        DA.SetData(2, result.Curve);
        DA.SetDataList(3, result.PIPoints);
        DA.SetDataList(4, result.ElevationPoints);
    }
}
