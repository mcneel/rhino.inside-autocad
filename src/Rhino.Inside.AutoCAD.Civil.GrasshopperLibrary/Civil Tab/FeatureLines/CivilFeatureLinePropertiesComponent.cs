using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts individual values from Civil 3D Feature Line Properties,
/// and optionally updates the feature line.
/// </summary>
[ComponentVersion(introduced: "1.3.12")]
public class CivilFeatureLinePropertiesComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("E5B3C8D2-91A4-4C7F-B6E0-3D8A2F1C9B47");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CivilFeatureLinePropertiesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CivilFeatureLinePropertiesComponent"/> class.
    /// </summary>
    public CivilFeatureLinePropertiesComponent()
        : base("Civil3d Feature Line Properties", "CVL-FLProps",
            "Extracts individual values from Civil 3D Feature Line Properties",
            "Civil3d", "Feature Lines")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilFeatureLineProperties(GH_ParamAccess.item), "Properties",
            "Props", "Feature Line properties from a Civil3d Feature Line", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "N",
            "The name of the feature line. When set this will update the name of the feature line.", GH_ParamAccess.item);
        pManager[1].Optional = true;

        pManager.AddTextParameter("Description", "Desc",
            "The description of the feature line. When set this will update the description of the feature line.", GH_ParamAccess.item);
        pManager[2].Optional = true;

        pManager.AddTextParameter("Style", "Style",
            "The name of a feature line style. When set this will update the style of the feature line.", GH_ParamAccess.item);
        pManager[3].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "N",
            "The name of the feature line.", GH_ParamAccess.item);

        pManager.AddTextParameter("Description", "Desc",
            "The description of the feature line.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_NamedId(GH_ParamAccess.item), "Style",
            "Style", "The style applied to this feature line as a NamedId.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_NamedId(GH_ParamAccess.item), "Site",
            "Site", "The site that owns this feature line as a NamedId. Empty for siteless feature lines.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Length 2D", "L2D",
            "The 2D (plan) length of the feature line, in drawing units.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Length 3D", "L3D",
            "The 3D length of the feature line, in drawing units.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Min Elevation", "MinZ",
            "The minimum elevation of the feature line, in drawing units.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Max Elevation", "MaxZ",
            "The maximum elevation of the feature line, in drawing units.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Min Grade", "MinG",
            "The minimum grade of the feature line.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Max Grade", "MaxG",
            "The maximum grade of the feature line.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("PI Count", "PIC",
            "The number of PI points.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("Elevation Point Count", "EPC",
            "The number of elevation points.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("Curve Count", "CC",
            "The number of arc segments.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_CivilFeatureLineProperties? propsGoo = null;

        if (!DA.GetData(0, ref propsGoo) || propsGoo?.Value is null) return;

        ICivilFeatureLineProperties props = propsGoo.Value;

        var newName = props.Name;
        var newDescription = props.Description;
        var newStyle = props.Style.Name;

        var updateFlag = false;

        if (DA.GetData(1, ref newName) && newName != props.Name) updateFlag = true;
        if (DA.GetData(2, ref newDescription) && newDescription != props.Description) updateFlag = true;
        if (DA.GetData(3, ref newStyle) && newStyle != props.Style.Name) updateFlag = true;

        if (updateFlag)
        {
            var document = this.GetDocumentForObjectId(props.FeatureLineId);
            if (document is null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
                return;
            }

            var transactionManager = document.CreateTransactionManager();

            try
            {
                props = transactionManager.PerformTask(() =>
                    props.Update(transactionManager, newName, newDescription, newStyle));
            }
            catch (Exception ex)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to update feature line: {ex.Message}");
                return;
            }
        }

        DA.SetData(0, props.Name);
        DA.SetData(1, props.Description);
        DA.SetData(2, new GH_NamedId(props.Style));
        DA.SetData(3, new GH_NamedId(props.Site));
        DA.SetData(4, props.Length2D);
        DA.SetData(5, props.Length3D);
        DA.SetData(6, props.MinElevation);
        DA.SetData(7, props.MaxElevation);
        DA.SetData(8, props.MinGrade);
        DA.SetData(9, props.MaxGrade);
        DA.SetData(10, props.PIPointCount);
        DA.SetData(11, props.ElevationPointCount);
        DA.SetData(12, props.CurveCount);
    }
}
