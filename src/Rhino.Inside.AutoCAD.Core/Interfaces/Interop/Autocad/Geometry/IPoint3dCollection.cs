namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an AutoCAD collection of 3D points, as handed to the AutoCAD graphics API.
/// </summary>
/// <remarks>
/// Only the members the preview drawables need are exposed; the Interop layer unwraps it to
/// the native AutoCAD collection to fill and draw it.
/// </remarks>
public interface IPoint3dCollection
{
    /// <summary>
    /// Gets the number of points in the collection.
    /// </summary>
    int Count { get; }
}
