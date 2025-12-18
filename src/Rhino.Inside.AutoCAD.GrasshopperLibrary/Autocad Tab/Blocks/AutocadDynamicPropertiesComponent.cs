using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts properties from an AutoCAD dynamic block reference property.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class AutocadDynamicPropertiesComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("B8C4D2E1-6F3A-4B9C-8D5E-2A1F9B7C3D6E");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadDynamicPropertiesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadDynamicPropertiesComponent"/> class.
    /// </summary>
    public AutocadDynamicPropertiesComponent()
        : base("Dynamic Block Property", "AC-DynPrp",
            "Gets information from an AutoCAD Dynamic Block Reference Property",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_DynamicBlockReferenceProperty(GH_ParamAccess.item),
            "Property", "P", "A Dynamic Block Reference Property", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "N", "The name of the property", GH_ParamAccess.item);
        pManager.AddGenericParameter("Value", "V", "The value of the property", GH_ParamAccess.item);
        pManager.AddBooleanParameter("IsReadOnly", "RO", "Whether the property is read-only", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        DynamicBlockReferencePropertyWrapper? property = null;

        if (!DA.GetData(0, ref property) || property is null)
            return;

        DA.SetData(0, property.Name);
        DA.SetData(1, property.Value);
        DA.SetData(2, property.IsReadOnly);
    }
}