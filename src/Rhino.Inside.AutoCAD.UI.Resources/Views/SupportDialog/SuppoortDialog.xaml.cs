using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.UI.Resources.ViewModels;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Rhino.Inside.AutoCAD.UI.Resources.Views;

/// <summary>
/// Interaction logic for LoadingScreenWindow.xaml
/// </summary>
public partial class SupportDialog : IWindow
{
    /// <summary>
    /// Constructs a new <see cref="SupportDialog"/>.
    /// </summary>
    public SupportDialog(SupportDialogViewModel viewModel)
    {
        this.InitializeComponent();

        this.DataContext = viewModel;
    }

    /// <summary>
    /// Closes the splash screen window.
    /// </summary>
    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
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

    /// <summary>
    /// Navigates to the hyperlink's URL when clicked.
    /// </summary>
    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        System.Diagnostics.Process.Start(e.Uri.ToString());
    }

}
