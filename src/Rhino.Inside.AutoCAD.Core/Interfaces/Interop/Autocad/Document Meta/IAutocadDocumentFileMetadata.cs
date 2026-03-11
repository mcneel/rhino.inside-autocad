namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides file metadata and status information for an <see cref="IAutocadDocument"/>.
/// </summary>
/// <remarks>
/// This interface exposes file system properties and document state information
/// for an AutoCAD drawing file. Accessed via <see cref="IAutocadDocument.FileMetadata"/>
/// and used by Grasshopper components such as AutocadDocumentComponent to display
/// document information.
/// </remarks>
/// <seealso cref="IAutocadDocument"/>
public interface IAutocadDocumentFileMetadata
{
    /// <summary>
    /// Gets the file name of the document, including the extension (e.g., "Drawing1.dwg").
    /// </summary>
    /// <remarks>
    /// For unsaved documents, this returns the temporary name assigned by AutoCAD.
    /// </remarks>
    string FileName { get; }

    /// <summary>
    /// Gets the full file path of the document (e.g., "C:\Projects\Drawing1.dwg").
    /// </summary>
    /// <remarks>
    /// For unsaved documents, this may return an empty string or a temporary path
    /// depending on AutoCAD's internal state.
    /// </remarks>
    string FilePath { get; }

    /// <summary>
    /// Gets a value indicating whether this document is the currently active document in AutoCAD.
    /// </summary>
    /// <remarks>
    /// Only one document can be active at a time. The active document receives user input
    /// and is displayed in the main viewport.
    /// </remarks>
    bool IsActive { get; }

    /// <summary>
    /// Gets a value indicating whether this document is read-only.
    /// </summary>
    /// <remarks>
    /// A document may be read-only if the file is locked by another process, opened from
    /// a read-only location, or explicitly opened in read-only mode by the user.
    /// </remarks>
    bool IsReadOnly { get; }
}