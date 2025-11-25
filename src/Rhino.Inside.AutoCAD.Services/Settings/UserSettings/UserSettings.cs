using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IRhinoInsideAutoCadUserSettings"/>
public class UserSettings : IRhinoInsideAutoCadUserSettings
{
    /// <inheritdoc/>
    public bool DeploymentTesting { get; set; }
}