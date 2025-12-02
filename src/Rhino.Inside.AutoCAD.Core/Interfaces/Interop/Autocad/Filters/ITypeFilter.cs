namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A filter that selects entities of a specific type.
/// </summary>
public interface ITypeFilter : IFilter
{
    /// <summary>
    /// The type name to filter by (e.g. "LINE", "CIRCLE", etc.).
    /// </summary>
    string TypeName { get; }
}
