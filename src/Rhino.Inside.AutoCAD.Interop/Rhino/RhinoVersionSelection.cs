using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.Models;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoVersionSelection"/>
/// <remarks>
/// Must be resolved before <see cref="RhinoCoreExtension.BindTo"/>, and therefore before any
/// Rhino type is touched, so nothing here may reference RhinoCommon. It is deliberately an
/// ordinary object created on the startup path rather than something driven by a type
/// initializer: it shows a dialog, and doing that while the CLR holds a class initialization
/// lock is a good way to deadlock.
/// </remarks>
/// <seealso cref="RhinoCoreExtension"/>
public class RhinoVersionSelection : IRhinoVersionSelection
{
    private const string _rhinoInstallationsFoundFormat = MessageConstants.RhinoInstallationsFoundFormat;
    private const string _rhinoInstallationDescriptionFormat = MessageConstants.RhinoInstallationDescriptionFormat;
    private const string _noRhinoInstallationsFound = MessageConstants.NoRhinoInstallationsFound;
    private const string _rhinoOutdatedInstallationsSkippedFormat = MessageConstants.RhinoOutdatedInstallationsSkippedFormat;

    private static readonly Version _minimumRhinoCommonVersion =
        ApplicationConstants.MinimumRhinoCommonVersion;

    private readonly IRhinoInstallationLocator _installationLocator;

    private readonly IUserSettingsStore _userSettingsStore;

    private readonly IRhinoVersionDialogManager _dialogManager;

    /// <inheritdoc />
    public IReadOnlyList<IRhinoInstallation> OutdatedInstallations { get; private set; } = [];

    /// <summary>
    /// Constructs a new <see cref="RhinoVersionSelection"/>.
    /// </summary>
    /// <param name="installationLocator">
    /// The locator which discovers the Rhino installations to choose between.
    /// </param>
    /// <param name="userSettingsStore">
    /// The store holding the version the user previously settled on. Must be the shared
    /// store, so that a change made later on the settings page is seen here.
    /// </param>
    /// <param name="dialogManager">
    /// The dialog which asks the user to choose, used only when there is a choice to make.
    /// </param>
    public RhinoVersionSelection(IRhinoInstallationLocator installationLocator,
        IUserSettingsStore userSettingsStore,
        IRhinoVersionDialogManager dialogManager)
    {
        _installationLocator = installationLocator;
        _userSettingsStore = userSettingsStore;
        _dialogManager = dialogManager;
    }

    /// <summary>
    /// Describes the located installations for the log, which is the first place to look
    /// when a version the user expects to be offered is missing.
    /// </summary>
    /// <param name="installations">The located installations.</param>
    private string DescribeInstallations(IReadOnlyList<IRhinoInstallation> installations)
    {
        if (installations.Count == 0)
            return _noRhinoInstallationsFound;

        return string.Join(", ", installations.Select(installation =>
            string.Format(_rhinoInstallationDescriptionFormat,
                installation.DisplayName,
                installation.VersionKey,
                installation.RhinoCommonVersion,
                installation.RhinoCommonPath)));
    }

    /// <summary>
    /// Returns the installation with the given version key, or null when there is none.
    /// </summary>
    /// <param name="installations">The installations to search.</param>
    /// <param name="versionKey">The version key to match, which may be null.</param>
    private IRhinoInstallation? FindByVersionKey(
        IReadOnlyList<IRhinoInstallation> installations,
        string? versionKey)
    {
        if (string.IsNullOrWhiteSpace(versionKey))
            return null;

        return installations.FirstOrDefault(installation =>
            string.Equals(installation.VersionKey, versionKey, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public IRhinoInstallation? Resolve(out RhinoVersionResolution resolution)
    {
        var logger = LoggerService.Instance;

        var located = _installationLocator.Locate();

        logger.LogMessage(string.Format(_rhinoInstallationsFoundFormat,
            this.DescribeInstallations(located)));

        // An outdated install is left out entirely rather than offered with a warning, so
        // the user can never bind to a Rhino this build does not support.
        var installations = located
            .Where(installation => !installation.IsOutdated)
            .ToList();

        this.OutdatedInstallations = located
            .Where(installation => installation.IsOutdated)
            .ToList();

        if (this.OutdatedInstallations.Count > 0)
            logger.LogMessage(string.Format(_rhinoOutdatedInstallationsSkippedFormat,
                this.DescribeInstallations(this.OutdatedInstallations),
                _minimumRhinoCommonVersion));

        if (installations.Count == 0)
        {
            resolution = this.OutdatedInstallations.Count > 0
                ? RhinoVersionResolution.UpdateRequired
                : RhinoVersionResolution.NotInstalled;

            return null;
        }

        resolution = RhinoVersionResolution.Resolved;

        if (installations.Count == 1)
            return installations[0];

        var settings = _userSettingsStore.Settings;

        var preferred = this.FindByVersionKey(installations, settings.PreferredRhinoVersion);

        // A saved "always" choice is only honoured while that version is still installed;
        // if it has been uninstalled the user is asked again rather than silently moved.
        if (settings.AlwaysUsePreferredRhinoVersion && preferred != null)
            return preferred;

        var result = _dialogManager.Show(installations, preferred);

        if (result.Choice == RhinoVersionChoice.Cancel || result.Installation == null)
        {
            resolution = RhinoVersionResolution.Cancelled;

            return null;
        }

        settings.PreferredRhinoVersion = result.Installation.VersionKey;

        settings.AlwaysUsePreferredRhinoVersion =
            result.Choice == RhinoVersionChoice.AlwaysUse;

        _userSettingsStore.Save();

        return result.Installation;
    }
}
