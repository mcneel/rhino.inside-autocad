namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents a repository for handling
/// <see cref="IAutocadLayer"/>s.
/// </summary>
public interface ILayerRepository : IRepository<IAutocadLayer>, IEnumerable<IAutocadLayer>
{
    /// <summary>
    /// The layer added event handler. This event is raised when a new
    /// <see cref="IAutocadLayer"/> is added to this <see cref="ILayerRepository"/>.
    /// </summary>
    event EventHandler<ILayerAddedEventArgs>? LayerAdded;

    /// <summary>
    /// Layer Table modified event handler. This event is raised when the layer table
    /// is modified.
    /// </summary>
    event EventHandler? LayerTableModified;

    /// <summary>
    /// Check if the given <paramref name="layer"/> exists in the
    /// <see cref="ILayerRepository"/>. Returns true if the
    /// <paramref name="layer"/> is found, false otherwise.
    /// </summary>
    bool Exists(IAutocadLayer layer);

    /// <summary>
    /// Attempts to add a new <see cref="IAutocadLayer"/> to this <see cref="ILayerRepository"/>
    /// and creates it in the active document. If the <see cref="IAutocadLayer"/> already exists
    /// no change is made.
    /// </summary>
    bool TryAddLayer(IColor color, string name, out IAutocadLayer layer);

    /// <summary>
    /// Returns the default layer 0 from AutoCAD.
    /// </summary>
    IAutocadLayer GetDefault();

    /// <summary>
    /// Attempts to return the <see cref="IAutocadLayer"/> by name. If no matching layer is found
    /// returns the <see cref="GetDefault"/>default layer 0 from AutoCAD.
    /// </summary>
    IAutocadLayer GetByNameOrDefault(string name);

    /// <summary>
    /// Retrieves layers from the <see cref="ILayerRepository"/>
    /// by provided <see cref="names"/>.
    /// </summary>
    IList<IAutocadLayer> GetLayers(IList<string> names);
}