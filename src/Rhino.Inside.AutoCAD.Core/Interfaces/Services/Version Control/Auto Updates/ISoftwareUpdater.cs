namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining a method for updating the application
/// when a new release is deployed on the
/// <see cref="IDeploymentDirectory"/>.
/// </summary>
public interface ISoftwareUpdater
{
    /// <summary>
    /// The currently installed <see cref="Version"/> of the software.
    /// </summary>
    Version CurrentVersion { get; }

    /// <summary>
    /// The latest release <see cref="Version"/>.
    /// </summary>
    Version LatestRelease { get; }

    /// <summary>
    /// True if this <see cref="ISoftwareUpdater"/> has found a newer
    /// version of the software and can perform the update.
    /// </summary>
    bool CanUpdate { get; }

    /// <summary>
    /// True if the user has confirmed the application should update if
    /// a new version is available, otherwise false.
    /// </summary>
    bool UpdateConfirmed { get; }

    /// <summary>
    /// Checks for updated version of the application.
    /// </summary>
    void CheckForUpdate();

    /// <summary>
    /// Updates the application to the latest version.
    /// </summary>
    void Update();

    /// <summary>
    /// Called to confirm the update if an update is available.
    /// </summary>
    void ConfirmUpdate();
}