using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Inside.AutoCAD.UI.Resources.ViewModels;
using Rhino.Inside.AutoCAD.UI.Resources.Views;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.UI.Resources.Models;

/// <summary>
/// The splash screen window launcher. Provides a UI window with
/// progress reporting and failure information for starting the
/// <see cref="IApplicationMain"/> and <see cref="ISatelliteService"/>.
/// </summary>
public class SplashScreenLauncher : ISplashScreenLauncher
{
    private bool _disposed;

    private readonly ILoggerService _logger = LoggerService.Instance;

    private readonly ISplashScreenConstants _splashScreenConstants;
    private readonly IVersionLog _versionLog;

    private SplashScreenWindow? _splashScreenWindow;
    private SplashScreenViewModel? _splashScreenViewModel;

    private Dispatcher? _dispatcher;
    private Thread? _newWindowThread;

    private bool _isValidThreadState = true;

    private readonly object _initLock = new object();
    private bool _isDispatcherReady = false;

    /// <summary>
    /// Constructs a new <see cref="SplashScreenLauncher"/>.
    /// </summary>
    public SplashScreenLauncher(IRhinoInsideAutoCadApplication application)
    {
        _splashScreenConstants = application.SettingsManager.Core.SplashScreenConstants;

        _versionLog = application.Bootstrapper.VersionLog;
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
    /// The thread start point to launch the progress reporter window.
    /// </summary>
    private void ThreadStartingPoint()
    {
        // Add assembly resolver for WPF pack URIs
        // This is necessary because WPF's pack URI resolver can't find the assembly in the new thread context
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        try
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            lock (_initLock)
            {
                _splashScreenViewModel = new SplashScreenViewModel(_splashScreenConstants, _versionLog);

                _splashScreenWindow = new SplashScreenWindow(_splashScreenViewModel!)
                {
                    Topmost = true
                };

                _splashScreenWindow.Show();

                _isDispatcherReady = true;

                Monitor.PulseAll(_initLock); // Notify all waiting threads that the dispatcher is ready
            }

            Dispatcher.Run();
        }
        catch (Exception e)
        {
            _logger.LogError(e);

            _isValidThreadState = false;
        }
        finally
        {
            // Remove the assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }
    }

    /// <summary>
    /// Shows the failure message in the splash screen by calling
    /// <see cref="SplashScreenViewModel.SetToFailedState"/>.
    /// </summary>
    private void ShowFailure(string message)
    {
        _dispatcher?.Invoke(() =>
        {
            _splashScreenViewModel?.SetToFailedState(message);
        });
    }

    /// <summary>
    /// Waits for the dispatcher to be ready.
    /// </summary>
    private void WaitForDispatcherReady()
    {
        lock (_initLock)
        {
            while (_isDispatcherReady == false)
            {
                Monitor.Wait(_initLock);
            }
        }
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            this.Close();
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

    /// <inheritdoc/>
    public void Show()
    {
        if (_isValidThreadState == false)
            return;

        _newWindowThread = new Thread(this.ThreadStartingPoint);

        _newWindowThread.SetApartmentState(ApartmentState.STA);

        _newWindowThread.IsBackground = true;

        _newWindowThread.Start();
    }

    /// <inheritdoc/>
    public void ShowFailedValidationInfo(IValidationLogger validationLogger)
    {
        this.WaitForDispatcherReady();

        _dispatcher?.Invoke(() =>
        {
            var messageInfo = validationLogger.GetMessage();

            this.ShowFailure(messageInfo);
        });
    }

    /// <inheritdoc/>
    public void ShowExceptionInfo()
    {
        this.WaitForDispatcherReady();

        _dispatcher?.Invoke(() =>
        {
            this.ShowFailure(_splashScreenConstants.FailedServiceMessage!);
        });
    }

    /// <inheritdoc/>
    public void Close()
    {
        _splashScreenViewModel?.Dispose();

        _dispatcher?.Invoke(() =>
        {
            _splashScreenWindow?.Close();
        });

        _dispatcher?.InvokeShutdown();

        _newWindowThread?.Join();
    }
}