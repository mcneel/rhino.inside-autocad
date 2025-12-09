using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD ObjectIds.
/// </summary>
public abstract class GH_AutocadObjectGoo<TWrapperType> : GH_Goo<TWrapperType>, IGH_AutocadReferenceObject
where TWrapperType : IDbObject
{

    /// <inheritdoc />
    public IObjectId AutocadReferenceId => this.Value.Id;

    /// <inheritdoc />
    public IDbObject ObjectValue => this.Value;



    /// <inheritdoc />
    public override bool IsValid => this.Value != null && this.Value.IsValid;

    /// <inheritdoc />
    public override string TypeName => typeof(TWrapperType).Name;

    /// <inheritdoc />
    public override string TypeDescription => $"Represents an AutoCAD {this.TypeName}";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with no value.
    /// </summary>
    protected GH_AutocadObjectGoo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    protected GH_AutocadObjectGoo(TWrapperType dbObject) : base(dbObject)
    {
    }

    /// <summary>
    /// News up a new <see cref="IGH_Goo"/> instance wrapping the specified
    /// <see cref="IDbObject"/>
    /// </summary>
    protected abstract IGH_Goo CreateInstance(IDbObject dbObject);

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return this.CreateInstance(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        var converter = new GooConverter();

        if (converter.TryConvertFromGoo(source, out GH_AutocadObjectGoo<TWrapperType> target))
        {
            this.Value = target.Value;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(TWrapperType)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadObjectGoo<TWrapperType>)))
        {
            target = (Q)(object)this.CreateInstance(this.Value);
            return true;
        }
        return false;
    }
    /// <inheritdoc />
    public void GetLatestObject()
    {
        var picker = new AutocadObjectPicker();
        if (picker.TryGetUpdatedObject(this.AutocadReferenceId, out var entity))
        {
            this.Value = (TWrapperType?)entity;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return $"Null Autocad {this.TypeName}";

        return $"Autocad {this.TypeName} [Id: {this.Value.Id.ToString()} ]";
    }
}