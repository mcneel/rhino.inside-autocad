using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// Configurations for the <see cref="IBimorphApplication"/>.
/// </summary>
public class RhinoInsideAutoCadApplicationConfig : IApplicationConfig
{
    private const string _rootInstallFolderName = ApplicationConstants.RootInstallFolderName;

    /// <inheritdoc/>
    public string ApplicationName => ApplicationConstants.ApplicationName;

    /// <inheritdoc/>
    public string ProductName => ApplicationConstants.ProductName;

    /// <inheritdoc/>
    public string ClientFolderName => ApplicationConstants.ClientFolderName;

    /// <inheritdoc/>
    public string PackagePrefixName => ApplicationConstants.PackagePrefixName;

    /// <inheritdoc/>
    public string RootInstallDirectory { get; }

    /// <summary>
    /// Constructs a new <see cref="RhinoInsideAutoCadApplicationConfig"/>
    /// </summary>
    public RhinoInsideAutoCadApplicationConfig()
    {
        var userRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var rootInstallDirectory = $"{userRoaming}\\{_rootInstallFolderName}\\";

        this.RootInstallDirectory = rootInstallDirectory;
    }
}