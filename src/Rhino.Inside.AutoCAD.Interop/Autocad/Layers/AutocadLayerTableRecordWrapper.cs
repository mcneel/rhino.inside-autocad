using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadLayerTableRecord"/>
public class AutocadLayerTableRecordWrapper : DbObjectWrapper, IAutocadLayerTableRecord
{
    private readonly LayerTableRecord _layerTableRecord;
    private readonly AutocadColorConverter _autocadColorConverter = AutocadColorConverter.Instance!;

    /// <inheritdoc/>
    public IColor Color { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IObjectId LinePattenId { get; }

    /// <inheritdoc/>
    public bool IsLocked => _layerTableRecord.IsLocked;

    /// <summary>
    /// Constructs a new <see cref="AutocadLayerTableRecordWrapper"/>.
    /// </summary>
    public AutocadLayerTableRecordWrapper(LayerTableRecord layerTableRecord) : base(layerTableRecord)
    {
        _layerTableRecord = layerTableRecord;

        this.Name = layerTableRecord.Name;

        this.LinePattenId = new AutocadObjectId(layerTableRecord.LinetypeObjectId);

        this.Color = _autocadColorConverter.Convert(layerTableRecord.Color);

    }

    /// <inheritdoc/>
    public IAutocadLinetypeTableRecord GetLinePattern(ILineTypeRepository lineTypeRepository)
    {
        return lineTypeRepository.GetById(this.LinePattenId);
    }

    /// <inheritdoc/>
    public IAutocadLayerTableRecord ShallowClone()
    {
        return new AutocadLayerTableRecordWrapper(_layerTableRecord);
    }
}