using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using AutocadText = Autodesk.AutoCAD.DatabaseServices.MText;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that converts an AutoCAD text to a Rhino TextEntity
/// </summary>
public class ConvertFromAutoCadTextComponent : GH_Component
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("17410dc2-c448-4ffe-98d7-7faea039b4da");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertFromAutoCadTextComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertFromAutoCadTextComponent"/> class.
    /// </summary>
    public ConvertFromAutoCadTextComponent()
        : base("From AutoCAD Text", "FrTxt",
            "Converts an AutoCAD Text to a Rhino TextEntity",
            "AutoCAD", "Convert")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadText(), "Text", "T", "AutoCAD text", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGeometryParameter("Text", "T", "A Rhino TextEntity", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadText? autocadText = null;

        if (!DA.GetData(0, ref autocadText)
            || autocadText is null) return;

        var rhinoText = _geometryConverter.ToRhinoType(autocadText);

        if (rhinoText == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                "Failed to convert text to Rhino format");
            return;
        }

        DA.SetData(0, rhinoText);
    }
}
