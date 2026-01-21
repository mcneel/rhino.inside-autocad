using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rhino.Inside.AutoCAD.UI.Resources.Models;
using System.Diagnostics;
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
