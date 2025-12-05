using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutoCADEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which represents a <see cref="AutoCADEntity"/>.
/// </summary>
public class EntityWrapper : DbObjectWrapper, IEntity
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc/>
    public BoundingBox BoundingBox => this.GetBoundingBox3d();

    /// <inheritdoc/>
    public string TypeName { get; }

    /// <inheritdoc/>
    public string LayerName { get; }

    /// <summary>
    /// Constructs a new <see cref="EntityWrapper"/> based on an
    /// <see cref="Autodesk.AutoCAD.DatabaseServices.Entity"/>.
    /// </summary>
    public EntityWrapper(AutoCADEntity autoCadEntity) : base(autoCadEntity)
    {
        this.LayerName = autoCadEntity.Layer;

        this.TypeName = autoCadEntity.GetRXClass().Name;
    }

    /// <summary>
    /// Extracts the current <see cref="BoundingBox"/> of the <see cref=
    /// "Autodesk.AutoCAD.DatabaseServices.Entity"/> in case its custom properties were
    /// changed. If the entity's bounding box is empty, returns a new <see cref=
    /// "BoundingBox"/> instance with <see cref="IBoundBoundingBoxingBox3d.IsValid"/> set to 
    /// false, indicating an invalid bounding box. 
    /// </summary>
    private BoundingBox GetBoundingBox3d()
    {
        var bounds = _wrappedValue.Bounds;

        return bounds.HasValue
            ? _geometryConverter.ToRhinoType(bounds.Value)
            : BoundingBox.Unset;
    }
}