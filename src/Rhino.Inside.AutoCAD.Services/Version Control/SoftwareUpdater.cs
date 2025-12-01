using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.IO.Compression;
using System.Text.Json;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A class responsible for updating the application when
/// a new release is deployed on the <see cref="IDeploymentDirectory"/>. Provides a
/// prompt to the user to update the application in the UI application.
/// </summary>
/// <remarks>
/// Singleton to provide domain level access to the <see cref="ISoftwareUpdater"/>.
/// The lifetime scope of the <see cref="ISoftwareUpdater"/> is the lifetime of the
/// AutoCAD application. The other purpose of singleton access is to prevent the user
/// from being repeatedly shown the dialog to update the app if they open any of the
/// Rhino.Inside.AutoCAD.Core applications in the same AutoCAD session. If they reject the update, the dialog
/// will not show until the next time AutoCAD is launched.
/// </remarks>
public class SoftwareUpdater : ISoftwareUpdater
{
    private string _packagePrefixName;

    private readonly IApplicationDirectories _applicationDirectories;
    private readonly IVersionLog _versionLog;
    private readonly IUpdaterConfigs _updaterConfigs;
    private readonly string _deploymentDirectory;

    /// <summary>
    /// Static singleton instance of the <see cref="ISoftwareUpdater"/>.
    /// </summary>
    public static ISoftwareUpdater? Instance { get; private set; }

    /// <inheritdoc/>
    public Version CurrentVersion { get; }

    /// <inheritdoc/>
    public Version LatestRelease { get; private set; }

    /// <inheritdoc/>
    public bool CanUpdate => this.LatestRelease > this.CurrentVersion;

    /// <inheritdoc/>
    public bool UpdateConfirmed { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="SoftwareUpdater"/>.
    /// </summary>
    private SoftwareUpdater(IBootstrapper bootstrapper, IApplicationConfig applicationConfig,
       ISettingsManager<IApplicationSettings, IUserSettings> settingsManager)
    {
        var applicationDirectories = bootstrapper.ApplicationDirectories;

        var versionLog = bootstrapper.VersionLog;

        _packagePrefixName = applicationConfig.PackagePrefixName;

        _applicationDirectories = applicationDirectories;

        _versionLog = versionLog;

        var settingsCore = settingsManager.Core;

        _updaterConfigs = settingsCore.UpdaterConfigs;

        _deploymentDirectory = settingsCore.DeploymentDirectory.GetDeploymentLocation(settingsManager.User);

        this.CurrentVersion = versionLog.CurrentVersion;

        this.LatestRelease = versionLog.CurrentVersion;
    }

    /// <summary>
    /// Private static constructor to ensure thread safety.
    /// </summary>
    static SoftwareUpdater()
    {

    }

    /// <summary>
    /// Returns the root install package directory with the versioned folder name.
    /// </summary>
    private string GetRootInstallPackageDirectory(Version version)
    {
        var packageDirectory = $"{_applicationDirectories.RootInstall}{version}\\";

        return packageDirectory;
    }

    /// <summary>
    /// Deletes the version previous to current to clean up the root install folder.
    /// </summary>
    private void CleanupOldVersion()
    {
        var previousVersionNumber = _versionLog.PreviousVersion;

        var previousVersionDirectory = this.GetRootInstallPackageDirectory(previousVersionNumber);

        if (Directory.Exists(previousVersionDirectory))
            Directory.Delete(previousVersionDirectory, true);
    }

    /// <summary>
    /// Copies all files recursively form the source directory to the destination directory
    /// iterating through all subfolders.
    /// </summary>
    private void CopyFiles(string sourceDirectory, string destinationDirectory)
    {
        var directoryInfo = new DirectoryInfo(sourceDirectory);

        var subDirectories = directoryInfo.GetDirectories();

        var files = directoryInfo.GetFiles();

        foreach (var file in files)
        {
            var packagePath = Path.Combine(destinationDirectory, file.Name);

            file.CopyTo(packagePath, true);
        }

        foreach (var subDirectory in subDirectories)
        {
            var subPackagePath = Path.Combine(destinationDirectory, subDirectory.Name);

            Directory.CreateDirectory(subPackagePath);

            this.CopyFiles(subDirectory.FullName, subPackagePath);
        }
    }

    /// <summary>
    /// Extracts all application files and resources from the <see cref="_deploymentDirectory"/>
    /// to a new versioned folder in the <see cref="IApplicationDirectories.RootInstall"/> location.
    /// The files are unzipped to a temporary folder to enable the contents to overwrite the
    /// files in the root install location, otherwise the deployment will fail.
    /// </summary>
    private void ExtractPackageFiles()
    {
        var releasePackagePath = $"{_deploymentDirectory}{_packagePrefixName}{this.LatestRelease}.zip";

        var tempDeploymentLocation = $"{_applicationDirectories.UserLocal}{Guid.NewGuid()}\\";

        var rootInstallDirectory = _applicationDirectories.RootInstall;

        if (File.Exists(releasePackagePath) == false)
        {
            LoggerService.Instance.LogMessage($"Release package does not exist at {releasePackagePath}");

            return;
        }

        ZipFile.ExtractToDirectory(releasePackagePath, tempDeploymentLocation);

        this.CopyFiles(tempDeploymentLocation, rootInstallDirectory);

        Directory.Delete(tempDeploymentLocation, true);
    }

    /// <summary>
    /// Initializes the <see cref="ISoftwareUpdater"/> singleton instance.
    /// </summary>
    public static void Initialize(IBootstrapper bootstrapper, IApplicationConfig applicationConfig, ISettingsManager<IApplicationSettings, IUserSettings> settingsManager)
    {
        if (Instance != null) return;

        var softwareUpdater = new SoftwareUpdater(bootstrapper, applicationConfig, settingsManager);

        Instance = softwareUpdater;

        softwareUpdater.CheckForUpdate();
    }

    /// <inheritdoc/>
    public void ConfirmUpdate()
    {
        this.UpdateConfirmed = true;
    }

    /// <inheritdoc/>
    public void CheckForUpdate()
    {
        var releasesFilePath = $"{_deploymentDirectory}{_updaterConfigs.ReleasesFileName}";

        if (File.Exists(releasesFilePath) == false)
            return;

        var serializerOptions = new JsonSerializerOptions
        {
            Converters = {
                new InterfaceConverterFactory(typeof(Releases), typeof(IReleases))
            }
        };

        using var jsonFileStream = File.OpenRead(releasesFilePath);

        var releases = JsonSerializer.Deserialize<IReleases>(jsonFileStream, serializerOptions);

        var latestRelease = releases!.GetLatestRelease();

        this.LatestRelease = latestRelease;
    }

    /// <inheritdoc/>
    public void Update()
    {
        if (this.CanUpdate && this.UpdateConfirmed)
        {
            this.ExtractPackageFiles();

            this.CleanupOldVersion();
        }
    }
}
