using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadLayer"/>
public class AutocadLayerWrapper : WrapperDisposableBase<LayerTableRecord>, IAutocadLayer
{
    private readonly InternalColorConverter _internalColorConverter = InternalColorConverter.Instance!;

    /// <inheritdoc/>
    public IColor Color { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IObjectId Id { get; }

    /// <inheritdoc/>
    public IObjectId LinePattenId { get; }

    /// <inheritdoc/>
    public bool IsValid => this.Id.IsValid;

    /// <inheritdoc/>
    public bool IsLocked => this.Internal.IsLocked;

    /// <summary>
    /// Constructs a new <see cref="AutocadLayerWrapper"/>.
    /// </summary>
    public AutocadLayerWrapper(LayerTableRecord layerTableRecord) : base(layerTableRecord)
    {
        this.Name = layerTableRecord.Name;

        this.Id = new AutocadObjectId(layerTableRecord.Id);

        this.LinePattenId = new AutocadObjectId(layerTableRecord.LinetypeObjectId);

        this.Color = _internalColorConverter.Convert(layerTableRecord.Color);

    }

    /// <inheritdoc/>
    public IAutocadLinePattern GetLinePattern(ILinePatternCache linePatternCache)
    {
        return linePatternCache.GetById(this.LinePattenId);
    }

    /// <inheritdoc/>
    public IAutocadLayer ShallowClone()
    {
        return new AutocadLayerWrapper(_wrappedValue);
    }
}