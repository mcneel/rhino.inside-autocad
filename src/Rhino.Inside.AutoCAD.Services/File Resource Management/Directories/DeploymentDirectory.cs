using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class which represents the deployment directory for the application.
/// </summary>
public class DeploymentDirectory : IDeploymentDirectory
{
    /// <summary>
    /// The deployment directory for company-wide distribution.
    /// </summary>
    public string Release { get; }

    /// <summary>
    /// The deployment directory for client-side application testing.
    /// </summary>
    public string Debug { get; }
    
    /// <summary>
    /// Constructs a new <see cref="DeploymentDirectory"/>.
    /// </summary>
    public DeploymentDirectory(string release, string debug)
    {
        this.Release = release;

        this.Debug = debug;
    }

    /// <summary>
    /// Returns the deployment directory based on the
    /// <see cref="IUserSettings.DeploymentTesting"/> flag.
    /// </summary>
    public string GetDeploymentLocation(IUserSettings userSettings)
    {
        return userSettings.DeploymentTesting ? this.Debug : this.Release;
    }
}