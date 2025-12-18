using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts properties from an AutoCAD dynamic block reference property.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class SetAutocadDynamicPropertiesComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("b28d185e-fe02-4330-9159-59e6faa508d1");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SetAutocadDynamicPropertiesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetAutocadDynamicPropertiesComponent"/> class.
    /// </summary>
    public SetAutocadDynamicPropertiesComponent()
        : base("Set Dynamic Block Properties", "AC-SetPrp",
            "Gets information from an AutoCAD Dynamic Block Reference Property",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_DynamicBlockReferenceProperty(GH_ParamAccess.item),
            "Property", "P", "A Dynamic Block Reference Property", GH_ParamAccess.item);

        pManager.AddGenericParameter("Value", "Value", "The value of the property",
            GH_ParamAccess.item);
        pManager[1].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_DynamicBlockReferenceProperty(GH_ParamAccess.item),
            "Property", "P", "A Dynamic Block Reference Property", GH_ParamAccess.item);

        pManager.AddGenericParameter("Value", "V", "The value of the property", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        DynamicBlockReferencePropertyWrapper? property = null;

        if (!DA.GetData(0, ref property) || property is null)
            return;

        var value = property.Value;
        DA.GetData(2, ref value);

        _ = property.SetValue(value);

        var goo = new GH_DynamicBlockReferenceProperty(property);

        DA.SetData(0, goo);
        DA.SetData(2, property.Value);
    }
}