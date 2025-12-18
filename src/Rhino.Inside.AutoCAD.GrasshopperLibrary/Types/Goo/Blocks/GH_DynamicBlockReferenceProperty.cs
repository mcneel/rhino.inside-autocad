using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD dynamic block reference properties.
/// </summary>
public class GH_DynamicBlockReferenceProperty : GH_Goo<DynamicBlockReferencePropertyWrapper>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => "DynamicBlockReferenceProperty";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD dynamic block reference property";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_DynamicBlockReferenceProperty"/> class with no value.
    /// </summary>
    public GH_DynamicBlockReferenceProperty()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_DynamicBlockReferenceProperty"/> class with the
    /// specified wrapper.
    /// </summary>
    /// <param name="wrapper">The dynamic block reference property wrapper.</param>
    public GH_DynamicBlockReferenceProperty(DynamicBlockReferencePropertyWrapper wrapper) : base(wrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_DynamicBlockReferenceProperty"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_DynamicBlockReferenceProperty(GH_DynamicBlockReferenceProperty other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new instance via the interface.
    /// </summary>
    public GH_DynamicBlockReferenceProperty(IDynamicBlockReferencePropertyWrapper wrapper)
        : base((wrapper as DynamicBlockReferencePropertyWrapper)!)
    { }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_DynamicBlockReferenceProperty(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_DynamicBlockReferenceProperty goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is DynamicBlockReferencePropertyWrapper wrapper)
        {
            this.Value = wrapper;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(DynamicBlockReferencePropertyWrapper)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_DynamicBlockReferenceProperty)))
        {
            target = (Q)(object)new GH_DynamicBlockReferenceProperty(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null DynamicBlockReferenceProperty";

        return $"DynamicBlockReferenceProperty [Name: {this.Value.Name}, Value: {this.Value.Value}]";
    }
}
