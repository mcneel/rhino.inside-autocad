using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class providing essential file resource directories and file names
/// for obtaining external resources.
/// </summary>
public class FileResourceManager : IFileResourceManager
{
    private IFilepath? _currentFile = null;

    /// <inheritdoc/>
    public IApplicationDirectories ApplicationDirectories { get; }

    /// <summary>
    /// Constructs a new <see cref="IFileResourceManager"/>
    /// </summary>
    public FileResourceManager(IApplicationDirectories applicationDirectories, ISettingsManager settingsManager)
    {
        this.ApplicationDirectories = applicationDirectories;
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