using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadLayerTableRecord"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="LayerTableRecord"/> to expose layer properties such as
/// <see cref="Name"/>, <see cref="Color"/>, and <see cref="LineTypeId"/>.
/// Used by the Grasshopper library in layer components including
/// <c>AutocadLayerComponent</c>, <c>GetAutocadLayersComponent</c>, and <c>CreateAutocadLayerComponent</c>.
/// </remarks>
public class AutocadLayerTableRecordWrapper : AutocadDbObjectWrapper, IAutocadLayerTableRecord
{
    private readonly LayerTableRecord _layerTableRecord;

    /// <inheritdoc/>
    public IAutocadColor Color { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IObjectId LineTypeId { get; }

    /// <inheritdoc/>
    public bool IsLocked => _layerTableRecord.IsLocked;

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadLayerTableRecordWrapper"/>.
    /// </summary>
    /// <param name="layerTableRecord">
    /// The AutoCAD <see cref="LayerTableRecord"/> to wrap.
    /// </param>
    public AutocadLayerTableRecordWrapper(LayerTableRecord layerTableRecord) : base(layerTableRecord)
    {
        _layerTableRecord = layerTableRecord;

        this.Name = layerTableRecord.Name;

        this.LineTypeId = new AutocadObjectIdWrapper(layerTableRecord.LinetypeObjectId);

        this.Color = new AutocadColorWrapper(layerTableRecord.Color);
    }

    /// <inheritdoc/>
    public override IDbObject ShallowClone()
    {
        return new AutocadLayerTableRecordWrapper(_layerTableRecord);
    }

    /// <summary>
    /// Creates a new layer in the active document and returns the <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    public static IAutocadLayerTableRecord Create(IAutocadDocument document, IAutocadColor color, string name)
    {
        var transactionManagerWrapper = document.CreateTransactionManager();

        var layerWrapper = transactionManagerWrapper.PerformTask(() =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var newLayer = new LayerTableRecord
            {
                Name = name,
                Color = color.Unwrap()
            };

            var layerTable = (LayerTable)transactionManager.GetObject(
                document.AutocadDatabase.LayerTableId.Unwrap(), OpenMode.ForWrite);

            layerTable.Add(newLayer);

            transactionManager.AddNewlyCreatedDBObject(newLayer, true);

            return new AutocadLayerTableRecordWrapper(newLayer);
        });

        document.Regenerate();

        return layerWrapper;
    }
}