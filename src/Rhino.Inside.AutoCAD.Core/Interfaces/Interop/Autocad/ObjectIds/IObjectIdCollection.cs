namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a collection of <see cref="IObjectId"/>s.
/// </summary>
public interface IObjectIdCollection : IEnumerable<IObjectId>
{
    /// <summary>
    /// Adds an <see cref="IObjectId"/> to the <see cref="IObjectIdCollection"/>.
    /// </summary>
    void Add(IObjectId entity);

    /// <summary>
    /// Adds a collection of <see cref="IObjectId"/>s to the <see cref="IObjectIdCollection"/>.
    /// </summary>
    void Add(IObjectIdCollection entityCollection);
}