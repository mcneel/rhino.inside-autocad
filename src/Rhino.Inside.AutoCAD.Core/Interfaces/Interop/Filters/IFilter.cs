namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a filter which can be used to filter objects from the
/// <see cref="IDocument"/>.
/// </summary>
public interface IFilter
{
    /// <summary>
    /// Returns the selection filter wrapper used by the filter.
    /// </summary>
    ISelectionFilter GetSelectionFilter();
}