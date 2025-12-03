using Autofac;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.ComponentModel;
using System.Windows.Threading;
using IContainer = Autofac.IContainer;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// The base class main entry point into an application.
/// </summary>
public abstract class ApplicationMainBase : IApplicationMain
{
    private readonly IRhinoInsideAutoCadApplication _bimorphApplication;

    // To detect redundant calls
    private bool _disposed;

    private readonly Dispatcher _appDispatcher;

    private readonly IApplicationServicesCore _applicationServicesCore;

    protected readonly ContainerBuilder _containerBuilder;

    private readonly IWindowConfig _windowConfig;
    private readonly IVersionLog _versionLog;
    protected readonly IList<ISatelliteService> _satelliteServices;

    private readonly ILoggerService _logger = LoggerService.Instance;
    private IContainer? _container;

    /// <summary>
    /// Event raised when the application shutdown starts.
    /// </summary>
    public event EventHandler? ShutdownStarted;

    /// <summary>
    /// The IoC container providing static access.
    /// </summary>
    public static ILifetimeScope? LifeTimeContainer { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="ApplicationMainBase" />.
    /// </summary>
    protected ApplicationMainBase(IRhinoInsideAutoCadApplication bimorphApplication)
    {
        _bimorphApplication = bimorphApplication;

        var bootstrapper = bimorphApplication.Bootstrapper;

        _versionLog = bootstrapper.VersionLog;

        _windowConfig = bootstrapper.WindowConfig;

        _appDispatcher = bootstrapper.Dispatcher;

        _applicationServicesCore = bimorphApplication.ApplicationServicesCore;

        _containerBuilder = new ContainerBuilder();

        _satelliteServices = [];
    }

    /// <summary>
    /// Event handler for when the document is closing or the user changes
    /// the active document. This will close the WPF application window
    /// and shutdown the application.
    /// </summary>
    protected void OnDocumentClosingOrChanged(object sender, EventArgs e)
    {
        if (LifeTimeContainer != null && LifeTimeContainer.TryResolve<IWindow>(out var window))
        {
            window.Close();
        }
    }

    /// <summary>
    /// Shows the <paramref name="window"/> and configures it using the
    /// <see cref="IWindowConfig"/>.
    /// </summary>
    protected void ShowWindow(IWindow window)
    {
        window.Closing += this.OnWindowClosing;

        _windowConfig.Apply(window);

        window.Show();
    }

    /// <summary>
    /// Shut down window event, shuts down all services.
    /// </summary>
    private void OnWindowClosing(object sender, CancelEventArgs e)
    {
        var window = (IWindow)sender;

        window.Closing -= this.OnWindowClosing;

        this.Shutdown();
    }

    /// <summary>
    /// Classes that inherit from <see cref="ApplicationMainBase"/> are required
    /// to implement this method and register all their application-specific types
    /// to the IoC <see cref="LifeTimeContainer"/>.  
    /// </summary>
    protected abstract void RegisterTypes();

    protected virtual void ShutdownTasks()
    {

    }

    /// <summary>
    /// Runs the mainline application function.
    /// </summary>
    public RunResult Run()
    {
        try
        {
            _containerBuilder.RegisterInstance(_bimorphApplication.Bootstrapper).As<IBootstrapper>().SingleInstance();
            _containerBuilder.RegisterInstance(_bimorphApplication.ApplicationConfig).As<IApplicationConfig>().SingleInstance();
            _containerBuilder.RegisterInstance(_applicationServicesCore).As<IApplicationServicesCore>().SingleInstance();
            _containerBuilder.RegisterInstance(_versionLog).As<IVersionLog>().SingleInstance();
            _containerBuilder.RegisterInstance(_windowConfig).As<IWindowConfig>().SingleInstance();
            _containerBuilder.RegisterInstance(_appDispatcher).SingleInstance();

            foreach (var satelliteService in _satelliteServices)
            {
                _applicationServicesCore.RegisterSatelliteService(satelliteService);

                if (satelliteService.IsValid)
                    continue;

                this.Shutdown();

                return RunResult.Invalid;
            }

            this.RegisterTypes();

            _container = _containerBuilder.Build();

            LifeTimeContainer = _container.BeginLifetimeScope();

            this.ShowWindow(LifeTimeContainer.Resolve<IWindow>());

            return RunResult.Success;
        }
        catch
        {
            this.Shutdown();

            throw;
        }
    }

    /// <summary>
    /// Shuts down the application.
    /// </summary>
    public void Shutdown()
    {
        try
        {
            ShutdownStarted?.Invoke(this, EventArgs.Empty);

            _applicationServicesCore.Shutdown();

            this.ShutdownTasks();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception);
        }
        finally
        {
            _logger.Shutdown();

            this.Dispose();
        }
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _container?.Dispose();

            LifeTimeContainer?.Dispose();

            _disposed = true;
        }
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