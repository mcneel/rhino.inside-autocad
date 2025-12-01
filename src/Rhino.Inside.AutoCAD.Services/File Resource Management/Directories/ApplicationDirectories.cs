using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IApplicationDirectories"/>
public class ApplicationDirectories : IApplicationDirectories
{
    private const string _userTestFolderName = CoreConstants.UserTestFolderName;
    private const string _supportedApplicationsFolderName = CoreConstants.SupportedApplicationFolderName;
    private const string _usageMetricsFolderName = CoreConstants.UsageMetricsFolderName;
    private const string _assemblyFolder = CoreConstants.AssemblyFolderName;
    private const string _resourcesFolderName = CoreConstants.ResourcesFolderName;
    private const string _bimorphAppDataFolderName = CoreConstants.BimorphAppDataFolderName;

    /// <inheritdoc />
    public string RootInstall { get; }

    /// <inheritdoc />
    public string Resources { get; }

    /// <inheritdoc />
    public string Assemblies { get; }

    /// <inheritdoc />
    public string UserLocal { get; }

    /// <inheritdoc />
    public string UserTests { get; }

    /// <inheritdoc />
    public string SupportedApplication { get; }

    /// <inheritdoc />
    public string UsageMetrics { get; }

    /// <inheritdoc />
    public string BimorphAppData { get; }

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

        var bimorphAppData = $"{appData}\\{_bimorphAppDataFolderName}\\";

        var userTests = $"{bimorphAppData}{_userTestFolderName}\\";

        var supportedApplications = $"{bimorphAppData}{_supportedApplicationsFolderName}\\";

        var usageMetrics = $"{bimorphAppData}{_usageMetricsFolderName}\\";

        if (Directory.Exists(userLocal) == false)
            Directory.CreateDirectory(userLocal);

        this.RootInstall = rootInstallDirectory;

        this.Resources = appResourcesDirectory;

        this.Assemblies = $"{rootInstallDirectory}{currentVersion}\\{_assemblyFolder}\\";

        this.UserLocal = userLocal;

        this.BimorphAppData = bimorphAppData;

        this.UserTests = userTests;

        this.SupportedApplication = supportedApplications;

        this.UsageMetrics = usageMetrics;

        this.ApplicationName = applicationName;

        this.ProductName = applicationConfig.ProductName;
    }
}