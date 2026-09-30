using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace Rhino.Inside.AutoCAD.UI.Resources.ViewModels;

/// <summary>
/// The view model for the Support Dialog.
/// </summary>
public partial class SupportDialogViewModel : ObservableObject
{
    private const string _documentationUrl = UIConstants.DocumentationUrl;
    private const string _forumUrl = UIConstants.ForumUrl;
    private const string _bimorphUrl = UIConstants.BimorphUrl;
    private const string _notDetermined = UIConstants.NotDetermined;
    private const string _openForVersion = UIConstants.OpenForVersion;
    private const string _rhinoVersionRestartNote = UIConstants.RhinoVersionRestartNote;
    private const string _previewColorsHeading = UIConstants.PreviewColorsHeading;
    private const string _previewColorsDescription = UIConstants.PreviewColorsDescription;
    private const string _rhinoPreviewColorLabel = UIConstants.RhinoPreviewColorLabel;
    private const string _grasshopperPreviewColorLabel = UIConstants.GrasshopperPreviewColorLabel;
    private const string _selectedPreviewColorLabel = UIConstants.SelectedPreviewColorLabel;
    private const string _previewEntityLimitLabel = UIConstants.PreviewEntityLimitLabel;
    private const string _previewEntityLimitToolTip = UIConstants.PreviewEntityLimitToolTip;
    private const int _minPreviewEntityLimit = ApplicationConstants.MinPreviewEntityLimit;
    private const int _maxPreviewEntityLimit = ApplicationConstants.MaxPreviewEntityLimit;

    /// <summary>
    /// The <see cref="Visibility"/> of the buttons in the dialog.
    /// </summary>
    [ObservableProperty]
    private Visibility _buttonVisibility = Visibility.Visible;

    /// <summary>
    /// The current AutoCAD version.
    /// </summary>
    [ObservableProperty]
    private string _autocadVersion = string.Empty;

    /// <summary>
    /// The current Rhino version.
    /// </summary>
    [ObservableProperty]
    private string _rhinoVersion = string.Empty;

    /// <summary>
    /// The current Grasshopper version.
    /// </summary>
    [ObservableProperty]
    private string _grasshopperVersion = string.Empty;

    /// <summary>
    /// The current Rhino.Inside.AutoCAD version.
    /// </summary>
    [ObservableProperty]
    private string _rhinoInsideAutocadVersion = string.Empty;

    /// <summary>
    /// The .NET runtime hosting this process, for example ".NET 8.0.11" or
    /// ".NET Framework 4.8.9300.0".
    /// </summary>
    /// <remarks>
    /// Read from the process rather than passed in with the other versions: it is a
    /// property of the host AutoCAD, not of anything this plugin loads. AutoCAD 2025 and
    /// 2026 run on either .NET 8 or .NET 10 depending on how far the user has updated,
    /// and the build alone does not tell them apart, so it is worth showing.
    /// </remarks>
    public string DotNetVersion { get; } =
        string.IsNullOrWhiteSpace(RuntimeInformation.FrameworkDescription)
            ? _notDetermined
            : RuntimeInformation.FrameworkDescription;

    /// <summary>
    /// Indicates whether AutoCAD is up to date.
    /// </summary>
    [ObservableProperty]
    private bool _autocadIsUpToDate = true;

    /// <summary>
    /// Indicates whether AutoCAD is up to date.
    /// </summary>
    [ObservableProperty]
    private bool _rhinoIsUpToDate = true;

    /// <summary>
    /// Indicates whether AutoCAD is up to date.
    /// </summary>
    [ObservableProperty]
    private bool _grasshopperIsUpToDate = true;

    /// <summary>
    /// Indicates whether AutoCAD is up to date.
    /// </summary>
    [ObservableProperty]
    private bool _rhinoInsideAutocadIsUpToDate = true;

    /// <summary>
    /// The currently selected tab index.
    /// </summary>
    [ObservableProperty]
    private int _selectedTabIndex;

    private readonly IUserSettingsStore _userSettingsStore;

    private readonly IRhinoInsideManager _rhinoInsideManager;

    /// <summary>
    /// True while the settings are being read into this view model, during which changes
    /// must not be written back.
    /// </summary>
    private readonly bool _isLoadingSettings;

    /// <summary>
    /// The Rhino installations found on this machine which the plugin can bind to.
    /// </summary>
    public ObservableCollection<IRhinoInstallation> RhinoInstallations { get; } = [];

    /// <summary>
    /// The note explaining that a change of Rhino version only takes effect on the next
    /// AutoCAD session.
    /// </summary>
    public string RhinoVersionRestartNote => _rhinoVersionRestartNote;

    /// <summary>
    /// The Rhino version the plugin will bind to, saved as soon as it is changed.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanAlwaysUseRhinoVersion))]
    private IRhinoInstallation? _selectedRhinoInstallation;

    /// <summary>
    /// True when <see cref="SelectedRhinoInstallation"/> is used without prompting,
    /// otherwise false to be asked at every AutoCAD start.
    /// </summary>
    /// <remarks>
    /// Clearing this is how a user who chose "Always use" gets the startup dialog back.
    /// </remarks>
    [ObservableProperty]
    private bool _alwaysUseSelectedRhinoVersion;

    /// <summary>
    /// True when more than one Rhino version is installed, which is the only case in which
    /// the startup dialog appears and so the only case in which the "always use" choice
    /// means anything.
    /// </summary>
    public bool HasRhinoVersionChoice => this.RhinoInstallations.Count > 1;

    /// <summary>
    /// True when the user can stop being asked at startup, which needs both a choice worth
    /// making and a version chosen to settle on.
    /// </summary>
    public bool CanAlwaysUseRhinoVersion =>
        this.HasRhinoVersionChoice && this.SelectedRhinoInstallation != null;

    /// <summary>
    /// The heading of the preview colors section.
    /// </summary>
    public string PreviewColorsHeading => _previewColorsHeading;

    /// <summary>
    /// The description of the preview colors section.
    /// </summary>
    public string PreviewColorsDescription => _previewColorsDescription;

    /// <summary>
    /// The label of the Rhino preview color.
    /// </summary>
    public string RhinoPreviewColorLabel => _rhinoPreviewColorLabel;

    /// <summary>
    /// The label of the Grasshopper preview color.
    /// </summary>
    public string GrasshopperPreviewColorLabel => _grasshopperPreviewColorLabel;

    /// <summary>
    /// The label of the selected preview color.
    /// </summary>
    public string SelectedPreviewColorLabel => _selectedPreviewColorLabel;

    /// <summary>
    /// The label of the preview entity limit.
    /// </summary>
    public string PreviewEntityLimitLabel => _previewEntityLimitLabel;

    /// <summary>
    /// The tooltip explaining the preview entity limit.
    /// </summary>
    public string PreviewEntityLimitToolTip => _previewEntityLimitToolTip;

    /// <summary>
    /// The AutoCAD Color Index unselected Rhino previews are drawn in, saved and applied as
    /// soon as it is changed.
    /// </summary>
    [ObservableProperty]
    private int _rhinoPreviewColorIndex;

    /// <summary>
    /// The AutoCAD Color Index unselected Grasshopper previews are drawn in, saved and
    /// applied as soon as it is changed.
    /// </summary>
    [ObservableProperty]
    private int _grasshopperPreviewColorIndex;

    /// <summary>
    /// The AutoCAD Color Index selected previews of either kind are drawn in, saved and
    /// applied as soon as it is changed.
    /// </summary>
    [ObservableProperty]
    private int _selectedPreviewColorIndex;

    /// <summary>
    /// The most entities each preview server draws, saved and applied as soon as it is
    /// changed.
    /// </summary>
    [ObservableProperty]
    private int _maxPreviewEntityCount;

    /// <summary>
    /// Constructs a new <see cref="SupportDialogViewModel"/>.
    /// </summary>
    /// <param name="userSettingsStore">The store to read and write user settings through.</param>
    /// <param name="rhinoInsideManager">The manager the preview colors are applied through.</param>
    /// <param name="rhinoInstallations">The Rhino installations found on this machine.</param>
    public SupportDialogViewModel(
        IUserSettingsStore userSettingsStore,
        IRhinoInsideManager rhinoInsideManager,
        IReadOnlyList<IRhinoInstallation> rhinoInstallations)
    {
        _isLoadingSettings = true;

        _userSettingsStore = userSettingsStore;

        _rhinoInsideManager = rhinoInsideManager;

        foreach (var installation in rhinoInstallations)
            this.RhinoInstallations.Add(installation);

        var settings = userSettingsStore.Settings;

        _selectedRhinoInstallation = rhinoInstallations.FirstOrDefault(installation =>
            string.Equals(installation.VersionKey, settings.PreferredRhinoVersion,
                StringComparison.OrdinalIgnoreCase));

        _alwaysUseSelectedRhinoVersion = settings.AlwaysUsePreferredRhinoVersion;

        _rhinoPreviewColorIndex = settings.RhinoPreviewColorIndex;

        _grasshopperPreviewColorIndex = settings.GrasshopperPreviewColorIndex;

        _selectedPreviewColorIndex = settings.SelectedPreviewColorIndex;

        _maxPreviewEntityCount = settings.MaxPreviewEntityCount;

        _isLoadingSettings = false;
    }

    /// <summary>
    /// Persists the newly chosen Rhino version.
    /// </summary>
    partial void OnSelectedRhinoInstallationChanged(IRhinoInstallation? value)
    {
        if (value == null)
            return;

        this.SaveRhinoVersionSettings();
    }

    /// <summary>
    /// Persists the newly chosen prompting behaviour.
    /// </summary>
    partial void OnAlwaysUseSelectedRhinoVersionChanged(bool value)
    {
        this.SaveRhinoVersionSettings();
    }

    /// <summary>
    /// Persists and applies the newly chosen Rhino preview color.
    /// </summary>
    partial void OnRhinoPreviewColorIndexChanged(int value)
    {
        this.SavePreviewColorSettings();
    }

    /// <summary>
    /// Persists and applies the newly chosen Grasshopper preview color.
    /// </summary>
    partial void OnGrasshopperPreviewColorIndexChanged(int value)
    {
        this.SavePreviewColorSettings();
    }

    /// <summary>
    /// Persists and applies the newly chosen selected preview color.
    /// </summary>
    partial void OnSelectedPreviewColorIndexChanged(int value)
    {
        this.SavePreviewColorSettings();
    }

    /// <summary>
    /// Persists and applies the newly chosen preview entity limit, clamped to the range the
    /// user may choose from.
    /// </summary>
    partial void OnMaxPreviewEntityCountChanged(int value)
    {
        var clampedValue = Math.Max(_minPreviewEntityLimit, Math.Min(value, _maxPreviewEntityLimit));

        // Setting the clamped value raises this handler again, which does the saving.
        if (clampedValue != value)
        {
            this.MaxPreviewEntityCount = clampedValue;
            return;
        }

        this.SavePreviewEntityLimitSettings();
    }

    /// <summary>
    /// Writes the preview entity limit back to disk and applies it to the previews.
    /// </summary>
    private void SavePreviewEntityLimitSettings()
    {
        if (_isLoadingSettings)
            return;

        var settings = _userSettingsStore.Settings;

        settings.MaxPreviewEntityCount = this.MaxPreviewEntityCount;

        _userSettingsStore.Save();

        // The choice is already saved, so a failure to apply it now costs the user nothing
        // more than waiting for the next AutoCAD session to see it take effect.
        try
        {
            _rhinoInsideManager.UpdatePreviewEntityLimit(this.MaxPreviewEntityCount);
        }
        catch (Exception e)
        {
            LoggerService.Instance.LogError(e);
        }
    }

    /// <summary>
    /// Writes the preview colors back to disk and redraws the previews in them.
    /// </summary>
    private void SavePreviewColorSettings()
    {
        if (_isLoadingSettings)
            return;

        var settings = _userSettingsStore.Settings;

        settings.RhinoPreviewColorIndex = this.RhinoPreviewColorIndex;

        settings.GrasshopperPreviewColorIndex = this.GrasshopperPreviewColorIndex;

        settings.SelectedPreviewColorIndex = this.SelectedPreviewColorIndex;

        _userSettingsStore.Save();

        // The choice is already saved, so a failure to redraw the previews now costs the user
        // nothing more than waiting for the next AutoCAD session to see the new colors.
        try
        {
            _rhinoInsideManager.UpdatePreviewColors(this.RhinoPreviewColorIndex,
                this.GrasshopperPreviewColorIndex, this.SelectedPreviewColorIndex);
        }
        catch (Exception e)
        {
            LoggerService.Instance.LogError(e);
        }
    }

    /// <summary>
    /// Writes the Rhino version settings back to disk.
    /// </summary>
    private void SaveRhinoVersionSettings()
    {
        if (_isLoadingSettings)
            return;

        var settings = _userSettingsStore.Settings;

        // Left as it was when nothing is selected, so clearing the selection cannot silently
        // discard a saved preference.
        if (this.SelectedRhinoInstallation != null)
            settings.PreferredRhinoVersion = this.SelectedRhinoInstallation.VersionKey;

        settings.AlwaysUsePreferredRhinoVersion =
            this.AlwaysUseSelectedRhinoVersion && settings.PreferredRhinoVersion != null;

        _userSettingsStore.Save();
    }

    /// <summary>
    /// Updates the version information displayed in the dialog.
    /// </summary>
    /// <param name="autocadVersion">The AutoCAD version string.</param>
    /// <param name="rhinoVersion">The Rhino version string.</param>
    /// <param name="grasshopperVersion">The Grasshopper version string.</param>
    /// <param name="rhinoInsideVersion">The Rhino.Inside.AutoCAD version string.</param>
    public void UpdateVersionInfo(
        Version? autocadVersion,
        Version? rhinoVersion,
        Version? grasshopperVersion,
        Version? rhinoInsideVersion)
    {
        this.AutocadVersion = autocadVersion?.ToString() ?? _notDetermined;
        this.RhinoVersion = rhinoVersion?.ToString() ?? _openForVersion;
        this.GrasshopperVersion = grasshopperVersion?.ToString() ?? _openForVersion;
        this.RhinoInsideAutocadVersion = rhinoInsideVersion?.ToString() ?? _notDetermined;
    }

    /// <summary>
    /// Opens the documentation website.
    /// </summary>
    [RelayCommand]
    private void OpenDocumentation()
    {
        OpenUrl(_documentationUrl);
    }

    /// <summary>
    /// Opens the McNeel forum.
    /// </summary>
    [RelayCommand]
    private void OpenForum()
    {
        OpenUrl(_forumUrl);
    }

    /// <summary>
    /// Opens the Bimorph website.
    /// </summary>
    [RelayCommand]
    private void OpenBimorph()
    {
        OpenUrl(_bimorphUrl);
    }

    /// <summary>
    /// Triggers the update process for Rhino.Inside.AutoCAD.
    /// </summary>
    [RelayCommand]
    private void UpdateRhinoInside()
    {
        Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Automatic Update is not Implemented yet");

    }

    /// <summary>
    /// Opens a URL in the default browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    private static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
