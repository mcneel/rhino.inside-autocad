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
/// <seealso cref="ILayerRegister"/>
public class AutocadLayerTableRecordWrapper : AutocadDbObjectWrapper, IAutocadLayerTableRecord
{
    private readonly LayerTableRecord _layerTableRecord;

    /// <inheritdoc/>
    public IColor Color { get; }

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

        this.Color = new InternalColor(layerTableRecord.Color);
    }

    /// <inheritdoc/>
    public IAutocadLinetypeTableRecord GetLinePattern(ILineTypeRegister lineTypeRegister)
    {
        return lineTypeRegister.TryGetById(this.LineTypeId, out var lineType)
            ? lineType!
            : lineTypeRegister.GetDefault();
    }

    /// <inheritdoc/>
    public override IDbObject ShallowClone()
    {
        return new AutocadLayerTableRecordWrapper(_layerTableRecord);
    }
}