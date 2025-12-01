namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides file directory and paths of a <see cref="IAutoCadDocument"/> file.
/// </summary>
public interface IAutocadDocumentFileInfo
{
    /// <summary>
    /// Returns the <see cref="IAutoCadDocument.Id"/>.
    /// </summary>
    Guid UniqueId { get; }

    /// <summary>
    /// The file name of the <see cref="IAutoCadDocument"/>.
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// The file path of the <see cref="IAutoCadDocument"/>.
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// The root directory where the <see cref="IAutoCadDocument"/> is saved.
    /// </summary>
    string RootDirectory { get; }

    /// <summary>
    /// The directory to the resources folder where project-specific resources
    /// are stored.
    /// </summary>
    string ResourceDirectory { get; }
}