namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Handles object erasing in the active <see cref="IDocument"/>.
/// </summary>
public interface IObjectEraser
{
    /// <summary>
    /// The active <see cref="IDocument"/>.
    /// </summary>
    IDocument Document { get; }

    /// <summary>
    /// Erases an object by the provided <see cref="IObjectId"/>.
    /// </summary>
    void Erase(IDbObject dbObject);
}