using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoObjectPreviewer"/>
public class RhinoObjectPreviewer : IRhinoObjectPreviewer
{
    private readonly IObjectRegister _objectRegister;
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyInterCollection = [];
    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Contrast;

    /// <inheritdoc/>
    public bool Visible { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="RhinoObjectPreviewer"/>
    /// </summary>
    public RhinoObjectPreviewer(IObjectRegister objectRegister)
    {
        _objectRegister = objectRegister;
        this.Visible = true;
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void UpdateTransientElements()
    {
        if (this.Visible)
        {
            foreach (var entities in _objectRegister)
            {
                this.AddEntities(entities);
            }
        }
        else
        {
            foreach (var entities in _objectRegister)
            {
                this.RemoveEntities(entities);
            }
        }
    }

    /// <inheritdoc/>
    public void AddEntity(IEntity entity)
    {
        var autoCadEntity = entity.Unwrap();

        var transientManager = Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager;

        transientManager.AddTransient(autoCadEntity, _transientDrawingMode, _subDrawingMode, _emptyInterCollection);
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

        var transientManager = Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager;

        transientManager.EraseTransient(autoCadEntity, _emptyInterCollection);
    }

    /// <inheritdoc/>
    public void RemoveEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            this.RemoveEntity(entity);
        }
    }

    /// <inheritdoc />
    public void ToggleVisibility()
    {
        this.Visible = !this.Visible;

        this.UpdateTransientElements();
    }
}