using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Runtime.InteropServices;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IApplicationDirectories"/>
public class ApplicationDirectories : IApplicationDirectories
{
    private const string _assemblyFolder = ApplicationConstants.AssemblyFolderName;
    private const string _resourcesFolderName = ApplicationConstants.ResourcesFolderName;
    private const string _netFrameworkFilter = ApplicationConstants.NetFrameworkFilter;
    private const string _net48FolderName = ApplicationConstants.Net48FolderName;
    private const string _net8FolderName = ApplicationConstants.Net8FolderName;

    /// <inheritdoc />
    public string RootInstall { get; }

    /// <inheritdoc />
    public string Resources { get; }

    /// <inheritdoc />
    public string Assemblies { get; }

    /// <inheritdoc />
    public string UserLocal { get; }

    /// <inheritdoc />
    public string ApplicationName { get; }

    /// <inheritdoc />
    public string ProductName { get; }

    /// <summary>
    /// Constructs a new <see cref="ApplicationDirectories"/>.
    /// </summary>
    public ApplicationDirectories(IVersionLog versionLog, IApplicationConfig applicationConfig)
    {
        var rootInstallDirectory = versionLog.RootInstallDirectory;

        var currentVersion = versionLog.CurrentVersion;

        var applicationName = applicationConfig.ApplicationName;

        var appResourcesDirectory = $"{rootInstallDirectory}{_resourcesFolderName}\\";

        var appData =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var userLocal = $"{appData}\\{applicationConfig.ClientFolderName}\\";

        if (Directory.Exists(userLocal) == false)
            Directory.CreateDirectory(userLocal);

        var frameworkFolder = this.GetFrameworkFolder();

        this.RootInstall = rootInstallDirectory;

        this.Resources = appResourcesDirectory;

        this.Assemblies = $"{rootInstallDirectory}{currentVersion}\\{_assemblyFolder}\\{frameworkFolder}\\";

        this.UserLocal = userLocal;

        this.ApplicationName = applicationName;

        this.ProductName = applicationConfig.ProductName;
    }

    /// <summary>
    /// Returns the framework folder name based on the current runtime framework.
    /// In case of .NET Framework 4.8, returns "NET48", otherwise "NET8".
    /// </summary>
    public string GetFrameworkFolder()
    {
        var description = RuntimeInformation.FrameworkDescription;

        return description.StartsWith(_netFrameworkFilter, StringComparison.OrdinalIgnoreCase)
            ? _net48FolderName
            : _net8FolderName;
    }
}