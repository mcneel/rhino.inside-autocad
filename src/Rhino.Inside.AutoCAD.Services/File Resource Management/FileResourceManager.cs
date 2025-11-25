using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class providing essential file resource directories and file names
/// for obtaining external resources.
/// </summary>
public class FileResourceManager : IRhinoInsideAutoCadFileResourceManager
{
    private IFilepath? _currentFile = null;

    /// <inheritdoc/>
    public IApplicationDirectories ApplicationDirectories { get; }

    /// <inheritdoc/>
    public IFileNameLibrary FileNameLibrary { get; }

    /// <inheritdoc/>
    public IJsonNameLibrary JsonNameLibrary { get; }

    /// <inheritdoc/>
    public IJsonResourceImporter JsonResourceImporter { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideAutoCadFileResourceManager"/>
    /// </summary>
    public FileResourceManager(IApplicationDirectories applicationDirectories, IRhinoInsideAutoCadSettingsManager settingsManager)
    {
        this.ApplicationDirectories = applicationDirectories;

        this.FileNameLibrary = settingsManager.Application.FileNameLibrary;

        this.JsonNameLibrary = settingsManager.Application.JsonNameLibrary;

        this.JsonResourceImporter = new JsonResourceImporter(applicationDirectories);
    }

    /// <inheritdoc/>
    public bool TryGetCurrentOpenFile(out IFilepath? filePath)
    {
        filePath = _currentFile;
        return _currentFile != null;
    }

    /// <inheritdoc/>
    public void SetCurrentOpenFile(IFilepath filePath)
    {
        _currentFile = filePath;
    }
}