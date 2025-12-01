namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A filter that selects entities of a specific name.
/// </summary>
public interface IBlockNameFilter : IFilter
{
    /// <summary>
    /// The name to filter by.
    /// </summary>
    string Name { get; }
}