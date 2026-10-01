namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an AutoCAD collection of native pointer values, as handed to the AutoCAD graphics API.
/// </summary>
/// <remarks>
/// Only the members the preview drawables need are exposed; the Interop layer unwraps it to
/// the native AutoCAD collection to fill and draw it.
/// </remarks>
public interface IIntPtrCollection
{
    /// <summary>
    /// Gets the number of values in the collection.
    /// </summary>
    int Count { get; }
}
