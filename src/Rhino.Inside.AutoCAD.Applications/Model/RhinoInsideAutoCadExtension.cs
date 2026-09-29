using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.State;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.Models;
using System.Globalization;
using System.Reflection;

[assembly: ExtensionApplication(typeof(RhinoInsideAutoCadExtension))]

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IRhinoInsideAutoCadApplication"/>
public class RhinoInsideAutoCadExtension : IExtensionApplication
{

    private const string _applicationLoadedSuccessMessage = ApplicationConstants.ApplicationLoadedSuccessMessage;
    private const string _applicationLoadErrorMessageFormat = ApplicationConstants.ApplicationLoadErrorMessageFormat;
    private const string _stackTraceMessageFormat = ApplicationConstants.StackTraceMessageFormat;
    private const string _expiredMessage = ApplicationConstants.ExpiredMessage;
    private const string _buildVersionMetadataPrefix = ApplicationConstants.BuildVersionMetadataPrefix;
    private const string _rhinoNotInstalledErrorMessage = ApplicationConstants.RhinoNotInstalledErrorMessage;
    private const string _rhinoVersionNotSelectedErrorMessage = ApplicationConstants.RhinoVersionNotSelectedErrorMessage;
    private const string _applicationLoadAbortedMessageFormat = ApplicationConstants.ApplicationLoadAbortedMessageFormat;
    private const string _rhinoUpdateRequiredMessageFormat = ApplicationConstants.RhinoUpdateRequiredMessageFormat;
    private const string _rhinoDownloadUrl = ApplicationConstants.RhinoDownloadUrl;

    private static readonly Version _minimumRhinoCommonVersion = ApplicationConstants.MinimumRhinoCommonVersion;

    /// <summary>
    /// The singleton instance of the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public static IRhinoInsideAutoCadApplication? Application { get; private set; }

    /// <summary>
    /// Indicates whether the application has expired.
    /// </summary>
    public static bool IsExpired { get; private set; }

    /// <summary>
    /// The reason the plugin did not finish loading, or null if it did.
    /// </summary>
    /// <remarks>
    /// The command methods stay registered with AutoCAD whether or not
    /// <see cref="Initialize"/> completed, so they report this rather than failing
    /// obscurely. Set when no Rhino version could be bound, which is the one case the
    /// plugin declines to load rather than running without Rhino.
    /// </remarks>
    public static string? LoadFailureMessage { get; private set; }

    /// <summary>
    /// Initialize the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public void Initialize()
    {

        var editor = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument?.Editor;

        var currentDate = System.DateTime.Now;

        try
        {

            var compliedDate = this.GetCompliedDate();

            var limitDate = compliedDate.AddDays(180);

            if (currentDate > limitDate)
            {
                IsExpired = true;
            }


            // Bootstrap before anything Rhino related. Nothing here references a RhinoCommon
            // type, and it gives the Rhino version selection dialog below the logger, the
            // WPF dispatcher and the Material Design assemblies it needs.
            var applicationConfig = new RhinoInsideAutoCadApplicationConfig();

            var bootstrapper = new Bootstrapper(new AutocadBootstrapperConfig(applicationConfig));

            // Bootstrapping initialises LoggerService, so anything RhinoCoreExtension
            // recorded before this point is still only in memory. Drain it now, otherwise
            // the earliest start-up failures never reach the log file.
            RhinoCoreExtension.Instance.StartUpLogger.Flush();

            // Decide which Rhino to run, then register the AssemblyResolve handlers which
            // load it. Both must happen before any code references a RhinoCommon type,
            // which the application constructor below does. The settings store must be the
            // shared instance, so the version chosen here is the one the settings page
            // later reads and writes.
            var installationLocator = new RhinoInstallationLocator();

            var userSettingsStore = UserSettingsStore.Instance;

            var versionDialogManager = new RhinoVersionDialogManager();

            var versionSelection = new RhinoVersionSelection(installationLocator,
                userSettingsStore, versionDialogManager);

            var installation = versionSelection.Resolve(out var resolution);

            if (installation is null)
            {
                this.AbortLoad(editor, resolution, versionSelection.OutdatedInstallations);

                return;
            }

            RhinoCoreExtension.BindTo(installation);

            Application = new RhinoInsideAutoCadApplication(bootstrapper, applicationConfig);

            Autodesk.AutoCAD.ApplicationServices.Core.Application.BeginQuit += this.OnApplicationBeginQuit;

            editor?.WriteMessage(_applicationLoadedSuccessMessage);
        }
        catch (System.Exception e)
        {
            var message = string.Format(_applicationLoadErrorMessageFormat, e.Message);

            editor?.WriteMessage(message);
            editor?.WriteMessage(string.Format(_stackTraceMessageFormat, e.StackTrace));
            editor?.WriteMessage(Assembly.GetExecutingAssembly().Location.ToString());

            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(message);

            throw;
        }
    }

    /// <summary>
    /// Stops the plugin loading because no Rhino version could be bound to this session.
    /// </summary>
    /// <remarks>
    /// Reported on the command line, and only in a dialog when Rhino must be updated: the
    /// other usual way to get here is the user cancelling the version selection, and
    /// answering a dialog with another dialog helps nobody. The command methods surface
    /// <see cref="LoadFailureMessage"/> if one is then used.
    /// </remarks>
    /// <param name="editor">The editor to write the reason to, if there is one.</param>
    /// <param name="resolution">Why no installation was resolved.</param>
    /// <param name="outdatedInstallations">
    /// The installations passed over for being too old, ordered newest first.
    /// </param>
    private void AbortLoad(Autodesk.AutoCAD.EditorInput.Editor? editor,
        RhinoVersionResolution resolution,
        IReadOnlyList<IRhinoInstallation> outdatedInstallations)
    {
        LoadFailureMessage = resolution switch
        {
            RhinoVersionResolution.UpdateRequired =>
                this.BuildUpdateRequiredMessage(outdatedInstallations),
            RhinoVersionResolution.Cancelled => _rhinoVersionNotSelectedErrorMessage,
            _ => _rhinoNotInstalledErrorMessage
        };

        editor?.WriteMessage(string.Format(_applicationLoadAbortedMessageFormat,
            LoadFailureMessage));

        if (resolution == RhinoVersionResolution.UpdateRequired)
        {
            var updateDialogManager = new RhinoUpdateDialogManager();

            updateDialogManager.Show(LoadFailureMessage, _rhinoDownloadUrl);
        }
    }

    /// <summary>
    /// Returns the message asking the user to update Rhino, naming the newest outdated
    /// RhinoCommon installed so they can see how far behind it is.
    /// </summary>
    /// <param name="outdatedInstallations">
    /// The installations passed over for being too old, ordered newest first.
    /// </param>
    private string BuildUpdateRequiredMessage(
        IReadOnlyList<IRhinoInstallation> outdatedInstallations)
    {
        var newestInstalledVersion = outdatedInstallations
            .Select(installation => installation.RhinoCommonVersion)
            .Max();

        return string.Format(_rhinoUpdateRequiredMessageFormat,
            _minimumRhinoCommonVersion, newestInstalledVersion);
    }

    /// <summary>
    /// Returns the date the assembly was compiled.
    /// </summary>
    private DateTime GetCompliedDate()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion != null)
        {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(_buildVersionMetadataPrefix);
            if (index > 0)
            {
                value = value.Substring(index + _buildVersionMetadataPrefix.Length);
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }
        }

        return default;
    }

    /// <summary>
    /// Safely terminates the <see cref="IRhinoInsideAutoCadApplication"/> by saving
    /// any edited rhino or grasshopper files and catching any exceptions that may
    /// occur during termination. This ensures  that the application can attempt to
    /// terminate without crashing, even if  there are issues during the termination
    /// process.
    /// </summary>
    private void SafeTermination()
    {
        try
        {
            Application?.Terminate();

            Application = null;

        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Shutdown error: {ex.Message}");
            try { LoggerService.Instance?.LogError(ex, "Termination error"); } catch { }
            Application = null;
        }
    }

    /// <summary>
    /// Subscribes to the AutoCAD application quit event to ensure that the
    /// <see cref="IRhinoInsideAutoCadApplication"/> is properly terminated.
    /// </summary>
    private void OnApplicationBeginQuit(object sender, Autodesk.AutoCAD.ApplicationServices.BeginQuitEventArgs e)
    {
        if (Application is not null)
        {
            e.IsVetoed = true;

            ApplicationState.BeginShutdown();

            Autodesk.AutoCAD.ApplicationServices.Core.Application.BeginQuit -= this.OnApplicationBeginQuit;

            this.SafeTermination();

            Autodesk.AutoCAD.ApplicationServices.Core.Application.Quit();
        }
    }

    /// <summary>
    /// Terminate the <see cref="IRhinoInsideAutoCadApplication"/>
    /// </summary>
    public void Terminate()
    {
        Autodesk.AutoCAD.ApplicationServices.Core.Application.BeginQuit -= this.OnApplicationBeginQuit;
    }
}
