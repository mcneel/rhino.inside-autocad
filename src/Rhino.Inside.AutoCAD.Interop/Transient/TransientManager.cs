using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which wraps an AutoCAD <see cref="TransientManager"/> providing
/// functionality for managing transient entities.
/// </summary>
public class TransientManagerWrapper : WrapperDisposableBase<TransientManager>, ITransientManager
{
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyInterCollection = [];

    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Contrast;

    /// <summary>
    /// Constructs a new <see cref="TransientManagerWrapper"/>
    /// </summary>
    public TransientManagerWrapper(TransientManager transientManager)
        : base(transientManager) { }

    /// <inheritdoc/>
    public void AddEntity(IEntity entity)
    {
        var autoCadEntity = entity.Unwrap();

        _wrappedValue.AddTransient(autoCadEntity, _transientDrawingMode, _subDrawingMode, _emptyInterCollection);
    }

    /// <inheritdoc/>
    public void AddEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            this.AddEntity(entity);
        }
    }

    /// <inheritdoc/>
    public void RemoveEntity(IEntity entity)
    {
        var autoCadEntity = entity.Unwrap();

        _wrappedValue.EraseTransient(autoCadEntity, _emptyInterCollection);
    }

    /// <inheritdoc/>
    public void RemoveEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            this.RemoveEntity(entity);
        }
    }
}