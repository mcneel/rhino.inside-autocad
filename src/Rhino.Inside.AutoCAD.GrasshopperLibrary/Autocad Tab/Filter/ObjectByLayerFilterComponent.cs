using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates a filter for selecting objects on a specific layer.
/// </summary>
[ComponentVersion(introduced: "1.0.17")]
public class ObjectByLayerFilterComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("007dd81b-792d-42b3-9d84-be20d3c9750e");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ObjectByLayerFilterComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectByLayerFilterComponent"/> class.
    /// </summary>
    public ObjectByLayerFilterComponent()
        : base("Autocad Layer Filter", "AC-LayerFilter",
            "Creates a filter that selects objects on a specific layer.",
            "AutoCAD", "Filter")
    { }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddGenericParameter("Layer", "L",
            "The layer name (string) or layer object to filter by.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter",
            "F", "A filter that selects objects on the specified layer.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        object? layerInput = null;
        if (!DA.GetData(0, ref layerInput))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Layer input is required.");
            return;
        }

        ObjectByLayerFilter? filter = null;

        switch (layerInput)
        {
            case GH_AutocadLayer layerGoo when layerGoo.Value != null:
                filter = new ObjectByLayerFilter(layerGoo.Value);
                break;

            case IAutocadLayerTableRecord layer:
                filter = new ObjectByLayerFilter(layer);
                break;

            case GH_String ghString when !string.IsNullOrWhiteSpace(ghString.Value):
                filter = new ObjectByLayerFilter(ghString.Value);
                break;

            case string layerName when !string.IsNullOrWhiteSpace(layerName):
                filter = new ObjectByLayerFilter(layerName);
                break;

            default:
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "Invalid layer input. Provide a layer name (string) or a layer object.");
                return;
        }

        DA.SetData(0, new GH_AutocadFilter(filter));
    }
}
