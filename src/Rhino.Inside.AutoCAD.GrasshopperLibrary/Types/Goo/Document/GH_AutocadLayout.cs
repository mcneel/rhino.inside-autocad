using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD layouts.
/// </summary>
public class GH_AutocadLayout : GH_Goo<AutocadLayoutWrapper>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => nameof(AutocadLayoutWrapper);

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD layout object";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayout"/> class with no value.
    /// </summary>
    public GH_AutocadLayout()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayout"/> class with the
    /// specified AutoCAD layout.
    /// </summary>
    /// <param name="layoutWrapper">The AutoCAD layout to wrap.</param>
    public GH_AutocadLayout(AutocadLayoutWrapper layoutWrapper) : base(layoutWrapper)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayout"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadLayout(GH_AutocadLayout other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IAutocadLayout"/> via the interface.
    /// </summary>
    public GH_AutocadLayout(IAutocadLayout autocadLayout)
        : base((autocadLayout as AutocadLayoutWrapper)!)
    {

    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return new GH_AutocadLayout(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadLayout goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadLayoutWrapper layout)
        {
            this.Value = layout;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadLayoutWrapper)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadLayout)))
        {
            target = (Q)(object)new GH_AutocadLayout(this.Value);
            return true;
        }
        return false;
    }
    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadLayout";

        return $"AutocadLayout [Name: {this.Value.Name}, Id: {this.Value.Id.Value} ]";
    }
}

