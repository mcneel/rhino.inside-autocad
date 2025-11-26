using Autodesk.AutoCAD.ApplicationServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides file directory and paths of a <see cref="IDocument"/> file.
/// </summary>
public class DocumentFileInfo : IDocumentFileInfo
{
    private readonly string _resourcesFolderName = InteropConstants.ResourcesFolderName;

    /// <summary>
    /// Returns a unique hashcode ID based on the <see cref="FilePath"/>. This ensures
    /// the hash is always reliable regardless of whether the <see cref="IDocument"/> is
    /// closed and reopened since the <see cref="FilePath"/> is always unique.
    /// </summary>
    public Guid UniqueId { get; }

    /// <summary>
    /// The file name of the <see cref="IDocument"/>.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// The file path of the <see cref="IDocument"/>.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// The root directory where the <see cref="IDocument"/> is saved.
    /// </summary>
    public string RootDirectory { get; }

    /// <summary>
    /// The directory to the resources folder where project-specific resources
    /// are stored.
    /// </summary>
    public string ResourceDirectory { get; }

    /// <summary>
    /// Constructs a new <see cref="DocumentFileInfo"/>.
    /// </summary>
    public DocumentFileInfo(Document document, Guid id)
    {
        var filePath = document.Database.Filename;

        var rootDirectory = File.Exists(filePath) ? Path.GetDirectoryName(filePath) : string.Empty;

        var resourceDirectory = $"{rootDirectory}\\{_resourcesFolderName}\\";

        if (Directory.Exists(resourceDirectory) == false)
            Directory.CreateDirectory(resourceDirectory);

        this.UniqueId = id;

        this.FileName = Path.GetFileNameWithoutExtension(filePath);

        this.FilePath = filePath;

        this.RootDirectory = rootDirectory!;

        this.ResourceDirectory = resourceDirectory;
    }
}