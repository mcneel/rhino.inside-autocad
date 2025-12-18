namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A interface defining core application-wide settings.
/// </summary>
public interface ISettingsCore
{
    /// <summary>
    /// The <see cref="IDeploymentDirectory"/>.
    /// </summary>
    IDeploymentDirectory DeploymentDirectory { get; }

    /// <summary>
    /// The <see cref="ILoadingScreenConstants"/>.
    /// </summary>
    ILoadingScreenConstants LoadingScreenConstants { get; }
}