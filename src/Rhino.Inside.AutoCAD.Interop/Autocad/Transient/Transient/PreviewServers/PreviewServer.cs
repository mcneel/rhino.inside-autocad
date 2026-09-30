using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewServer"/>
public class PreviewServer : IPreviewServer
{
    private readonly IGeometryPreviewSettings _previewSettings;
    private readonly IGeometryPreviewSettings _selectedPreviewSettings;
    private readonly IPreviewGeometryConverter _previewGeometryConverter;
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyInterCollection = [];
    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Main;
    private int _maxEntityCount;

    /// <inheritdoc/>
    public IObjectRegister ObjectRegister { get; }

    /// <inheritdoc/>
    public bool Visible { get; private set; } = true;

    /// <inheritdoc/>
    public int MaxEntityCount
    {
        get => _maxEntityCount;
        set
        {
            _maxEntityCount = value;

            this.EvictOldestObjects(0);
        }
    }

    /// <summary>
    /// Constructs a new <see cref="IPreviewServer"/>
    /// </summary>
    public PreviewServer(IGeometryPreviewSettings previewSettings, IGeometryPreviewSettings selectedPreviewSettings,
        IPreviewGeometryConverter previewGeometryConverter, int maxEntityCount)
    {
        _previewSettings = previewSettings;
        _selectedPreviewSettings = selectedPreviewSettings;
        _previewGeometryConverter = previewGeometryConverter;
        _maxEntityCount = maxEntityCount;
        this.ObjectRegister = new ObjectRegister();
    }

    /// <summary>
    /// Removes and disposes the oldest registered objects until
    /// <paramref name="incomingEntityCount"/> more entities fit within
    /// <see cref="MaxEntityCount"/>.
    /// </summary>
    private void EvictOldestObjects(int incomingEntityCount)
    {
        var evictedCount = 0;

        while (this.ObjectRegister.EntityCount + incomingEntityCount > _maxEntityCount &&
               this.ObjectRegister.TryGetOldest(out var oldestId, out var oldestEntities))
        {
            this.ObjectRegister.RemoveObject(oldestId);

            this.RemoveTransientEntities(oldestEntities, disposeEntities: true);

            evictedCount++;
        }

        if (evictedCount > 0)
        {
            LoggerService.Instance.LogMessage(
                $"Preview limit of {_maxEntityCount} entities reached: removed the {evictedCount} oldest preview(s).");
        }
    }

    /// <summary>
    /// Adds the transient representation of an entity in AutoCAD.
    /// </summary>
    private void AddTransientEntities(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            var autoCadEntity = entity.Unwrap();

            // Guard clause: skip if entity is null or already disposed
            if (autoCadEntity == null || autoCadEntity.IsDisposed)
            {
                continue;
            }

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
    ///  <remarks>
    /// TransientManager can throw if the entity was already erased or disposed, such
    /// as during closing of the application when we try to clear and dispose all
    /// entities. In those cases, so we still need to dispose the entities
    /// if disposeEntities is true.
    /// </remarks>
    /// <param name="entities">The entities to remove from the transient manager.</param>
    /// <param name="disposeEntities">If true, disposes the entities after removal.</param>
    private void RemoveTransientEntities(IEnumerable<IEntity> entities,
        bool disposeEntities = false)
    {
        try
        {
            var transientManager =
                TransientManager
                    .CurrentTransientManager; //Throws here in Civil Application Shutdown

            foreach (var entity in entities)
            {
                var autoCadEntity = entity.Unwrap();

                transientManager.EraseTransient(autoCadEntity, _emptyInterCollection);

                if (disposeEntities)
                {
                    autoCadEntity.Dispose();
                }
            }
        }
        catch (Exception e)
        {
            foreach (var entity in entities)
            {
                var autoCadEntity = entity.Unwrap();

                if (disposeEntities)
                {
                    autoCadEntity.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// Removes transient elements from display but keeps them in the register for later re-use.
    /// Used for visibility toggling (preview on/off).
    /// </summary>
    public void ClearServer()
    {
        this.Visible = false;

        foreach (var entities in this.ObjectRegister)
        {
            this.RemoveTransientEntities(entities);
        }
    }

    /// <summary>
    /// Removes all transient elements and disposes the underlying AutoCAD entities.
    /// Used during application shutdown to ensure clean disposal.
    /// </summary>
    public void ClearAndDisposeAll()
    {
        System.Diagnostics.Debug.WriteLine("PreviewServer.ClearAndDisposeAll() - disposing entities");

        foreach (var entities in this.ObjectRegister)
        {
            this.RemoveTransientEntities(entities, disposeEntities: true);
        }

        System.Diagnostics.Debug.WriteLine("PreviewServer.ClearAndDisposeAll() - complete");
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    public void PopulateServer()
    {
        this.Visible = true;

        foreach (var entities in this.ObjectRegister)
        {
            this.AddTransientEntities(entities);
        }
    }

    /// <inheritdoc/>
    public void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet, bool selected)
    {
        if (rhinoConvertibleSet.Any)
        {
            var settings = selected ? _selectedPreviewSettings : _previewSettings;

            // Anything already registered under this id is replaced, so it must not count
            // against the room the new entities need.
            this.RemoveObject(rhinoObjectId);

            var entities = _previewGeometryConverter.Convert(rhinoConvertibleSet, settings, _maxEntityCount);

            if (entities.Count == _maxEntityCount)
            {
                LoggerService.Instance.LogMessage(
                    $"Preview limit of {_maxEntityCount} entities reached: preview may be incomplete.");
            }

            this.EvictOldestObjects(entities.Count);

            this.ObjectRegister.RegisterObject(rhinoObjectId, entities);

            // Only draw when the server is visible; hidden servers keep the entities
            // registered so PopulateServer can display them when visibility returns.
            if (this.Visible)
            {
                this.AddTransientEntities(entities);
            }
        }
    }

    /// <inheritdoc/>
    public void RemoveObject(Guid rhinoObjectId)
    {
        if (this.ObjectRegister.TryGetObject(rhinoObjectId, out var entities))
        {
            this.ObjectRegister.RemoveObject(rhinoObjectId);
            this.RemoveTransientEntities(entities, disposeEntities: true);
        }
    }

    /// <summary>
    /// Applies the preview settings to the given entity.
    /// </summary>
    private void ApplySettings(IEntity entity, IGeometryPreviewSettings previewSettings)
    {
        var autocadEntity = entity.Unwrap();

        var materialId = previewSettings.MaterialId.Unwrap();

        autocadEntity.ColorIndex = previewSettings.ColorIndex;

        autocadEntity.LineWeight = LineWeight.LineWeight050;

        autocadEntity.Transparency = new Transparency(previewSettings.Transparency);

        if (materialId.IsValid)
        {
            autocadEntity.MaterialId = materialId;
        }
    }

    /// <inheritdoc />
    public void DeselectAll()
    {
        foreach (var entities in this.ObjectRegister)
        {
            if (this.Visible)
            {
                this.RemoveTransientEntities(entities);
            }

            foreach (var entity in entities)
            {
                 this.ApplySettings(entity, _previewSettings);
            }

            if (this.Visible)
            {
                this.AddTransientEntities(entities);
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Restyling an entity does not redraw it, so the transients are erased and added back to
    /// force AutoCAD to draw them again. The caller is left to restore the visibility state,
    /// as adding transients back shows previews which are currently toggled off.
    /// </remarks>
    public void RefreshAppearance()
    {
        this.DeselectAll();
    }
}
