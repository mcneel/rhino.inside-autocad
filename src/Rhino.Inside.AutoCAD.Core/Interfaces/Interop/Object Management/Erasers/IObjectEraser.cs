namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Handles object erasing in the active <see cref="IAutoCadDocument"/>.
/// </summary>
public interface IObjectEraser
{
    /// <summary>
    /// The active <see cref="IAutoCadDocument"/>.
    /// </summary>
    IAutoCadDocument AutoCadDocument { get; }

    /// <summary>
    /// Erases an object by the provided <see cref="IObjectId"/>.
    /// </summary>
    void Erase(IDbObject dbObject);
}