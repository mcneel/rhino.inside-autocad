namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a set of <see cref="IHatchLoop"/>s belonging to the one particular 
/// <see cref="IHatch"/>.
/// </summary>
public interface IHatchLoopSet : IEnumerable<IHatchLoop>
{
    /// <summary>
    /// The <see cref="IHatchLoop"/> at the given <paramref name="index"/>.
    /// </summary>
    IHatchLoop this[int index] { get; }

    /// <summary>
    /// The outermost <see cref="IHatchLoop"/> of the <see cref="IHatchLoopSet"/>.
    /// </summary>
    IHatchLoop OutermostLoop { get; }

    /// <summary>
    /// The inner <see cref="IHatchLoop"/>s - the voids in the <see cref="IHatchLoopSet"/>.
    /// Empty if there are no voids.
    /// </summary>
    IList<IHatchLoop> InnerLoops { get; }

    /// <summary>
    /// The total number of <see cref="IHatchLoop"/>s in the <see cref="IHatchLoopSet"/>.
    /// </summary>
    int TotalLoops { get; }
}