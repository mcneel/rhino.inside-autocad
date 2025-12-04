using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An event args class created when a <see cref="IAutocadLayer"/> is added
/// to the <see cref="ILayerRepository"/>.
/// </summary>
public class LayerAddedEventArgs : EventArgs, ILayerAddedEventArgs
{
    /// <summary>
    /// The layer added to the <see cref="ILayerRepository"/> that raised this
    /// event args.
    /// </summary>
    public IAutocadLayer Layer { get; }

    /// <summary>
    /// Constructs a new <see cref="LayerAddedEventArgs"/>.
    /// </summary>
    public LayerAddedEventArgs(IAutocadLayer layer)
    {
        this.Layer = layer;
    }
}