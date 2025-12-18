namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface providing essential file resource directories and file names
/// for obtaining external resources.
/// </summary>
public interface IFileResourceManager
{
    /// <summary>
    /// The <see cref="IApplicationDirectories"/>.
    /// </summary>
    IApplicationDirectories ApplicationDirectories { get; }

    /// <summary>
    /// The <see cref="IFilepath"/> of the currently open file
    /// </summary>
    bool TryGetCurrentOpenFile(out IFilepath? filePath);

    /// <summary>
    /// Set current <see cref="IFilepath"/> to the provided <paramref name="filePath"/>.
    /// </summary>
    void SetCurrentOpenFile(IFilepath filePath);
}
