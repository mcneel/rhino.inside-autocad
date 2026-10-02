using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Rhino.Inside.AutoCAD.UI.Resources.Views;

/// <summary>
/// Interaction logic for RhinoUpdateDialogWindow.xaml
/// </summary>
public partial class RhinoUpdateDialogWindow : IWindow
{
    private readonly RhinoUpdateDialogViewModel _viewModel;

    /// <summary>
    /// Constructs a new <see cref="RhinoUpdateDialogWindow"/>.
    /// </summary>
    public RhinoUpdateDialogWindow(RhinoUpdateDialogViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        this.DataContext = viewModel;
    }

    /// <summary>
    /// Closes the dialog window.
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// Opens the Rhino download page in the default browser, then closes the dialog, since
    /// there is nothing more to do until Rhino is updated and AutoCAD restarted.
    /// </summary>
    private void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _viewModel.DownloadUrl,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
        catch (Exception exception)
        {
            LoggerService.Instance.LogError(exception);
        }

        this.Close();
    }

    /// <summary>
    /// Allows the window to be dragged by holding down the left mouse button.
    /// </summary>
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }
}
