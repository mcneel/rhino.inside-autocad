using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ILayer"/>
public class LayerWrapper : WrapperDisposableBase<LayerTableRecord>, ILayer
{
    private readonly InternalColorConverter _internalColorConverter = InternalColorConverter.Instance!;

    /// <inheritdoc/>
    public IColor Color { get; }

    /// <inheritdoc/>
    public ILinePattern LinePattern { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IObjectId Id { get; }

    /// <inheritdoc/>
    public bool IsValid => this.Id.IsValid;

    /// <inheritdoc/>
    public bool IsLocked => this.Internal.IsLocked;

    /// <summary>
    /// Constructs a new <see cref="LayerWrapper"/>.
    /// </summary>
    public LayerWrapper(LayerTableRecord layerTableRecord, ILinePattern pattern) : base(layerTableRecord)
    {
        this.Name = layerTableRecord.Name;

        this.Id = new ObjectId(layerTableRecord.Id);

        this.Color = _internalColorConverter.Convert(layerTableRecord.Color);

        this.LinePattern = pattern;
    }
}