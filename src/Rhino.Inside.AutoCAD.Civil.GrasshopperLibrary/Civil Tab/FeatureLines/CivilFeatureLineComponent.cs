using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from a standalone Civil 3D Feature Line.
/// </summary>
[ComponentVersion(introduced: "1.3.12")]
public class CivilFeatureLineComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("7D2A9E41-0B6C-4F18-A3E5-2C9B8D7F6E01");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CivilFeatureLineComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CivilFeatureLineComponent"/> class.
    /// </summary>
    public CivilFeatureLineComponent()
        : base("Civil3d Feature Line", "CVL-FL",
            "Extracts information from a standalone Civil 3D Feature Line",
            "Civil3d", "Feature Lines")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilFeatureLine(), "Feature Line",
            "FL", "A Civil3d Feature Line", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the Feature Line.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilFeatureLineProperties(GH_ParamAccess.item), "Properties", "Props",
            "Feature Line properties (use Feature Line Properties component to extract values).", GH_ParamAccess.item);

        pManager.AddCurveParameter("Curve", "Crv",
            "The feature line as a Rhino curve, with arcs preserved in plan.", GH_ParamAccess.item);

        pManager.AddPointParameter("PI Points", "PI",
            "The PI points, which define the horizontal geometry.", GH_ParamAccess.list);

        pManager.AddPointParameter("Elevation Points", "EP",
            "The elevation points, which add grade breaks between PI points.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_CivilFeatureLine? featureLineGoo = null;

        if (!DA.GetData(0, ref featureLineGoo) || featureLineGoo is null) return;

        var featureLineId = featureLineGoo.Reference.ObjectId;

        var document = this.GetDocumentForObjectId(featureLineId);
        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
            return;
        }

        var transactionManager = document.CreateTransactionManager();

        var result = transactionManager.PerformTask(() =>
        {
            var featureLine = transactionManager.Unwrap()
                .GetObject(featureLineId.Unwrap(), OpenMode.ForRead) as FeatureLine;

            if (featureLine == null)
            {
                return FeatureLineGooResult.Failed;
            }

            var wrapper = new CivilFeatureLineWrapper(featureLine);

            return new FeatureLineGooResult(
                new GH_CivilFeatureLineProperties(wrapper.Properties),
                wrapper.GetCurve(),
                wrapper.GetPIPoints(),
                wrapper.GetElevationPoints());
        });

        if (result.IsSuccess == false)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to read Feature Line");
            return;
        }

        if (result.Curve == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Feature Line has no usable geometry");
        }

        DA.SetData(0, new GH_AutocadObjectId(featureLineId));
        DA.SetData(1, result.PropertiesGoo);
        DA.SetData(2, result.Curve);
        DA.SetDataList(3, result.PIPoints);
        DA.SetDataList(4, result.ElevationPoints);
    }
}
