namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A shell buffer, in AutoCAD units, ready to be drawn as an AutoCAD graphics shell.
/// </summary>
public interface IPreviewShell
{
    /// <summary>
    /// Gets the vertices of the shell.
    /// </summary>
    IPoint3dCollection Vertices { get; }

    /// <summary>
    /// Gets the faces of the shell in AutoCAD shell format: a vertex count followed by that
    /// many indices into <see cref="Vertices"/>, repeated per face.
    /// </summary>
    IIntegerCollection Faces { get; }
}
