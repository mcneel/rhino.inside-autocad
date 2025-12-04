using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewServer"/>
public class PreviewServer : IPreviewServer
{
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyInterCollection = [];
    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Main;

    /// <inheritdoc/>
    public IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// Constructs a new <see cref="IPreviewServer"/>
    /// </summary>
    public PreviewServer()
    {
        this.ObjectRegister = new ObjectRegister();

    }

    /// <summary>
    /// Adds the transient representation of an entity in AutoCAD.
    /// </summary>
    public void AddTransientEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            var autoCadEntity = entity.Unwrap();

            var transientManager = TransientManager.CurrentTransientManager;

            transientManager.AddTransient(autoCadEntity, _transientDrawingMode, _subDrawingMode, _emptyInterCollection);
        }
    }

    /// <summary>
    /// Removes the transient representation of an entity in AutoCAD.
    /// </summary>
    public void RemoveTransientEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            var autoCadEntity = entity.Unwrap();

            var transientManager = TransientManager.CurrentTransientManager;

            transientManager.EraseTransient(autoCadEntity, _emptyInterCollection);
        }
    }

    /// <inheritdoc/>
    public void AddObject(Guid rhinoObjectId, List<IEntity> entities)
    {
        this.ObjectRegister.RegisterObject(rhinoObjectId, entities);

        this.AddTransientEntities(entities);
    }

    /// <inheritdoc/>
    public void RemoveObject(Guid rhinoObjectId)
    {
        if (this.ObjectRegister.TryGetObject(rhinoObjectId, out var entities))
        {
            this.ObjectRegister.RemoveObject(rhinoObjectId);
            this.RemoveTransientEntities(entities);
        }
    }
}