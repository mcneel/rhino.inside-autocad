using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD Scale3d
/// </summary>
public class GH_Scale3d : GH_Goo<AutocadScale>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => "Scale3d";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD Scale3d";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_Scale3d"/> class with no value.
    /// </summary>
    public GH_Scale3d()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_Scale3d"/> class with the
    /// specified wrapper.
    /// </summary>
    /// <param name="wrapper">The dynamic block reference property wrapper.</param>
    public GH_Scale3d(AutocadScale wrapper) : base(wrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_Scale3d"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_Scale3d(GH_Scale3d other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new instance via the interface.
    /// </summary>
    public GH_Scale3d(IAttributeWrapper wrapper)
        : base((wrapper as AutocadScale)!)
    { }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_Scale3d(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_Scale3d goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadScale wrapper)
        {
            this.Value = wrapper;
            return true;
        }

        if (source is GH_Number number)
        {
            this.Value = new AutocadScale(number.Value);
            return true;
        }

        if (source is List<GH_Number> { Count: >= 3 } numbers)
        {
            this.Value = new AutocadScale(numbers[0].Value, numbers[1].Value, numbers[2].Value);
            return true;
        }

        if (source is double numberDouble)
        {
            this.Value = new AutocadScale(numberDouble);
            return true;
        }

        if (source is List<double> { Count: >= 3 } doubles)
        {
            this.Value = new AutocadScale(doubles[0], doubles[1], doubles[2]);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadScale)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_Scale3d)))
        {
            target = (Q)(object)new GH_Scale3d(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "No Scale";

        return $" Scale3d [X: {this.Value.X}, Y: {this.Value.Y}, Z: {this.Value.Z}]";
    }
}
