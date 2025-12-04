namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides file directory and paths of a <see cref="IAutocadDocument"/> file.
/// </summary>
public interface IAutocadDocumentFileInfo
{
    /// <summary>
    /// Returns the <see cref="IAutocadDocument.Id"/>.
    /// </summary>
    Guid UniqueId { get; }

    /// <summary>
    /// The file name of the <see cref="IAutocadDocument"/>.
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// The file path of the <see cref="IAutocadDocument"/>.
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// The root directory where the <see cref="IAutocadDocument"/> is saved.
    /// </summary>
    string RootDirectory { get; }

    /// <summary>
    /// Returns whether the <see cref="IAutocadDocument"/> is currently active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Returns whether the <see cref="IAutocadDocument"/> is read-only.
    /// </summary>
    bool IsReadOnly { get; }
}