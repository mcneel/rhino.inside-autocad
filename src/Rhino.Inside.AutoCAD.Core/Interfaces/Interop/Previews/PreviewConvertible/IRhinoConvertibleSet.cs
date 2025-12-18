namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A set of <see cref="IRhinoConvertible"/> items.
/// </summary>
public interface IRhinoConvertibleSet : IEnumerable<IRhinoConvertible>
{
    /// <summary>
    /// A boolean indicating whether the set contains any items.
    /// </summary>
    bool Any { get; }

    /// <summary>
    /// Adds a <see cref="IRhinoConvertible"/> to the set.
    /// </summary>
    /// <param name="convertible"></param>
    void Add(IRhinoConvertible convertible);
}