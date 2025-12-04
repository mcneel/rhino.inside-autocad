using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD layers.
/// </summary>
public class GH_AutocadLayer : GH_Goo<AutocadLayerWrapper>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => nameof(AutocadLayerWrapper);

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD layer object";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayer"/> class with no value.
    /// </summary>
    public GH_AutocadLayer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayer"/> class with the
    /// specified AutoCAD layer.
    /// </summary>
    /// <param name="layerWrapper">The AutoCAD layer to wrap.</param>
    public GH_AutocadLayer(AutocadLayerWrapper layerWrapper) : base(layerWrapper)
    {
    }

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
    /// Constructs a new <see cref="IAutocadLayer"/> via the interface.
    /// </summary>
    public GH_AutocadLayer(IAutocadLayer autocadLayer)
        : base((autocadLayer as AutocadLayerWrapper)!)
    {

    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return new GH_AutocadLayer(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadLayer goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadLayerWrapper layer)
        {
            this.Value = layer;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadLayerWrapper)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadLayer)))
        {
            target = (Q)(object)new GH_AutocadLayer(this.Value);
            return true;
        }
        return false;
    }
    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadLayer";

        return $"AutocadLayer [Name: {this.Value.Name}, Id: {this.Value.Id} ]";
    }
}

