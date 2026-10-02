using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewShell"/>
public class PreviewShell : IPreviewShell
{
    /// <inheritdoc/>
    public IPoint3dCollection Vertices { get; }

    /// <inheritdoc/>
    public IIntegerCollection Faces { get; }

    /// <summary>
    /// Constructs a new <see cref="PreviewShell"/>.
    /// </summary>
    /// <param name="vertices">The vertices of the shell.</param>
    /// <param name="faces">The faces of the shell in AutoCAD shell format.</param>
    public PreviewShell(IPoint3dCollection vertices, IIntegerCollection faces)
    {
        this.Vertices = vertices;
        this.Faces = faces;
    }
}
