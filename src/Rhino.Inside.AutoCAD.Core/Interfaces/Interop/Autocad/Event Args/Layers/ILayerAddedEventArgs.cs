namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An event args interface created when a <see cref="IAutocadLayer"/> is added
/// to the <see cref="ILayerRepository"/>.
/// </summary>
public interface ILayerAddedEventArgs
{
    /// <summary>
    /// The layer added to the <see cref="ILayerRepository"/> that raised this
    /// event args.
    /// </summary>
    IAutocadLayer Layer { get; }
}