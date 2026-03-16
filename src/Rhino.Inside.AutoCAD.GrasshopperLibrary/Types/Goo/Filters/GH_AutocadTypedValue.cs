using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD TypedValue wrappers.
/// </summary>
public class GH_AutocadTypedValue : GH_Goo<ITypedValueWrapper>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => "TypedValue";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an Filter Rule (AutoCAD TypedValue)";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadTypedValue"/> class with no value.
    /// </summary>
    public GH_AutocadTypedValue()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadTypedValue"/> class with the
    /// specified typed value wrapper.
    /// </summary>
    /// <param name="typedValue">The typed value wrapper to wrap.</param>
    public GH_AutocadTypedValue(ITypedValueWrapper typedValue) : base(typedValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadTypedValue"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadTypedValue(GH_AutocadTypedValue other)
    {
        this.Value = other.Value;
    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_AutocadTypedValue(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadTypedValue goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is ITypedValueWrapper typedValue)
        {
            this.Value = typedValue;
            return true;
        }

        if (source is TypedValueWrapper wrapper)
        {
            this.Value = wrapper;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(ITypedValueWrapper)))
        {
            target = (Q)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadTypedValue)))
        {
            target = (Q)(object)new GH_AutocadTypedValue(this.Value);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null Filter Rule";

        return $"FilterRules (Autocad TypedValues) [TypeCode: {this.Value.TypeCode}, Value: {this.Value.Value}]";
    }
}
