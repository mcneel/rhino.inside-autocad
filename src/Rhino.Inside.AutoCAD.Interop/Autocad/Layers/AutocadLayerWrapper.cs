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
    public IAutocadLinePattern AutocadLinePattern { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IObjectId Id { get; }

    /// <inheritdoc/>
    public bool IsValid => this.Id.IsValid;

    /// <inheritdoc/>
    public bool IsLocked => this.Internal.IsLocked;

    /// <summary>
    /// Constructs a new <see cref="AutocadLayerWrapper"/>.
    /// </summary>
    public AutocadLayerWrapper(LayerTableRecord layerTableRecord, IAutocadLinePattern pattern) : base(layerTableRecord)
    {
        this.Name = layerTableRecord.Name;

        this.Id = new ObjectId(layerTableRecord.Id);

        this.Color = _internalColorConverter.Convert(layerTableRecord.Color);

        this.AutocadLinePattern = pattern;
    }

    /// <inheritdoc/>
    public IAutocadLayer ShallowClone()
    {
        return new AutocadLayerWrapper(_wrappedValue, this.AutocadLinePattern);
    }
}