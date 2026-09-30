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
    /// <remarks>
    /// The preview material is not created here: this runs under Rhino and Grasshopper
    /// reactors, where the database write creating it needs is not permitted.
    /// <see cref="IPreviewMaterialScheduler"/> creates it out of band, and until it exists
    /// the entities are drawn unshaded.
    /// <para>
    /// The transaction still takes a document lock, because the conversion is not purely a
    /// read: converting a hatch appends its boundary curves to model space to evaluate the
    /// hatch, then erases them again.
    /// </para>
    /// <para>
    /// Conversion stops once <paramref name="maxEntities"/> is reached so an oversized
    /// preview is never built in full. A single geometry can convert to many entities, so the
    /// ones which overshoot the limit are disposed rather than returned.
    /// </para>
    /// </remarks>
    public List<IEntity> Convert(IRhinoConvertibleSet rhinoGeometries, IGeometryPreviewSettings previewSettings,
        int maxEntities)
    {
        if (this.TryGetActiveDocument(out var activeDocument) == false) return new List<IEntity>();

        var transactionManagerWrapper = activeDocument!.CreateTransactionManager();

        return transactionManagerWrapper.PerformTask(() =>
        {
            var entities = new List<IEntity>();
            foreach (var rhinoGeometry in rhinoGeometries)
            {
                if (entities.Count >= maxEntities) break;

                var convertedEntities =
                    rhinoGeometry.Convert(transactionManagerWrapper, previewSettings);

                var silent = true;

#if DEBUG
                silent = false;
#endif

                var validEntities = _entityValidator.ValidateEntitiesForTransientManager(convertedEntities, silent);

                entities.AddRange(validEntities);
            }

            if (entities.Count > maxEntities)
            {
                var surplusCount = entities.Count - maxEntities;

                foreach (var surplusEntity in entities.GetRange(maxEntities, surplusCount))
                {
                    surplusEntity.Unwrap().Dispose();
                }

                entities.RemoveRange(maxEntities, surplusCount);
            }

            return entities;
        });

    }
}