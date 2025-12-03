using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IUserSettings"/>
public class UserSettings : IUserSettings
{
    /// <inheritdoc/>
    public bool DeploymentTesting { get; set; }
}