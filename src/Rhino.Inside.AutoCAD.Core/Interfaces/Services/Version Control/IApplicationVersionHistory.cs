namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Tracks current and previous application versions for upgrade detection.
/// </summary>
/// <remarks>
/// Used to determine whether the application has been upgraded since the last run.
/// Version comparison enables migration logic, change log display, and settings updates
/// when users install a new version of Rhino.Inside.AutoCAD.
/// </remarks>
public interface IApplicationVersionHistory
{
    /// <summary>
    /// Gets the current application version using semantic versioning.
    /// </summary>
    /// <remarks>
    /// Represents the highest version number found in the installation directory.
    /// Returns an empty <see cref="Version"/> if no valid version folders exist.
    /// </remarks>
    Version GetCurrentVersion();

    /// <summary>
    /// Gets the application version using semantic versioning via the index where
    /// 0 is the current version and 1 would be the previous version ect.
    /// </summary>
    Version GetVersion(int index);
}