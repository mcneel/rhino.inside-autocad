using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD ObjectIds.
/// </summary>
public class GH_AutocadObject : GH_AutocadObjectGoo<DbObjectWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with no value.
    /// </summary>
    protected GH_AutocadObject()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    protected GH_AutocadObject(DbObjectWrapper dbObject) : base(dbObject)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    protected GH_AutocadObject(GH_AutocadObject other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IDbObject"/> via the interface.
    /// </summary>
    public GH_AutocadObject(IDbObject dbObject)
        : base((dbObject as AutocadLinetypeTableRecord)!)
    { }

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        return new GH_AutocadObject(dbObject);
    }
}