using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewServer"/>
public class PreviewServer : IPreviewServer
{
    private readonly IGeometryPreviewSettings _previewSettings;
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyInterCollection = [];
    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Contrast;

    /// <inheritdoc/>
    public IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// Constructs a new <see cref="IPreviewServer"/>
    /// </summary>
    public PreviewServer(IGeometryPreviewSettings previewSettings)
    {
        _previewSettings = previewSettings;
        this.ObjectRegister = new ObjectRegister();
    }

    /// <summary>
    /// Adds the transient representation of an entity in AutoCAD.
    /// </summary>
    public void AddTransientEntities(IEnumerable<IEntity> entities)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        foreach (var entity in entities)
        {
            var autoCadEntity = entity.Unwrap();

            using (var transaction = activeDocument.Database.TransactionManager.StartTransaction())
            {
                if (autoCadEntity.IsErased)
                {
                    var obj = transaction.GetObject(autoCadEntity.Id, OpenMode.ForWrite, openErased: true);

                    obj.Erase(false);
                }

                if (autoCadEntity.IsWriteEnabled == false)
                {
                    autoCadEntity.UpgradeOpen();
                }

                var materialId = _previewSettings.MaterialId.Unwrap();
                autoCadEntity.ColorIndex = _previewSettings.ColorIndex;
                autoCadEntity.LineWeight = LineWeight.LineWeight050;
                autoCadEntity.Transparency = new Transparency(_previewSettings.Transparency);

                if (materialId.IsValid)
                {
                    autoCadEntity.MaterialId = materialId;
                }

                transaction.Commit();
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
        if (entities.Count > 0)
        {

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

/// <inheritdoc cref="IPreviewServer"/>
public class PreviewServer2 : IPreviewServer2
{
    private readonly IGeometryPreviewSettings _previewSettings;
    private readonly IPreviewGeometryConverter _previewGeometryConverter;
    private readonly int _subDrawingMode = 0;
    private readonly IntegerCollection _emptyInterCollection = [];
    private readonly TransientDrawingMode _transientDrawingMode = TransientDrawingMode.Contrast;

    /// <inheritdoc/>
    public IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// Constructs a new <see cref="IPreviewServer"/>
    /// </summary>
    public PreviewServer2(IGeometryPreviewSettings previewSettings, IPreviewGeometryConverter previewGeometryConverter)
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

public interface IPreviewGeometryConverter
{
    List<IEntity> Convert(IRhinoConvertibleSet rhinoGeometries, IGeometryPreviewSettings previewSettings);
}

public class PreviewGeometryConverter : IPreviewGeometryConverter
{
    private readonly IAutoCadInstance _autoCadInstance;

    public PreviewGeometryConverter(IAutoCadInstance autoCadInstance)
    {
        _autoCadInstance = autoCadInstance;
    }

    private bool TryGetActiveDocument(out IAutocadDocument? activeDocument)
    {
        activeDocument = _autoCadInstance.ActiveDocument;

        return activeDocument != null;
    }

    public List<IEntity> Convert(IRhinoConvertibleSet rhinoGeometries, IGeometryPreviewSettings previewSettings)
    {
        if (this.TryGetActiveDocument(out var activeDocument) == false) return new List<IEntity>();

        return activeDocument.Transaction(transactionManagerWrapper =>
        {

            var entities = new List<IEntity>();
            foreach (var rhinoGeometry in rhinoGeometries)
            {

                var convertedEntities =
                    rhinoGeometry.Convert(transactionManagerWrapper, previewSettings);

                entities.AddRange(convertedEntities);
            }

            return entities;
        });

    }
}

public class RhinoConvertibleSet : IRhinoConvertibleSet
{
    private readonly List<IRhinoConvertible> _set
        = new List<IRhinoConvertible>();

    public bool Any => _set.Count > 0;

    public void Add(IRhinoConvertible convertible)
    {
        _set.Add(convertible);
    }

    public IEnumerator<IRhinoConvertible> GetEnumerator() => _set.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public interface IRhinoConvertibleFactory
{
    bool MakeConvertible<TRhinoType>(TRhinoType rhinoGeometry,
        out IRhinoConvertible? result)
        where TRhinoType : Rhino.Geometry.GeometryBase;
}

public interface IRhinoConvertibleTyped<TRhinoType> : IRhinoConvertible
where TRhinoType : Rhino.Geometry.GeometryBase
{
    TRhinoType RhinoGeometry { get; }
}
