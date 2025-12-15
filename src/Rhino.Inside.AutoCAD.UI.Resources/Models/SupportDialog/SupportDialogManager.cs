using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.ViewModels;
using Rhino.Inside.AutoCAD.UI.Resources.Views;

namespace Rhino.Inside.AutoCAD.UI.Resources.Models;

/// <inheritdoc cref="ISupportDialogManager"/>
public class SupportDialogManager : ISupportDialogManager
{
    private bool _disposed;

    private readonly ILoggerService _logger = LoggerService.Instance;

    private readonly IRhinoInsideAutoCadApplication _application;
    private SupportDialog? _dialog;
    private SupportDialogViewModel? _supportDialogViewModel;

    /// <summary>
    /// Constructs a new <see cref="SupportDialogManager"/>.
    /// </summary>
    /// <param name="application">The Rhino.Inside.AutoCAD application.</param>
    public SupportDialogManager(IRhinoInsideAutoCadApplication application)
    {
        // Add assembly resolver for WPF pack URIs
        // This is necessary because WPF's pack URI resolver can't find the assembly in the new thread context
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;

        _application = application;
    }

    /// <summary>
    /// Handles assembly resolution for WPF pack URIs and dependencies in the new thread context.
    /// </summary>
    private System.Reflection.Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        // Check if the requested assembly is the UI.Resources assembly
        var assemblyName = new System.Reflection.AssemblyName(args.Name);

        if (assemblyName.Name == "Rhino.Inside.AutoCAD.UI.Resources")
        {
            // Return the currently executing assembly (UI.Resources)
            return System.Reflection.Assembly.GetExecutingAssembly();
        }

        // Try to load the assembly from the same directory as the executing assembly
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
        catch (Exception ex)
        {
            _logger.LogError(ex);
        }

        return null;
    }

    /// <summary>
    /// Creates a new <see cref="SupportDialogViewModel"/> with version information.
    /// </summary>
    private void UpdateVersions()
    {
        var rhinoInsideVersion = _application.Bootstrapper.VersionLog.CurrentVersion;

        var rhinoInsideManager = _application.RhinoInsideManager;

        var autocadVersion = rhinoInsideManager.AutoCadInstance.ApplicationVersion;

        var rhinoVersion = rhinoInsideManager.RhinoInstance.ApplicationVersion;

        var grasshopperVersion = rhinoInsideManager.RhinoInstance.ApplicationVersion;

        _supportDialogViewModel.UpdateVersionInfo(autocadVersion, rhinoVersion, grasshopperVersion, rhinoInsideVersion);

    }

    /// <inheritdoc/>
    public void Show() => this.Show(SupportDialogTab.About);

    /// <inheritdoc/>
    public void Show(SupportDialogTab tab)
    {
        if (_dialog == null || !_dialog.IsVisible)
        {
            var viewModel = new SupportDialogViewModel();
            viewModel.SelectedTabIndex = (int)tab;
            _supportDialogViewModel = viewModel;
            _dialog = new SupportDialog(viewModel);
        }
        else
        {
            _supportDialogViewModel.SelectedTabIndex = (int)tab;
        }

        this.UpdateVersions();

        _dialog.Show();
    }

    /// <inheritdoc/>
    public void Hide()
    {
        _dialog?.Hide();
    }

    /// Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _dialog?.Close();
        }

        _disposed = true;
    }

    /// <summary>
    /// Public implementation of Dispose pattern callable by consumers.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }
}
