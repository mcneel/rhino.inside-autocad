using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD ObjectIds.
/// </summary>
public class GH_AutocadObjectId : GH_Goo<AutocadObjectId>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value is { IsValid: true };

    /// <inheritdoc />
    public override string TypeName => nameof(AutocadObjectId);

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD ObjectId";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObjectId"/> class with no value.
    /// </summary>
    public GH_AutocadObjectId()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObjectId"/> class with the
    /// specified AutoCAD ObjectId.
    /// </summary>
    /// <param name="objectId">The AutoCAD ObjectId to wrap.</param>
    public GH_AutocadObjectId(AutocadObjectId objectId) : base(objectId)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObjectId"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadObjectId(GH_AutocadObjectId other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IObjectId"/> via the interface.
    /// </summary>
    public GH_AutocadObjectId(IObjectId objectId)
        : base((objectId as AutocadObjectId)!)
    {

    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return new GH_AutocadObjectId(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadObjectId goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadObjectId objectId)
        {
            this.Value = objectId;
            return true;
        }

        var converter = new GooConverter();

        if (converter.TryConvertGetId(source, out var target))
        {
            this.Value = new AutocadObjectId(target!.Unwrap());
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadObjectId)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadObjectId)))
        {
            target = (Q)(object)new GH_AutocadObjectId(this.Value);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null Autocad Object";

        return $"Autocad ObjectId [Id: {this.Value.ToString()} ]";
    }
}