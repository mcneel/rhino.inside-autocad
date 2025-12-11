using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewGeometryConverter"/>
public class PreviewGeometryConverter : IPreviewGeometryConverter
{
    private readonly IAutoCadInstance _autoCadInstance;

    /// <summary>
    /// Constructs a new <see cref="PreviewGeometryConverter"/>.
    /// </summary>
    public PreviewGeometryConverter(IAutoCadInstance autoCadInstance)
    {
        _autoCadInstance = autoCadInstance;
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

                entities.AddRange(convertedEntities);
            }

            return entities;
        });

    }
}
