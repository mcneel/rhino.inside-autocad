using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD ObjectIds.
/// </summary>
public class GH_AutocadObject : GH_Goo<DbObjectWrapper>, IGH_AutocadReference
{

    /// <inheritdoc />
    public IObjectId Id => this.Value.Id;

    /// <inheritdoc />
    public override bool IsValid => this.Value != null && this.Value.IsValid;

    /// <inheritdoc />
    public override string TypeName => nameof(DbObjectWrapper);

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD Object";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with no value.
    /// </summary>
    public GH_AutocadObject()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    public GH_AutocadObject(DbObjectWrapper dbObject) : base(dbObject)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadObject(GH_AutocadObject other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IDbObject"/> via the interface.
    /// </summary>
    public GH_AutocadObject(IDbObject dbObject)
        : base((dbObject as DbObjectWrapper)!)
    {

    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return new GH_AutocadObject(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadObject goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is DbObjectWrapper autocadObject)
        {
            this.Value = autocadObject;
            return true;
        }

        if (source is GH_AutocadLayer layerGoo)
        {
            this.Value = new DbObjectWrapper(layerGoo.Value.Unwrap());
            return true;

        }

        if (source is AutocadLayerWrapper layer)
        {
            this.Value = new DbObjectWrapper(layer.Unwrap());
            return true;
        }

        if (source is GH_AutocadLayout layoutGoo)
        {
            this.Value = new DbObjectWrapper(layoutGoo.Value.Unwrap());
            return true;

        }

        if (source is AutocadLayoutWrapper layout)
        {
            this.Value = new DbObjectWrapper(layout.Unwrap());
            return true;
        }

        if (source is GH_AutocadLinePattern linePatternGoo)
        {
            this.Value = new DbObjectWrapper(linePatternGoo.Value.Unwrap());
            return true;

        }

        if (source is AutocadLinePattern linePattern)
        {
            this.Value = new DbObjectWrapper(linePattern.Unwrap());
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(DbObjectWrapper)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadObject)))
        {
            target = (Q)(object)new GH_AutocadObject(this.Value);
            return true;
        }
        return false;
    }
    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null Autocad Object";

        return $"AutocadObject [Type: {this.Value.GetType().Name.ToString()}, Id: {this.Value.Id.ToString()} ]";
    }
}

