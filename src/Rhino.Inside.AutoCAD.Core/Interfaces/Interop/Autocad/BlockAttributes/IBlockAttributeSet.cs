namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a collection of the <see cref="IAttributeWrapper"/>s.
/// </summary>
public interface IBlockAttributeSet : IEnumerable<IAttributeWrapper>
{
    /// <summary>
    /// Gets the number of <see cref="IAttributeWrapper"/>s in the <see
    /// cref="IBlockAttributeSet"/>.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Adds the provided <see cref="IAttributeWrapper"/> in the <see
    /// cref= "IBlockAttributeSet"/>.
    /// </summary>
    void Add(IAttributeWrapper property);

    /// <summary>
    /// Tries to get the <see cref="IAttributeWrapper"/> with the
    /// specified name.
    /// </summary>
    bool TryGet(string name, out IAttributeWrapper property);
}