using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class containing core application-wide settings.
/// </summary>
public class SettingsCore : ISettingsCore
{
    /// <inheritdoc/>
    public IDeploymentDirectory DeploymentDirectory { get; set; }

    /// <inheritdoc/>
    public ISplashScreenConstants SplashScreenConstants { get; set; }

    /// <inheritdoc/>
    public IUpdaterConfigs UpdaterConfigs { get; set; }
}