using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD layers.
/// </summary>
public class GH_AutocadLayer : GH_AutocadObjectGoo<AutocadLayerTableRecordWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayer"/> class with no value.
    /// </summary>
    public GH_AutocadLayer()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayer"/> class with the
    /// specified AutoCAD layer.
    /// </summary>
    /// <param name="layerWrapper">The AutoCAD layer to wrap.</param>
    public GH_AutocadLayer(AutocadLayerTableRecordWrapper layerWrapper) : base(layerWrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayer"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadLayer(GH_AutocadLayer other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IAutocadLayerTableRecord"/> via the interface.
    /// </summary>
    public GH_AutocadLayer(IAutocadLayerTableRecord autocadLayer)
        : base((autocadLayer as AutocadLayerTableRecordWrapper)!)
    {

    }

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(LayerTableRecord);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new AutocadLayerTableRecordWrapper(unwrapped as LayerTableRecord);

        return new GH_AutocadLayer(newWrapper);
    }
}