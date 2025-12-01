using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutoCADEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which represents a <see cref="AutoCADEntity"/>.
/// </summary>
public class Entity : DbObject, IEntity
{
    // private readonly InternalGeometryConverter _geometryConverter = InternalGeometryConverter.Instance!;

    /// <inheritdoc/>
    //public IBoundingBox3d BoundingBox => this.GetBoundingBox3d();

    /// <inheritdoc/>
    public string TypeName { get; }

    /// <inheritdoc/>
    public string LayerName { get; }

    /// <summary>
    /// Constructs a new <see cref="Entity"/> based on an
    /// <see cref="Autodesk.AutoCAD.DatabaseServices.Entity"/>.
    /// </summary>
    public Entity(AutoCADEntity autoCadEntity) : base(autoCadEntity)
    {
        this.LayerName = autoCadEntity.Layer;

        this.TypeName = autoCadEntity.GetRXClass().Name;
    }
    /*
    /// <summary>
    /// Extracts the current <see cref="IBoundingBox3d"/> of the <see cref=
    /// "Autodesk.AutoCAD.DatabaseServices.Entity"/> in case its custom properties were
    /// changed. If the entity's bounding box is empty, returns a new <see cref=
    /// "IBoundingBox3d"/> instance with <see cref="IBoundingBox3d.IsValid"/> set to 
    /// false, indicating an invalid bounding box. 
    /// </summary>
    private IBoundingBox3d GetBoundingBox3d()
    {
        var bounds = _wrappedValue.Bounds;

        return bounds.HasValue
            ? _geometryConverter.Convert(bounds.Value)
            : BoundingBox3d.Unset;
    }*/
}