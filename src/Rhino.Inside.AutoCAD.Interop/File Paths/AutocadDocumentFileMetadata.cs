using Autodesk.AutoCAD.ApplicationServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDocumentFileMetadata"/>
public class AutocadDocumentFileMetadata : IAutocadDocumentFileMetadata
{
    private readonly Document _document;

    /// <inheritdoc />
    public string FileName { get; }

    /// <inheritdoc />
    public string FilePath { get; }

    /// <inheritdoc />
    public bool IsActive => _document.IsActive;

    /// <inheritdoc />
    public bool IsReadOnly => _document.IsReadOnly;

    /// <summary>
    /// Constructs a new <see cref="IAutocadDocumentFileMetadata"/>.
    /// </summary>
    public AutocadDocumentFileMetadata(Document document)
    {
        _document = document;

        var filePath = document.Database.Filename;

        this.FileName = Path.GetFileNameWithoutExtension(filePath);

        this.FilePath = filePath;
    }
}