using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for Civil 3D Feature Line properties.
/// </summary>
/// <remarks>
/// This Goo wraps a <see cref="CivilFeatureLineProperties"/> containing
/// properties from a Feature Line. The wrapped value is immutable.
/// </remarks>
public class GH_CivilFeatureLineProperties : GH_Goo<CivilFeatureLineProperties>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilFeatureLineProperties"/> class with no value.
    /// </summary>
    public GH_CivilFeatureLineProperties()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilFeatureLineProperties"/> class with the
    /// specified feature line properties wrapper.
    /// </summary>
    /// <param name="featureLineProperties">The Civil 3D feature line properties wrapper.</param>
    public GH_CivilFeatureLineProperties(CivilFeatureLineProperties featureLineProperties) : base(featureLineProperties)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilFeatureLineProperties"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_CivilFeatureLineProperties(GH_CivilFeatureLineProperties other) : base(other.Value)
    {
    }

    /// <summary>
    /// Constructs a new <see cref="GH_CivilFeatureLineProperties"/> via the interface.
    /// </summary>
    public GH_CivilFeatureLineProperties(ICivilFeatureLineProperties featureLineProperties)
        : base((featureLineProperties as CivilFeatureLineProperties)!)
    {
    }

    /// <inheritdoc />
    public override bool IsValid => Value != null;

    /// <inheritdoc />
    public override string IsValidWhyNot
    {
        get
        {
            if (Value == null)
                return "No feature line properties data";
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public override string TypeName => "Civil3d Feature Line Properties";

    /// <inheritdoc />
    public override string TypeDescription => "Properties from a Civil 3D Feature Line";

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_CivilFeatureLineProperties(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_CivilFeatureLineProperties goo)
        {
            Value = goo.Value;
            return true;
        }

        if (source is CivilFeatureLineProperties wrapper)
        {
            Value = wrapper;
            return true;
        }

        if (source is ICivilFeatureLineProperties props)
        {
            Value = props as CivilFeatureLineProperties;
            return Value != null;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(CivilFeatureLineProperties)))
        {
            target = (Q)(object)Value!;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_CivilFeatureLineProperties)))
        {
            target = (Q)(object)new GH_CivilFeatureLineProperties(this);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (Value == null)
            return "Null Civil3d Feature Line Properties";

        return Value.ToString();
    }
}
