namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the user-specific settings for the specific application. 
/// </summary>
public interface IUserSettings
{
    /// <summary>
    /// True if the application is set to testing mode. Changes the lookup for the
    /// deployment location to the <see cref="IDeploymentDirectory.Debug"/> directory,
    /// otherwise the <see cref="IDeploymentDirectory.Release"/> directory is used.
    /// </summary>
    bool DeploymentTesting { get; }
}
