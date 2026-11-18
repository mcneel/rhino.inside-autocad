using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from a Civil 3D Grading, and
/// optionally updates its name and description.
/// </summary>
[ComponentVersion(introduced: "1.3.10")]
public class CivilGradingComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("1E7B3C5D-8F2A-4961-B4D0-6A9C2E8F1B35");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CivilGradingComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CivilGradingComponent"/> class.
    /// </summary>
    public CivilGradingComponent()
        : base("Civil3d Grading", "CVL-Grd",
            "Extracts information from a Civil 3D Grading. The Civil 3D .NET API exposes only the name, description and style; the grading group, criteria, footprint and target are not available. Curves come from exploding the grading, so they show what the grading style displays.",
            "Civil3d", "Grading")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilGrading(), "Grading",
            "Grd", "A Civil3d Grading", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "N",
            "The name of the grading. When set this will update the name of the grading.", GH_ParamAccess.item);
        pManager[1].Optional = true;

        pManager.AddTextParameter("Description", "Desc",
            "The description of the grading. When set this will update the description of the grading.", GH_ParamAccess.item);
        pManager[2].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the Grading.", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "N",
            "The name of the grading.", GH_ParamAccess.item);

        pManager.AddTextParameter("Description", "Desc",
            "The description of the grading.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_NamedId(GH_ParamAccess.item), "Style",
            "Style", "The style applied to this grading as a NamedId.", GH_ParamAccess.item);

        pManager.AddCurveParameter("Curves", "Crvs",
            "The displayed grading geometry as Rhino curves.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_CivilGrading? gradingGoo = null;

        if (!DA.GetData(0, ref gradingGoo) || gradingGoo is null) return;

        var gradingId = gradingGoo.Reference.ObjectId;

        var document = this.GetDocumentForObjectId(gradingId);
        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
            return;
        }

        string? newName = null;
        string? newDescription = null;

        DA.GetData(1, ref newName);
        DA.GetData(2, ref newDescription);

        var transactionManager = document.CreateTransactionManager();

        ICivilGradingProperties? properties = null;
        var curves = new List<RhinoCurve>();

        try
        {
            transactionManager.PerformTask(() =>
            {
                if (transactionManager.Unwrap()
                        .GetObject(gradingId.Unwrap(), OpenMode.ForRead) is not Grading grading)
                    return false;

                properties = new CivilGradingProperties(grading);

                var updateFlag = (newName != null && newName != properties.Name)
                    || (newDescription != null && newDescription != properties.Description);

                if (updateFlag)
                {
                    properties = properties.Update(transactionManager,
                        newName ?? properties.Name, newDescription ?? properties.Description);
                }

                curves = grading.ToRhinoCurves();

                return true;
            });
        }
        catch (Exception ex)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to update grading: {ex.Message}");
            return;
        }

        if (properties == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to read Grading");
            return;
        }

        if (curves.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                "No curves were produced by exploding the grading. Check that the grading style displays its components.");
        }

        DA.SetData(0, new GH_AutocadObjectId(gradingId));
        DA.SetData(1, properties.Name);
        DA.SetData(2, properties.Description);
        DA.SetData(3, new GH_NamedId(properties.Style));
        DA.SetDataList(4, curves);
    }
}
