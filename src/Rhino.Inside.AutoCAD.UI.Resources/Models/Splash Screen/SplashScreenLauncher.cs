using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;
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
    // To detect redundant calls
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
    /// The thread start point to launch the progress reporter window.
    /// </summary>
    private void ThreadStartingPoint()
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

        try
        {
            Dispatcher.Run();
        }
        catch (Exception e)
        {
            _logger.LogError(e);

            _isValidThreadState = false;
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