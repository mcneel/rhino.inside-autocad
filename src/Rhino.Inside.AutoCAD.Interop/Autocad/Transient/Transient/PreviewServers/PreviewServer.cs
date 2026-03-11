using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewServer"/>
public class PreviewServer : IPreviewServer
{
    private readonly IGeometryPreviewSettings _previewSettings;
    private readonly IPreviewGeometryConverter _previewGeometryConverter;
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyInterCollection = [];
    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Main;

    /// <inheritdoc/>
    public IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// Constructs a new <see cref="IPreviewServer"/>
    /// </summary>
    public PreviewServer(IGeometryPreviewSettings previewSettings, IPreviewGeometryConverter previewGeometryConverter)
    {
        _previewSettings = previewSettings;
        _previewGeometryConverter = previewGeometryConverter;
        this.ObjectRegister = new ObjectRegister();
    }

    /// <summary>
    /// Adds the transient representation of an entity in AutoCAD.
    /// </summary>
    private void AddTransientEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            var autoCadEntity = entity.Unwrap();

            var transientManager = TransientManager.CurrentTransientManager;

            if (transientManager.AddTransient(autoCadEntity, _transientDrawingMode,
                    _subDrawingMode, _emptyInterCollection) == false)
            {
                LoggerService.Instance.LogMessage("Unable to create Transient element");
            }
        }
    }

    /// <summary>
    /// Removes the transient representation of an entity in AutoCAD.
    /// </summary>
    private void RemoveTransientEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            var autoCadEntity = entity.Unwrap();

            var transientManager = TransientManager.CurrentTransientManager;

            transientManager.EraseTransient(autoCadEntity, _emptyInterCollection);
        }
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    public void ClearServer()
    {
        foreach (var entities in this.ObjectRegister)
        {
            this.RemoveTransientEntities(entities);
        }
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    public void PopulateServer()
    {
        foreach (var entities in this.ObjectRegister)
        {
            this.AddTransientEntities(entities);
        }
    }

    /// <inheritdoc/>
    public void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet)
    {
        if (rhinoConvertibleSet.Any)
        {
            var entities = _previewGeometryConverter.Convert(rhinoConvertibleSet, _previewSettings);

            this.ObjectRegister.RegisterObject(rhinoObjectId, entities);

            this.AddTransientEntities(entities);
        }
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