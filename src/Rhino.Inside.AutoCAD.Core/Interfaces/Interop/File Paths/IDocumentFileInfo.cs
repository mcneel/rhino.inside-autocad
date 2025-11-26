namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides file directory and paths of a <see cref="IDocument"/> file.
/// </summary>
public interface IDocumentFileInfo
{
    /// <summary>
    /// Returns the <see cref="IDocument.Id"/>.
    /// </summary>
    Guid UniqueId { get; }

    /// <summary>
    /// The file name of the <see cref="IDocument"/>.
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// The file path of the <see cref="IDocument"/>.
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// The root directory where the <see cref="IDocument"/> is saved.
    /// </summary>
    string RootDirectory { get; }

    /// <summary>
    /// The directory to the resources folder where project-specific resources
    /// are stored.
    /// </summary>
    string ResourceDirectory { get; }
}