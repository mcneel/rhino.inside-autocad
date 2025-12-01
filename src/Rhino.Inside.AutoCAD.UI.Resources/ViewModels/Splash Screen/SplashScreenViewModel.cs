using Rhino.Inside.AutoCAD.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.UI.Resources.ViewModels;

/// <summary>
/// The view model for the MFE Formwork software splash screen.
/// </summary>
public partial class SplashScreenViewModel : ObservableObject, IDisposable
{
    // To detect redundant calls
    private bool _disposed;
    private double _timeElapsed;

    // In milliseconds.
    private readonly int _tickIncrement = 250;

    private readonly DispatcherTimer _dispatcherTimer;

    /// <summary>
    /// The <see cref="Visibility"/> of the warning icon. Displays if there has been
    /// a start-up failure.
    /// </summary>
    [ObservableProperty]
    private Visibility _warningIconVisibility = Visibility.Hidden;

    /// <summary>
    /// The <see cref="Visibility"/> of the action button in the splash screen.
    /// </summary>
    [ObservableProperty]
    private Visibility _buttonVisibility = Visibility.Hidden;

    /// <summary>
    /// The <see cref="Visibility"/> of the progress messages in the splash screen.
    /// </summary>
    [ObservableProperty]
    private Visibility _progressMessageVisibility = Visibility.Visible;

    /// <summary>
    /// The current <see cref="IProgressReport"/> that has been posted.
    /// </summary>
    [ObservableProperty]
    private IProgressReport? _currentProgressReport;

    /// <summary>
    /// The error message to display to the user if there has been a startup
    /// failure.
    /// </summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// The version and copyright notice which appears on the splash screen.
    /// </summary>
    [ObservableProperty] private string _versionCopyrightNotice;

    /// <summary>
    /// The time elapsed message posted to the UI.
    /// </summary>
    [ObservableProperty] private string _timeElapsedMessage = string.Empty;

#if DEBUG
    public bool ShowHideButton => true;
#else
        public bool ShowHideButton => false;
#endif

    /// <summary>
    /// Constructs a new <see cref="SplashScreenViewModel"/>.
    /// </summary>
    public SplashScreenViewModel(ISplashScreenConstants splashScreenConstants, IVersionLog versionLog)
    {
        var version = versionLog.CurrentVersion;

        _versionCopyrightNotice = $"{splashScreenConstants.Copyright} {DateTime.Now.Year} ©. {splashScreenConstants.VersionPrefix} {version}";

        _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = new TimeSpan(0, 0, 0, 0, _tickIncrement)
        };

        _dispatcherTimer.Tick += this.dispatcherTimer_Tick;

        _dispatcherTimer.Start();
    }

    /// <summary>
    /// Sets the splash screen window to the failed state which informs the user of
    /// a <see cref="ISatelliteService.StartUp"/> failure.
    /// </summary>
    public void SetToFailedState(string failureMessage)
    {
        _dispatcherTimer.Tick -= this.dispatcherTimer_Tick;

        _dispatcherTimer.Stop();

        this.ButtonVisibility = Visibility.Visible;

        this.WarningIconVisibility = Visibility.Visible;

        this.ErrorMessage = failureMessage;

        this.ProgressMessageVisibility = Visibility.Hidden;
    }

    /// <summary>
    /// Starts the internal timer to provide progress feedback.
    /// </summary>
    private void dispatcherTimer_Tick(object sender, EventArgs e)
    {
        _timeElapsed += _tickIncrement;

        this.TimeElapsedMessage = $"Time elapsed {(int)(_timeElapsed / 1000)}s";
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing & _dispatcherTimer.IsEnabled)
        {
            _dispatcherTimer.Tick -= this.dispatcherTimer_Tick;

            _dispatcherTimer.Stop();
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