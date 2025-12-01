namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents the deployment directory for the application.
/// </summary>
public interface IDeploymentDirectory
{
    /// <summary>
    /// The deployment directory for company-wide distribution.
    /// </summary>
    string Release { get; }

    /// <summary>
    /// The deployment directory for client-side application testing.
    /// </summary>
    string Debug { get; }

    /// <summary>
    /// Returns the deployment directory based on the
    /// <see cref="IUserSettings.DeploymentTesting"/> flag.
    /// </summary>
    string GetDeploymentLocation(IUserSettings userSettings);
}