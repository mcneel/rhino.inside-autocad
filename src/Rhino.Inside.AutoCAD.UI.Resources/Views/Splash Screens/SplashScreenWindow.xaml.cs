using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.UI.Resources.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Rhino.Inside.AutoCAD.UI.Resources.Views;

/// <summary>
/// Interaction logic for SplashScreenWindow.xaml
/// </summary>
public partial class SplashScreenWindow : IWindow
{
    /// <summary>
    /// Constructs a new <see cref="SplashScreenWindow"/>.
    /// </summary>
    public SplashScreenWindow(SplashScreenViewModel viewModel)
    {
        InitializeComponent();

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
    /// Hides the splash screen. Only available in debug builds.
    /// </summary>
    private void HideButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        this.Visibility = Visibility.Hidden;
    }
}
