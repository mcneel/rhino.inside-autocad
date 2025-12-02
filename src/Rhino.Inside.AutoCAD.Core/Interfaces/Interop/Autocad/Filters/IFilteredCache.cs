namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A cache of <see cref="IDocument"/> entities filtered by a specific <see
/// cref="IFilter"/>.
/// </summary>
public interface IFilteredCache : IDisposable
{
    /// <summary>
    /// Returns the filter used to populate the cache.
    /// </summary>
    IFilter Filter { get; }

    /// <summary>
    /// Returns the collection of entities in the cache.
    /// </summary>
    IEntityCollection GetCollection();
}