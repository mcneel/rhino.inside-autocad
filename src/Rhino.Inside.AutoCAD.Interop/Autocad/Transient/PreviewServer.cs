using Autodesk.AutoCAD.ApplicationServices;
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