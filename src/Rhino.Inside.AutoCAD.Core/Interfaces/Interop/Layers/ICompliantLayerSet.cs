namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A unique set of <see cref="ILayer"/>s that are compliant with AWI CAD
/// standards 
/// </summary>
public interface ICompliantLayerSet : IEnumerable<ILayer>
{
    /// <summary>
    /// Indicates whether this <see cref="ICompliantLayerSet"/> is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Filters all layers in the <see cref="IAutoCadDocument"/> and adds layers that
    /// are compliant with AWI CAD standards using the <see cref="ILayerFilter"/>.
    /// </summary>
    void FilterLayers();
}