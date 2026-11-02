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
    public GH_AutocadObject()
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
    /// <remarks>
    /// Geometry Goo does not hold the database object. Its value is a clone, or a Rhino
    /// Brep for a solid, so it has no ObjectId or extension dictionary of its own and only
    /// its reference identifies the object in the drawing. A referenced Goo is therefore
    /// resolved through that reference. Geometry with no reference, or whose referenced
    /// object can no longer be opened, falls back to the base conversion.
    /// </remarks>
    public override bool CastFrom(object source)
    {
        if (source is IGH_GeometricGoo and IGH_AutocadReference { Reference.IsValid: true } referenceGoo)
        {
            var picker = new AutocadObjectPicker();

            if (picker.TryGetUpdatedObject(referenceGoo.Reference.ObjectId, out var entity)
                && entity is AutocadDbObjectWrapper databaseObject)
            {
                this.Value = databaseObject;
                return true;
            }
        }

        return base.CastFrom(source);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return $"Null {this.TypeName}";

        return $"{this.TypeName} [Id: {this.Reference}, Type: {this.Value.AutocadObject?.GetType().Name ?? "Unknown"} ]";
    }
}