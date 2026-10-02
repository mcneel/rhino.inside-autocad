using CommunityToolkit.Mvvm.ComponentModel;

namespace Rhino.Inside.AutoCAD.UI.Resources.ViewModels;

/// <summary>
/// The view model for the dialog asking the user to update Rhino.
/// </summary>
public partial class RhinoUpdateDialogViewModel : ObservableObject
{
    /// <summary>
    /// The message explaining why Rhino must be updated.
    /// </summary>
    [ObservableProperty]
    private string _message;

    /// <summary>
    /// The URL of the page to download the latest Rhino from.
    /// </summary>
    public string DownloadUrl { get; }

    /// <summary>
    /// Constructs a new <see cref="RhinoUpdateDialogViewModel"/>.
    /// </summary>
    /// <param name="message">The message explaining why Rhino must be updated.</param>
    /// <param name="downloadUrl">The URL of the page to download the latest Rhino from.</param>
    public RhinoUpdateDialogViewModel(string message, string downloadUrl)
    {
        _message = message;
        this.DownloadUrl = downloadUrl;
    }
}
