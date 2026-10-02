using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.ViewModels;
using Rhino.Inside.AutoCAD.UI.Resources.Views;
using System.Windows.Interop;

namespace Rhino.Inside.AutoCAD.UI.Resources.Models;

/// <inheritdoc cref="IRhinoUpdateDialogManager"/>
/// <remarks>
/// Shows its window modally on the calling thread for the same reason as
/// <see cref="RhinoVersionDialogManager"/>: it runs during AutoCAD's startup, on AutoCAD's
/// own UI thread, where pumping a window on a second thread risks a deadlock.
/// </remarks>
public class RhinoUpdateDialogManager : IRhinoUpdateDialogManager
{
    /// <summary>
    /// Handles assembly resolution for the WPF pack URIs the window's XAML references.
    /// </summary>
    private System.Reflection.Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var assemblyName = new System.Reflection.AssemblyName(args.Name);

        if (assemblyName.Name == "Rhino.Inside.AutoCAD.UI.Resources")
        {
            return System.Reflection.Assembly.GetExecutingAssembly();
        }

        try
        {
            var executingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = System.IO.Path.GetDirectoryName(executingAssemblyPath);

            if (assemblyDirectory != null)
            {
                var assemblyPath = System.IO.Path.Combine(assemblyDirectory, assemblyName.Name + ".dll");

                if (System.IO.File.Exists(assemblyPath))
                {
                    return System.Reflection.Assembly.LoadFrom(assemblyPath);
                }
            }
        }
        catch (Exception e)
        {
            LoggerService.Instance.LogError(e);
        }

        return null;
    }

    /// <summary>
    /// Makes the dialog modal to the AutoCAD main window, so it cannot be lost behind it.
    /// </summary>
    /// <remarks>
    /// The main window handle is not guaranteed to exist this early in startup, in which
    /// case the dialog is shown unowned rather than not at all.
    /// </remarks>
    /// <param name="window">The window to set the owner of.</param>
    private void SetAutoCadOwner(RhinoUpdateDialogWindow window)
    {
        try
        {
            var mainWindow = Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow;

            if (mainWindow?.Handle is { } handle && handle != IntPtr.Zero)
            {
                var interopHelper = new WindowInteropHelper(window);

                interopHelper.Owner = handle;
            }
        }
        catch (Exception e)
        {
            LoggerService.Instance.LogError(e);
        }
    }

    /// <inheritdoc />
    public void Show(string message, string downloadUrl)
    {
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;

        try
        {
            var viewModel = new RhinoUpdateDialogViewModel(message, downloadUrl);

            var window = new RhinoUpdateDialogWindow(viewModel)
            {
                Topmost = true
            };

            this.SetAutoCadOwner(window);

            window.ShowDialog();
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
        }
    }
}
