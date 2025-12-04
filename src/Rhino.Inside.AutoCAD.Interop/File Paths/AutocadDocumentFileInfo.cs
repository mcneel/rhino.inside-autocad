using Autodesk.AutoCAD.ApplicationServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDocumentFileInfo"/>
public class AutocadDocumentFileInfo : IAutocadDocumentFileInfo
{
    private readonly Document _document;

    /// <summary>
    /// Returns a unique hashcode ID based on the <see cref="FilePath"/>. This ensures
    /// the hash is always reliable regardless of whether the <see cref="IAutocadDocument"/> is
    /// closed and reopened since the <see cref="FilePath"/> is always unique.
    /// </summary>
    public Guid UniqueId { get; }

    /// <inheritdoc />
    public string FileName { get; }

    /// <inheritdoc />
    public string FilePath { get; }

    /// <inheritdoc />
    public string RootDirectory { get; }

    /// <inheritdoc />
    public bool IsActive => _document.IsActive;

    /// <inheritdoc />
    public bool IsReadOnly => _document.IsReadOnly;

    /// <summary>
    /// Constructs a new <see cref="AutocadDocumentFileInfo"/>.
    /// </summary>
    public AutocadDocumentFileInfo(Document document, Guid id)
    {
        _document = document;
        var filePath = document.Database.Filename;

        var rootDirectory = File.Exists(filePath) ? Path.GetDirectoryName(filePath) : string.Empty;

        this.UniqueId = id;

        this.FileName = Path.GetFileNameWithoutExtension(filePath);

        this.FilePath = filePath;

        this.RootDirectory = rootDirectory!;
    }
}