using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD ObjectIds.
/// </summary>
public class GH_AutocadObject : GH_AutocadObjectGoo<AutocadDbObjectWrapper>
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
    /// <param name="autocadDbObject">The AutoCAD Object to wrap.</param>
    protected GH_AutocadObject(AutocadDbObjectWrapper autocadDbObject) : base(autocadDbObject)
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

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(DBObject);

    /// <summary>
    /// Constructs a new <see cref="IDbObject"/> via the interface.
    /// </summary>
    public GH_AutocadObject(IDbObject dbObject)
        : base(dbObject as AutocadDbObjectWrapper)
    { }

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        return new GH_AutocadObject(dbObject);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return $"Null {this.TypeName}";

        return $"{this.TypeName} [Id: {this.Reference}, Type: {this.Value.AutocadObject?.GetType().Name ?? "Unknown"} ]";
    }
}