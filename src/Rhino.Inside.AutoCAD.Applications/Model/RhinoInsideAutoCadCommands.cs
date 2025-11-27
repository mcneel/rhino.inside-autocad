using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Bimorph.Core.Services.Core;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.ApplicationSettings;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.UI.Resources.Models;
using Rhino.Runtime.InProcess;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using CADApplication = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Entity = Rhino.Inside.AutoCAD.Interop.Entity;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
using RhinoCurve = Rhino.Geometry.Curve;

[assembly: CommandClass(typeof(RhinoInsideAutoCadCommands))]

namespace Rhino.Inside.AutoCAD.Applications;

public class RhinoInsideAutoCadCommands
{
    private static UnitSystem FallbackUnitSystem => InteropConstants.FallbackUnitSystem;
    private static bool _isRunning;

    /// <summary>
    /// The command to launch a GUI application with Rhino Inside.
    /// </summary>
    private static void RunApplication(
        Func<IRhinoInsideAutoCadApplication, IInteropService, IApplicationMain> mainlineType,
        ButtonApplicationId appId)
    {
        if (_isRunning)
            return;

        var application = RhinoInsideAutoCadExtension.Application;

        var splashScreenLauncher = new SplashScreenLauncher(application);

        splashScreenLauncher.Show();

        var applicationServicesCore = application.ApplicationServicesCore;

        var fileResourceManager = application.FileResourceManager;

        try
        {
            _isRunning = true;

            var interopService = new InteropService(application, appId);

            var mainline = mainlineType(application, interopService);

            mainline.ShutdownStarted += (_, _) => _isRunning = false;

            var result = mainline.Run();

            if (result == RunResult.Invalid)
            {
                var failingService = applicationServicesCore.GetFailedService();

                splashScreenLauncher.ShowFailedValidationInfo(failingService.ValidationLogger);

                _isRunning = false;
            }
            else
            {
                splashScreenLauncher.Close();
            }
        }
        catch (Exception e)
        {
            splashScreenLauncher.ShowExceptionInfo();

            _isRunning = false;

            LoggerService.Instance.LogError(e);
        }
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "RHINO", CommandFlags.Modal)]
    public static void RHINO()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Windowed);
    }

    [CommandMethod("RHINOINSIDE_COMMANDS", "GRASSHOPPER", CommandFlags.Modal)]
    public static void GRASSHOPPER()
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoLauncher = new RhinoLauncher(application!);

        rhinoLauncher.Launch(RhinoInsideMode.Headless);

        var rhinoInstance = application.RhinoInsideManager.RhinoInstance;

        rhinoInstance.RunRhinoCommand("Grasshopper");
    }
}

/// <summary>
/// A service to launch the Rhino Inside instance, This can be used
/// to launch headless or windowed Rhino Instances.
/// </summary>
public interface IRhinoLauncher
{
    /// <summary>
    /// Launches the Rhino Inside instance.
    /// </summary>
    void Launch(RhinoInsideMode mode);
}

/// <inheritdoc cref="IRhinoLauncher"/>
public class RhinoLauncher : IRhinoLauncher
{
    private readonly IRhinoInsideManager _rhinoInsideManager;

    /// <summary>
    /// Creates a new <see cref="IRhinoLauncher"/> instance.
    /// </summary>
    public RhinoLauncher(IRhinoInsideAutoCadApplication application)
    {
        _rhinoInsideManager = application.RhinoInsideManager;
    }

    /// <inheritdoc />
    public void Launch(RhinoInsideMode mode)
    {
        //ToDo : Splash Screen then uncomment
        //var application = RhinoInsideAutoCadExtension.Application;

        // var splashScreenLauncher = new SplashScreenLauncher(application);

        // splashScreenLauncher.Show();

        try
        {
            var rhinoCoreExtension = RhinoCoreExtension.Instance;

            var validationLogger = rhinoCoreExtension.ValidationLogger;

            if (validationLogger.HasValidationErrors)
            {
                CADApplication.ShowAlertDialog(validationLogger.GetMessage());

                return;
            }

            rhinoCoreExtension.ValidateRhinoCore();

            var rhinoInstance = _rhinoInsideManager.RhinoInstance;

            rhinoInstance.ValidateRhinoDoc(mode, validationLogger);

            if (mode != RhinoInsideMode.Headless)
            {
                rhinoCoreExtension.WindowManager.ShowWindow();
            }

            //  splashScreenLauncher.Close();
        }
        catch (Exception e)
        {
            // splashScreenLauncher.ShowExceptionInfo();

            LoggerService.Instance.LogError(e);
        }
    }
}

public interface IObjectRegister
{
    bool TryGetObjectId(RhinoObject rhinoObject, out List<IEntity> entities);
    void RegisterObjectId(RhinoObject rhinoObject, List<IEntity> entities);
}

public class ObjectRegister : IObjectRegister
{
    private readonly Dictionary<Guid, List<IEntity>> _objects;

    public ObjectRegister()
    {
        _objects = new Dictionary<Guid, List<IEntity>>();
    }

    public bool TryGetObjectId(RhinoObject rhinoObject, out List<IEntity> entities)
    {
        return _objects.TryGetValue(rhinoObject.Id, out entities);
    }

    public void RegisterObjectId(RhinoObject rhinoObject, List<IEntity> entities)
    {
        _objects[rhinoObject.Id] = entities;
    }
}

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private readonly IObjectRegister _objectRegister;
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;

    /// <inheritdoc />
    public IRhinoInstance RhinoInstance { get; }

    /// <inheritdoc />
    public IAutoCadInstance AutoCadInstance { get; }

    /// <inheritdoc />
    public IUnitSystemManager UnitSystemManager { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideManager"/> instance.
    /// </summary>
    public RhinoInsideManager(IRhinoInstance rhinoInstance,
        IAutoCadInstance autoCadInstance, IObjectRegister objectRegister)
    {
        _objectRegister = objectRegister;
        this.AutoCadInstance = autoCadInstance;
        autoCadInstance.OnDocumentCreated += this.UpdateUnitSystem;
        autoCadInstance.OnUnitsChanged += this.UpdateUnitSystem;

        this.RhinoInstance = rhinoInstance;
        rhinoInstance.OnDocumentCreated += this.UpdateUnitSystem;
        rhinoInstance.OnUnitChanged += this.UpdateUnitSystem;
        rhinoInstance.OnObjectModifiedOrAppended += this.OnRhinoObjectModifiedOrAppended;
        rhinoInstance.OnObjectRemoved += this.OnRhinoObjectRemoved;

        var unitsSystemManager = new UnitSystemManager(_defaultUnitSystem, _defaultUnitSystem);

        GeometryConverter.Initialize(unitsSystemManager);

        _geometryConverter = GeometryConverter.Instance!;

        this.UnitSystemManager = unitsSystemManager;

    }

    private void OnRhinoObjectRemoved(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        if (_objectRegister.TryGetObjectId(rhinoObject, out var oldEntities))
        {
            this.AutoCadInstance.TransientManager!.RemoveEntities(oldEntities);
        }
    }

    private void OnRhinoObjectModifiedOrAppended(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        if (_objectRegister.TryGetObjectId(rhinoObject, out var oldEntities))
        {
            this.AutoCadInstance.TransientManager!.RemoveEntities(oldEntities);
        }

        if (this.TryConvert(rhinoObject, out var newEntities))
        {
            _objectRegister.RegisterObjectId(rhinoObject, newEntities);
            this.AutoCadInstance.TransientManager!.AddEntities(newEntities);
        }
    }

    private bool TryConvert(RhinoObject rhinoObject, out List<IEntity> entities)
    {
        var geometry = rhinoObject.Geometry;

        entities = new List<IEntity>();

        switch (geometry.ObjectType)
        {
            case ObjectType.Curve:
                {
                    var rhinoCurve = geometry as RhinoCurve;

                    var curves = _geometryConverter.ToAutoCadType(rhinoCurve);

                    foreach (var curve in curves)
                    {
                        var entity = new Entity(curve);
                        entities.Add(entity);
                    }

                    return true;
                }
            default: return false;

        }
    }

    private void UpdateUnitSystem(object sender, EventArgs e)
    {
        var autoCadUnits = this.AutoCadInstance.Document?.UnitSystem ?? _defaultUnitSystem;
        var rhinoUnits = this.RhinoInstance.ActiveDoc?.ModelUnitSystem ?? _defaultUnitSystem;

        if (this.UnitSystemManager.AutoCadUnits != autoCadUnits ||
            this.UnitSystemManager.RhinoUnits != rhinoUnits)
        {
            var unitsSystemManager = new UnitSystemManager(autoCadUnits, rhinoUnits);
            this.UnitSystemManager = unitsSystemManager;

            GeometryConverter.Initialize(unitsSystemManager);

            _geometryConverter = GeometryConverter.Instance!;
        }
    }
}

/// <inheritdoc cref="IRhinoInstance"/>
public class RhinoInstance : IRhinoInstance
{
    private readonly IApplicationDirectories _applicationDirectories;

    /// <inheritdoc />
    public event EventHandler? OnDocumentCreated;

    /// <inheritdoc />
    public event EventHandler? OnUnitChanged;

    /// <inheritdoc />
    public event EventHandler<IRhinoObjectModifiedEventArgs>? OnObjectModifiedOrAppended;

    /// <inheritdoc />
    public event EventHandler<IRhinoObjectModifiedEventArgs>? OnObjectRemoved;

    /// <inheritdoc />
    public IRhinoCoreExtension RhinoCore { get; }

    /// <inheritdoc />
    public RhinoDoc? ActiveDoc { get; private set; }

    public UnitSystem UnitSystem { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInstance"/> instance. Note that this does
    /// not mean that Rhino is running yet. This only constructs the instance to manage
    /// the Rhino Inside lifecycle and document. Use <see cref="IRhinoLauncher"/> to
    /// create a running Rhino instance.
    /// </summary>
    public RhinoInstance(IApplicationDirectories applicationDirectories)
    {
        _applicationDirectories = applicationDirectories;
        this.RhinoCore = RhinoCoreExtension.Instance;

    }

    /// <summary>
    /// Initializes the Rhino Inside instance and returns true if it has successfully
    /// launched otherwise false indicating a failure.
    /// </summary>
    private RhinoDoc CreateRhinoDoc(IValidationLogger validationLogger,
        RhinoInsideMode mode)
    {
        var template =
            $"{_applicationDirectories.Resources}Large Objects - Millimeters.3dm";

        try
        {

            var rhinoDoc = mode == RhinoInsideMode.Headless
                ? RhinoDoc.CreateHeadless(template)
                : RhinoDoc.Create(template);

            FileSettings.AutoSaveEnabled = false;

            this.OnDocumentCreated?.Invoke(this, EventArgs.Empty);

            this.UnitSystem = rhinoDoc.ModelUnitSystem;

            RhinoDoc.DocumentPropertiesChanged += this.OnDocumentPropertiesModified;
            RhinoDoc.AddRhinoObject += this.OnAddRhinoObject;
            RhinoDoc.ModifyObjectAttributes += this.OnModifyRhinoObject;
            RhinoDoc.DeleteRhinoObject += this.OnRemoveRhinoObject;
            return rhinoDoc;
        }
        catch
        {
            validationLogger.AddMessage("Failed to initialize Rhino Doc.");

            throw;
        }
    }

    /// <summary>
    /// Event handler which fires when a Rhino object is removed.
    /// </summary>
    private void OnRemoveRhinoObject(object sender, RhinoObjectEventArgs e)
    {
        this.OnObjectRemoved?.Invoke(this, new RhinoObjectModifiedEventArgs(e.TheObject));
    }

    /// <summary>
    /// Event handler which fires when a Rhino object is modified.
    /// </summary>
    private void OnModifyRhinoObject(object sender, RhinoModifyObjectAttributesEventArgs e)
    {
        this.OnObjectModifiedOrAppended?.Invoke(this, new RhinoObjectModifiedEventArgs(e.RhinoObject));
    }

    /// <summary>
    /// Event handler which fires when a Rhino object is added.
    /// </summary>
    private void OnAddRhinoObject(object sender, RhinoObjectEventArgs e)
    {
        this.OnObjectModifiedOrAppended?.Invoke(this, new RhinoObjectModifiedEventArgs(e.TheObject));
    }

    /// <summary>
    /// An event handler which fires when the Rhino document properties are modified.
    /// It checks to see if the unit system has changed and raises the <see cref="OnUnitChanged"/>
    /// event if it has.
    /// </summary>
    private void OnDocumentPropertiesModified(object sender, DocumentEventArgs e)
    {
        var currentUnits = e.Document.ModelUnitSystem;

        if (currentUnits == this.UnitSystem)
            return;

        this.OnUnitChanged?.Invoke(this, EventArgs.Empty);

        this.UnitSystem = currentUnits;
    }

    /// <inheritdoc />
    public void ValidateRhinoDoc(RhinoInsideMode mode, IValidationLogger validationLogger)
    {
        if (this.ActiveDoc == null)
        {
            this.ActiveDoc = this.CreateRhinoDoc(validationLogger, mode);
        }
    }

    /// <inheritdoc />
    public Result RunRhinoCommand(string commandName)
    {
        return this.ActiveDoc == null
            ? Result.Failure
            : RhinoApp.ExecuteCommand(this.ActiveDoc, commandName);
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        RhinoDoc.DocumentPropertiesChanged -= this.OnDocumentPropertiesModified;
        RhinoDoc.AddRhinoObject -= this.OnAddRhinoObject;
        RhinoDoc.ModifyObjectAttributes -= this.OnModifyRhinoObject;
        RhinoDoc.DeleteRhinoObject -= this.OnRemoveRhinoObject;
        this.ActiveDoc?.Dispose();
    }
}

public class RhinoObjectModifiedEventArgs : IRhinoObjectModifiedEventArgs
{
    /// <inheritdoc/>
    public RhinoObject RhinoObject { get; }
    /// <summary>
    /// Constructs a new <see cref="IRhinoObjectModifiedEventArgs"/> instance.
    /// </summary>
    public RhinoObjectModifiedEventArgs(RhinoObject rhinoObject)
    {
        this.RhinoObject = rhinoObject;
    }
}

/// <inheritdoc cref="IRhinoWindowManager"/>
public class RhinoWindowManager : IRhinoWindowManager
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr windowHandle, int windowShowStyle);

    /// <summary>
    /// The pointer to the Rhino main window.
    /// </summary>
    private IntPtr _mainWindow;

    /// <summary>
    /// Constructs a new <see cref="IRhinoWindowManager"/> instance. This is the
    /// default state and does not have a window associated with it.
    /// </summary>
    public RhinoWindowManager()
    {
        _mainWindow = IntPtr.Zero;
    }

    /// <inheritdoc />
    public void SetWindow(IntPtr mainWindow)
    {
        _mainWindow = mainWindow;
    }

    /// <inheritdoc />
    public void HideWindow()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.Hide);
    }

    /// <inheritdoc />
    public void ShowWindow()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.Show);
    }
}

/// <summary>
/// Launches the Rhino inside instance.
/// </summary>
public class RhinoCoreExtension : IRhinoCoreExtension
{

    private static RhinoCore? _rhinoCore;

    /// <summary>
    /// True if Rhino is installed otherwise false.
    /// </summary>
    private static readonly bool _rhinoInstallDirectoryExists;

    /// <summary>
    /// The <see cref="RhinoCoreExtension"/> singleton instance.
    /// </summary>
    public static RhinoCoreExtension Instance { get; }

    /// <summary>
    /// The <see cref="IValidationLogger"/>.
    /// </summary>
    public IValidationLogger ValidationLogger { get; }

    /// <summary>
    /// Access to the Rhino window manager.
    /// </summary>
    public IRhinoWindowManager WindowManager { get; }

    /// <summary>
    /// Gets the Rhino system directory in the local machines registry.
    /// </summary>
    static readonly string _systemDir = (string)Microsoft.Win32.Registry.GetValue
    (
        @"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", string.Empty
    );

    /// <summary>
    /// Constructs a new <see cref="RhinoCoreExtension"/> instance.
    /// </summary>
    private RhinoCoreExtension()
    {
        this.ValidationLogger = new ValidationLogger();
        this.WindowManager = new RhinoWindowManager();
    }

    /// <summary>
    /// Uses assembly resolver to load the Rhino assembly once per app domain.
    /// </summary>
    static RhinoCoreExtension()
    {
        Instance = new RhinoCoreExtension();

        _rhinoInstallDirectoryExists = Directory.Exists(_systemDir);

        if (_rhinoInstallDirectoryExists)
        {
            ResolveEventHandler? OnRhinoCommonResolve = null;

            AppDomain.CurrentDomain.AssemblyResolve += OnRhinoCommonResolve = (_, args) =>
            {
                var rhinoCommonAssemblyName = "RhinoCommon";

                var assemblyName = new AssemblyName(args.Name).Name;

                if (assemblyName != rhinoCommonAssemblyName)
                    return null;

                AppDomain.CurrentDomain.AssemblyResolve -= OnRhinoCommonResolve;
                return Assembly.LoadFrom(Path.Combine(_systemDir, $"{rhinoCommonAssemblyName}.dll"));
            };
        }
        else
        {
            Instance.ValidationLogger.AddMessage("Rhino 7 not installed or could not be found. The application requires Rhino 7 to run.");
        }
    }

    /// <summary>
    /// Disposes the Rhino core when the rhino window is closed.
    /// </summary>
    private void OnClosing(object sender, EventArgs e)
    {
        RhinoApp.Closing -= this.OnClosing;

        _rhinoCore?.Dispose();
    }

    /// <summary>
    /// Creates the Rhino core instance.
    /// </summary>
    private void CreateCore()
    {
        try
        {
            var schemeName =
                $"Inside-{HostApplicationServices.Current.Product}-{HostApplicationServices.Current.releaseMarketVersion}";

            var style = WindowStyle.Hidden;

            var args = new[]
            {
                    "/nosplash",
                    $"/scheme={schemeName}"
                };

            _rhinoCore ??= new RhinoCore(args, style);

            var mainWindow = RhinoApp.MainWindowHandle();

            this.WindowManager.SetWindow(mainWindow);

            RhinoApp.Closing += this.OnClosing;

        }
        catch
        {
            this.ValidationLogger.AddMessage("Failed to initialize Rhino Core");

            throw;
        }
    }

    /// <summary>
    /// Ensures that the Rhino core is, created and running, if there is not an existing
    /// instance then it creates one.
    /// </summary>
    public void ValidateRhinoCore()
    {
        if (_rhinoCore == null)
            this.CreateCore();
    }

    /// <summary>
    /// The steps to take to shut down this rhino inside extension.
    /// </summary>
    public void Shutdown()
    {
        RhinoApp.Closing -= this.OnClosing;
        _rhinoCore?.Dispose();
    }
}

/// <summary>
/// The host <see cref="IAutoCadDocument"/> <see cref="ISatelliteService"/>.
/// The application is attached to this object and persists for its lifetime.
/// </summary>
public class AutoCadInstance : IAutoCadInstance
{
    private readonly Dispatcher _dispatcher;

    private DocumentCollection? _documentManager;
    private Document? _activeDocument;

    private readonly string _unsavedNotSupported = MessageConstants.UnsavedNotSupported;
    private readonly string _readOnlyNotSupported = MessageConstants.ReadOnlyNotSupported;
    private readonly string _fileUnitsNotSupported = MessageConstants.FileUnitsNotSupported;

    private bool _documentClosing;

    /// <inheritdoc/>
    public event EventHandler? OnDocumentCreated;

    /// <inheritdoc/>
    public event EventHandler? OnUnitsChanged;

    /// <inheritdoc/>
    public event EventHandler? DocumentClosingOrActivated;

    /// <inheritdoc/>
    public IValidationLogger ValidationLogger { get; }

    /// <inheritdoc/>
    public bool IsValid => this.ValidationLogger.HasValidationErrors == false;

    /// <inheritdoc/>
    public IAutoCadDocument Document { get; }

    /// <inheritdoc/>
    public IObjectIdTagDatabaseManager TagDatabaseManager { get; }

    /// <inheritdoc/>
    public IDataTagDatabaseManager DataTagDatabaseManager { get; }

    /// <inheritdoc/>
    public ITransientManager TransientManager { get; }

    /// <summary>
    /// Constructs a new <see cref="InteropService"/>.
    /// </summary>
    public AutoCadInstance(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;

        _documentManager = Application.DocumentManager;

        _activeDocument = _documentManager.MdiActiveDocument;

        var documentCloseAction = new DocumentCloseAction(_activeDocument, _documentManager);

        var document = new AutocadDocumentFile(_activeDocument, documentCloseAction, _dispatcher);

        _activeDocument.BeginDocumentClose += this.OnDocumentClosing;
        _documentManager.DocumentActivated += this.OnDocumentActivated;
        document.OnUnitsChanged += this.DocumentUnitsChanged;

        this.Document = document;

        this.TagDatabaseManager = new ObjectIdTagDatabaseManager(document);

        this.DataTagDatabaseManager = new DataTagDatabaseManager(document);

        this.TransientManager = new TransientManagerWrapper(Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager);

        this.ValidationLogger = new ValidationLogger();

        this.Validate(document);
    }

    /// <summary>
    /// Bubble up the units changed event from the document.
    /// </summary>
    private void DocumentUnitsChanged(object sender, EventArgs e)
    {
        this.OnUnitsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event handler which fires when the <see cref=" DocumentCollection.DocumentActivated"/>
    /// is raised. Raises the <see cref="DocumentClosingOrActivated"/> event. If the document
    /// is closing, the event is not raised.
    /// </summary>
    protected void OnDocumentActivated(object sender, DocumentCollectionEventArgs e)
    {
        if (_documentClosing == false)
            this.OnDocumentClosingOrChanged(EventArgs.Empty);

        this.OnDocumentCreated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event handler which fires when the <see cref="Document.BeginDocumentClose"/>
    /// event is raised. Veto's the Raises the <see cref="DocumentClosingOrActivated"/> event.
    /// </summary>
    protected void OnDocumentClosing(object sender, DocumentBeginCloseEventArgs e)
    {
        e.Veto();

        _documentClosing = true;

        this.OnDocumentClosingOrChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Validates this service by posting any known invalid states to the
    /// <see cref="ValidationLogger"/>.
    /// </summary>
    private void Validate(IAutoCadDocument autoCadDocument)
    {
        var validationLogger = this.ValidationLogger;

        var cadDocument = autoCadDocument.Unwrap();

        // If the file is not saved, the document is not named.
        if (cadDocument.IsNamedDrawing == false)
        {
            validationLogger.AddMessage(_unsavedNotSupported);
        }

        if (cadDocument.IsReadOnly)
        {
            validationLogger.AddMessage(_readOnlyNotSupported);
        }

        var unitSystem = autoCadDocument.UnitSystem;
        if (unitSystem == UnitSystem.Unset)
        {
            validationLogger.AddMessage(string.Format(_fileUnitsNotSupported, unitSystem));

        }
    }

    /// <summary>
    /// Event handler which raises the <see cref="DocumentClosingOrActivated"/> event.
    /// </summary>
    protected virtual void OnDocumentClosingOrChanged(EventArgs e)
    {
        DocumentClosingOrActivated?.Invoke(this, e);
    }

    /// <inheritdoc/>
    protected void RestartTasks()
    {
        _activeDocument!.BeginDocumentClose -= this.OnDocumentClosing;
        _documentManager!.DocumentActivated -= this.OnDocumentActivated;
        this.Document.OnUnitsChanged -= this.DocumentUnitsChanged;

        this.TagDatabaseManager!.CommitAll();
        this.DataTagDatabaseManager!.CommitAll();
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        var document = this.Document;

        if (document != null)
        {
            _activeDocument!.BeginDocumentClose -= this.OnDocumentClosing;
            _documentManager!.DocumentActivated -= this.OnDocumentActivated;
            this.Document.OnUnitsChanged -= this.DocumentUnitsChanged;

            this.TagDatabaseManager?.CommitAll();
            this.DataTagDatabaseManager?.CommitAll();

            document.Close();
        }
    }
}