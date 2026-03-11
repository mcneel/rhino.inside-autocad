using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewGeometryConverter"/>
public class PreviewGeometryConverter : IPreviewGeometryConverter
{
    private readonly IAutoCadInstance _autoCadInstance;
    private readonly IEntityValidator _entityValidator;

    /// <summary>
    /// Constructs a new <see cref="PreviewGeometryConverter"/>.
    /// </summary>
    public PreviewGeometryConverter(IAutoCadInstance autoCadInstance)
    {
        _autoCadInstance = autoCadInstance;
        _entityValidator = new EntityValidator();
    }

    /// <summary>
    /// Tries to get the active AutoCAD document.
    /// </summary>
    private bool TryGetActiveDocument(out IAutocadDocument? activeDocument)
    {
        activeDocument = _autoCadInstance.ActiveDocument;

        return activeDocument != null;
    }

    /// <inheritdoc/>
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

                var silent = true;

#if DEBUG
                silent = false;
#endif

                var validEntities = _entityValidator.ValidateEntitiesForTransientManager(convertedEntities, silent);

                entities.AddRange(validEntities);
            }

            return entities;
        });

    }
}