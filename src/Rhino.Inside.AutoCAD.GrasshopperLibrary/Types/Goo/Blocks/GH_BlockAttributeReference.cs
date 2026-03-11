using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD dynamic block attribute reference.
/// </summary>
public class GH_BlockAttributeReference : GH_Goo<AttributeWrapper>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => "AttributeReference";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD Block Attribute Reference";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_BlockAttributeReference"/> class with no value.
    /// </summary>
    public GH_BlockAttributeReference()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_BlockAttributeReference"/> class with the
    /// specified wrapper.
    /// </summary>
    /// <param name="wrapper">The dynamic block reference property wrapper.</param>
    public GH_BlockAttributeReference(AttributeWrapper wrapper) : base(wrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_BlockAttributeReference"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_BlockAttributeReference(GH_BlockAttributeReference other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new instance via the interface.
    /// </summary>
    public GH_BlockAttributeReference(IAttributeWrapper wrapper)
        : base((wrapper as AttributeWrapper)!)
    { }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_BlockAttributeReference(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_BlockAttributeReference goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AttributeWrapper wrapper)
        {
            this.Value = wrapper;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AttributeWrapper)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_BlockAttributeReference)))
        {
            target = (Q)(object)new GH_BlockAttributeReference(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null BlockAttributeReference";

        return $" BlockAttributeReference [Tag: {this.Value.Tag}, Value: {this.Value.Text}]";
    }
}
