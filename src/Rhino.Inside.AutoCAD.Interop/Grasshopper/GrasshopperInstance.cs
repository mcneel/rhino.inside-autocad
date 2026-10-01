using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Represents an implementation of <see cref="IGrasshopperInstance"/> that manages the
/// lifecycle and interactions with Grasshopper within the Rhino.Inside.AutoCAD
/// environment.
/// </summary>
public class GrasshopperInstance : IGrasshopperInstance
{
    private readonly IInstallationDirectories _installationDirectories;

    private readonly bool _loadCivil;
    private const string _grasshopperLibraryFileName = InteropConstants.GrasshopperLibraryFileName;
    private const string _grasshopperCivilLibraryFileName = InteropConstants.GrasshopperCivilLibraryFileName;
    private const string _loadGhaMethodNotFound = MessageConstants.LoadGhaMethodNotFound;
    private const string _grasshopperInitializationFailed = MessageConstants.GrasshopperInitializationFailed;
    private const string _grasshopperLibraryLoadFailedFormat = MessageConstants.GrasshopperLibraryLoadFailedFormat;
    private const string _grasshopperHostDiagnosticFormat = MessageConstants.GrasshopperHostDiagnosticFormat;
    private const string _loadGhaResolvedFormat = MessageConstants.LoadGhaResolvedFormat;
    private const string _grasshopperLibraryDiagnosticFormat = MessageConstants.GrasshopperLibraryDiagnosticFormat;
    private const string _grasshopperLibraryTypesLoadedFormat = MessageConstants.GrasshopperLibraryTypesLoadedFormat;
    private const string _grasshopperLibraryTypeLoadFailedFormat = MessageConstants.GrasshopperLibraryTypeLoadFailedFormat;
    private const string _grasshopperLibraryRegisteredFormat = MessageConstants.GrasshopperLibraryRegisteredFormat;
    private const string _grasshopperLibraryAlreadyRegisteredFormat = MessageConstants.GrasshopperLibraryAlreadyRegisteredFormat;
    private const string _loadGhaMethodName = InteropConstants.LoadGhaMethodName;
    private const string _loadGhaReturnedFormat = MessageConstants.LoadGhaReturnedFormat;
    private const string _grasshopperLoadingExceptionsFormat = MessageConstants.GrasshopperLoadingExceptionsFormat;
    private const string _grasshopperLoadingExceptionFormat = MessageConstants.GrasshopperLoadingExceptionFormat;
    private const string _grasshopperAssemblyExtension = InteropConstants.GrasshopperAssemblyExtension;
    private const string _grasshopperLibrariesFolderName = InteropConstants.GrasshopperLibrariesFolderName;
    private const string _applicationFolderName = ApplicationConstants.ApplicationFolderName;

    private IGrasshopperSelectionTracker? _selectionTracker;
    private GH_Canvas? _activeCanvas;

    /// <summary>
    /// Watches the Grasshopper editor for being minimised, restored, hidden or shown.
    /// Created with this instance, and attached to the editor once it exists.
    /// </summary>
    private readonly GrasshopperWindowManager _windowManager;

    /// <inheritdoc />
    public event EventHandler<IGrasshopperObjectModifiedEventArgs>? PreviewExpired;

    /// <inheritdoc />
    public event EventHandler<IGrasshopperObjectModifiedEventArgs>? ObjectRemoved;

    /// <inheritdoc />
    public event EventHandler<IGrasshopperSelectionEventArgs>? ComponentSelectionChanged;

    /// <inheritdoc />
    public event EventHandler? ActiveDocumentChanged;

    /// <inheritdoc />
    public event EventHandler? EditorDisplayStateChanged;

    /// <inheritdoc />
    public GH_Document? ActiveDoc { get; private set; }

    /// <inheritdoc />
    public Version? ApplicationVersion { get; private set; }

    /// <inheritdoc />
    public bool IsEnabled => Grasshopper.Kernel.GH_Document.EnableSolutions;

    /// <inheritdoc />
    public IGrasshopperWindowManager WindowManager => _windowManager;

    /// <inheritdoc />
    public bool IsEditorMinimised => _windowManager.IsMinimised;

    /// <inheritdoc />
    public bool IsEditorHidden => _windowManager.IsHidden;

    /// <summary>
    /// Initializes a new instance of the <see cref="GrasshopperInstance"/> class.
    /// </summary>
    /// <param name="installationDirectories">
    /// The application directories used to locate resources.
    /// </param>
    /// <param name="loadCivil">
    /// A Boolean indicating if the Civil3d grasshopper library will
    /// also be loaded when grasshopper loads
    /// </param>
    public GrasshopperInstance(IInstallationDirectories installationDirectories, bool loadCivil)
    {
        _installationDirectories = installationDirectories;
        _loadCivil = loadCivil;

        var windowManager = new GrasshopperWindowManager();

        windowManager.DisplayStateChanged += this.OnEditorDisplayStateChanged;

        _windowManager = windowManager;
    }

    /// <summary>
    /// Uses reflection to load the Grasshopper library into the Grasshopper
    /// component server.
    /// </summary>
    /// <exception cref="TargetException">
    /// Thrown if the LoadGHA method is not found.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown if an error occurs while invoking the LoadGHA method.
    /// </exception>
    private void LoadGrasshopperLibrary()
    {
        var assembliesFolder = _installationDirectories.VersionedAssemblies;
        var grasshopperLibraryPath = Path.Combine(assembliesFolder, _grasshopperLibraryFileName);
        var grasshopperCivilLibraryPath = Path.Combine(assembliesFolder, _grasshopperCivilLibraryFileName);

        var logger = LoggerService.Instance;

        this.LogGrasshopperHost(logger, assembliesFolder);

        var loadGhaMethod = this.ResolveLoadGhaMethod();

        if (loadGhaMethod == null)
        {
            throw new TargetException(_loadGhaMethodNotFound);
        }

        logger.LogMessage(string.Format(_loadGhaResolvedFormat, loadGhaMethod));

        this.LoadLibrary(loadGhaMethod, grasshopperLibraryPath, logger);

        // Guarded separately from the main library. Nesting this inside the main library's
        // "already registered?" check meant the Civil components never loaded once the main
        // library was registered.
        if (_loadCivil)
        {
            this.LoadLibrary(loadGhaMethod, grasshopperCivilLibraryPath, logger);
        }
    }

    /// <summary>
    /// Registers a single component library with the Grasshopper component server.
    /// </summary>
    /// <param name="loadGhaMethod">The resolved <c>GH_ComponentServer.LoadGHA</c> method.</param>
    /// <param name="libraryPath">The full path of the component library to register.</param>
    /// <param name="logger">The logger to record diagnostics to.</param>
    private void LoadLibrary(MethodInfo loadGhaMethod, string libraryPath, ILoggerService logger)
    {
        var grasshopperAssemblyPath = this.MirrorAsGrasshopperAssembly(libraryPath);

        var externalFile = new GH_ExternalFile(grasshopperAssemblyPath);

        logger.LogMessage(string.Format(_grasshopperLibraryDiagnosticFormat,
            grasshopperAssemblyPath, File.Exists(grasshopperAssemblyPath),
            externalFile.FileType));

        // Loaded from the original, not the mirror. Assembly.LoadFrom resolves both to the
        // one assembly already in the process, so this is only about being explicit.
        var assembly = Assembly.LoadFrom(libraryPath);

        // Before anything that can throw, so the diagnosis survives a later failure.
        this.LogLibraryTypes(assembly, logger);

        var libraryName = Path.GetFileName(libraryPath);

        if (IsRegistered(assembly))
        {
            logger.LogMessage(string.Format(_grasshopperLibraryAlreadyRegisteredFormat,
                libraryName));

            return;
        }

        var countBefore = Instances.ComponentServer.Libraries.Count;

        object? loaded = null;

        try
        {
            loaded = loadGhaMethod.Invoke(Instances.ComponentServer,
                this.BuildLoadGhaArguments(loadGhaMethod, externalFile));
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
        }

        // LoadGHA reports refusal by returning false rather than by throwing, so this is
        // the difference between "registered nothing" and "declined to register".
        logger.LogMessage(string.Format(_loadGhaReturnedFormat, loaded, libraryName));

        logger.LogMessage(string.Format(_grasshopperLibraryRegisteredFormat,
            libraryName,
            countBefore,
            Instances.ComponentServer.Libraries.Count,
            IsRegistered(assembly)));

        this.LogLoadingExceptions(logger);
    }

    /// <summary>
    /// Returns the path of a ".gha" copy of the given component library, creating or
    /// refreshing it when it is missing or out of date.
    /// </summary>
    /// <remarks>
    /// A ".gha" is just a renamed assembly, but the extension is not cosmetic to
    /// Grasshopper: <c>GH_ExternalFile.FileType</c> is derived from it, and Rhino 9 declines
    /// to register a file it classifies as anything other than an assembly. Rhino 8 loaded
    /// the ".dll" happily, which is why this was not needed before.
    /// <para>
    /// The libraries cannot simply be renamed at build time because AutoCAD loads the same
    /// files as managed modules through PackageContents.xml, which requires ".dll". Having
    /// both on disk costs nothing at runtime: the assembly is already loaded from the
    /// original, and <see cref="Assembly.LoadFrom(string)"/> resolves the copy to that same
    /// assembly rather than loading a second one.
    /// </para>
    /// </remarks>
    /// <param name="libraryPath">The full path of the component library.</param>
    /// <returns>The full path of the ".gha" copy.</returns>
    private string MirrorAsGrasshopperAssembly(string libraryPath)
    {
        var mirrorDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            _applicationFolderName,
            _grasshopperLibrariesFolderName);

        Directory.CreateDirectory(mirrorDirectory);

        var mirrorPath = Path.Combine(mirrorDirectory,
            Path.GetFileNameWithoutExtension(libraryPath) + _grasshopperAssemblyExtension);

        var library = new FileInfo(libraryPath);
        var mirror = new FileInfo(mirrorPath);

        // Refreshed on every upgrade, so the mirror never serves components from a build
        // the user has replaced.
        if (mirror.Exists &&
            mirror.Length == library.Length &&
            mirror.LastWriteTimeUtc == library.LastWriteTimeUtc)
            return mirrorPath;

        File.Copy(libraryPath, mirrorPath, true);

        File.SetLastWriteTimeUtc(mirrorPath, library.LastWriteTimeUtc);

        return mirrorPath;
    }

    /// <summary>
    /// Records the loading exceptions Grasshopper collected.
    /// </summary>
    /// <remarks>
    /// Grasshopper does not throw when it rejects a library; it records the reason here and
    /// returns false, which is why this is read after every registration attempt.
    /// </remarks>
    /// <param name="logger">The logger to record diagnostics to.</param>
    private void LogLoadingExceptions(ILoggerService logger)
    {
        var loadingExceptions = Instances.ComponentServer.LoadingExceptions;

        if (loadingExceptions == null || loadingExceptions.Count == 0)
            return;

        var detail = string.Join(Environment.NewLine, loadingExceptions
            .Where(loadingException => loadingException != null)
            .Select(loadingException => string.Format(_grasshopperLoadingExceptionFormat,
                loadingException.Type, loadingException.Name, loadingException.Message)));

        logger.LogMessage(string.Format(_grasshopperLoadingExceptionsFormat,
            loadingExceptions.Count, detail));
    }

    /// <summary>
    /// Returns true when the component server already holds the given assembly.
    /// </summary>
    /// <param name="assembly">The component library assembly.</param>
    private static bool IsRegistered(Assembly assembly)
    {
        return Instances.ComponentServer.Libraries.Contains(
            new GH_AssemblyInfoStub(assembly), new GH_AssemblyInfoStubComparer());
    }

    /// <summary>
    /// Resolves the private <c>GH_ComponentServer.LoadGHA</c> method used to register a
    /// component library.
    /// </summary>
    /// <remarks>
    /// Searched by name and parameter shape rather than with a plain
    /// <see cref="Type.GetMethod(string, BindingFlags)"/> call, which throws
    /// <see cref="AmbiguousMatchException"/> as soon as a Rhino release adds an overload.
    /// Falls back to the single-parameter form so a dropped trailing argument does not
    /// break registration either.
    /// </remarks>
    /// <returns>The method, or null when no usable overload exists.</returns>
    private MethodInfo? ResolveLoadGhaMethod()
    {
        var candidates = typeof(GH_ComponentServer)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(method => method.Name == _loadGhaMethodName)
            .ToList();

        return candidates.FirstOrDefault(method =>
                   HasParameters(method, typeof(GH_ExternalFile), typeof(bool))) ??
               candidates.FirstOrDefault(method =>
                   HasParameters(method, typeof(GH_ExternalFile)));
    }

    /// <summary>
    /// Returns true when the method takes exactly the given parameter types.
    /// </summary>
    /// <param name="method">The method to test.</param>
    /// <param name="parameterTypes">The parameter types to match.</param>
    private static bool HasParameters(MethodInfo method, params Type[] parameterTypes)
    {
        var parameters = method.GetParameters();

        if (parameters.Length != parameterTypes.Length)
            return false;

        return !parameters.Where((parameter, index) =>
            parameter.ParameterType != parameterTypes[index]).Any();
    }

    /// <summary>
    /// Builds the argument list for the resolved <c>LoadGHA</c> overload.
    /// </summary>
    /// <param name="loadGhaMethod">The resolved method.</param>
    /// <param name="externalFile">The component library to register.</param>
    private object[] BuildLoadGhaArguments(MethodInfo loadGhaMethod, GH_ExternalFile externalFile)
    {
        return loadGhaMethod.GetParameters().Length == 1
            ? [externalFile]
            : [externalFile, false];
    }

    /// <summary>
    /// Records which Grasshopper the plugin bound to and where its component libraries are
    /// being loaded from.
    /// </summary>
    /// <param name="logger">The logger to record diagnostics to.</param>
    /// <param name="assembliesFolder">The folder holding the component libraries.</param>
    private void LogGrasshopperHost(ILoggerService logger, string assembliesFolder)
    {
        var grasshopperAssembly = typeof(GH_ComponentServer).Assembly;

        logger.LogMessage(string.Format(_grasshopperHostDiagnosticFormat,
            grasshopperAssembly.FullName, grasshopperAssembly.Location, assembliesFolder));
    }

    /// <summary>
    /// Records whether a component library can expose its types.
    /// </summary>
    /// <remarks>
    /// Grasshopper discovers components by reflecting over the assembly's types and
    /// registers nothing, silently, when that throws - which presents as a canvas with the
    /// Rhino.Inside.AutoCAD tabs missing and no error anywhere. Forcing the same reflection
    /// here turns that into a log entry naming the member which moved between Rhino
    /// versions.
    /// </remarks>
    /// <param name="assembly">The component library assembly.</param>
    /// <param name="logger">The logger to record diagnostics to.</param>
    private void LogLibraryTypes(Assembly assembly, ILoggerService logger)
    {
        try
        {
            var types = assembly.GetTypes();

            logger.LogMessage(string.Format(_grasshopperLibraryTypesLoadedFormat,
                assembly.FullName, types.Length));
        }
        catch (ReflectionTypeLoadException e)
        {
            var loaderExceptions = string.Join(Environment.NewLine, e.LoaderExceptions
                .Where(loaderException => loaderException != null)
                .Select(loaderException => loaderException!.Message)
                .Distinct());

            var message = string.Format(_grasshopperLibraryTypeLoadFailedFormat,
                assembly.FullName, loaderExceptions);

            logger.LogError(e, message);

            // Registration will carry on and quietly add nothing, so this is the only point
            // at which the user can be told why the components are about to be missing.
            RhinoApp.WriteLine(message);
        }
    }

    /// <summary>
    /// Loads and initializes the Grasshopper environment.
    /// </summary>
    /// <param name="startUpLogger">
    /// The logger to record validation messages.
    /// </param>
    /// <returns>
    /// The active Grasshopper document.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown if Grasshopper fails to initialize.
    /// </exception>
    private void LoadGrasshopper(IStartUpLogger startUpLogger)
    {
        try
        {

            GooTypeRegistry.Initialize();

            // Removed first, as this runs on every launch and the handler is only wanted once.
            Grasshopper.Instances.CanvasCreated -= this.OnCanvasCreated;
            Grasshopper.Instances.CanvasCreated += this.OnCanvasCreated;

            // Picks up an editor which already exists, such as on a relaunch.
            _windowManager.TryAttach();

            this.ApplicationVersion = new Version(Grasshopper.Versioning.Version.ToString());
        }
        catch
        {
            startUpLogger.AddError(_grasshopperInitializationFailed);
            throw;
        }
    }

    /// <summary>
    /// Loads the component libraries and registers event handlers when a new Grasshopper
    /// canvas is created.
    /// </summary>
    /// <remarks>
    /// Grasshopper raises this while building the canvas, and the launch path in
    /// <c>RhinoLauncher</c> has long since returned, so an escaping exception would be
    /// reported nowhere and leave the canvas half wired up. Failing to register the
    /// components is reported and survivable; the canvas itself still works.
    /// </remarks>
    private void OnCanvasCreated(GH_Canvas canvas)
    {
        try
        {
            this.LoadGrasshopperLibrary();
        }
        catch (Exception e)
        {
            var message = string.Format(_grasshopperLibraryLoadFailedFormat, e.Message);

            LoggerService.Instance.LogError(e, message);

            RhinoApp.WriteLine(message);
        }

        _activeCanvas = Grasshopper.Instances.ActiveCanvas;
        _activeCanvas.DocumentChanged += this.OnDocumentChanged;

        // The editor holding the canvas is not assigned to Instances.DocumentEditor until
        // after the canvas is created, so it is looked for once AutoCAD is next idle.
        _windowManager.ScheduleAttach();
    }

    /// <summary>
    /// Handles the Grasshopper editor being minimised, restored, hidden or shown by raising
    /// <see cref="EditorDisplayStateChanged"/>.
    /// </summary>
    private void OnEditorDisplayStateChanged(object? sender, EventArgs e)
    {
        this.EditorDisplayStateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the event when objects are added to the Grasshopper document.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e"
    /// >The event data.
    /// </param>
    private void OnObjectsAdded(object sender, GH_DocObjectEventArgs e)
    {
        foreach (var ghDocumentObject in e.Objects)
        {
            this.HookPreviewExpired(ghDocumentObject);
        }
    }

    /// <summary>
    /// Handles the event when objects are deleted from the Grasshopper document.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The event data.
    /// </param>
    private void OnObjectsDeleted(object sender, GH_DocObjectEventArgs e)
    {
        foreach (var ghDocumentObject in e.Objects)
        {
            this.UnhookPreviewExpired(ghDocumentObject);

            this.ObjectRemoved?.Invoke(this,
                new GrasshopperObjectModifiedEventArgs(ghDocumentObject));
        }
    }

    /// <summary>
    /// Subscribes to the PreviewExpired event of a Grasshopper document object.
    /// </summary>
    /// <param name="documentObject">
    /// The document object to subscribe to
    /// .</param>
    private void HookPreviewExpired(IGH_DocumentObject documentObject)
    {
        documentObject.ObjectChanged += this.OnGrasshopperObjectChanged;
    }

    /// <summary>
    /// Unsubscribes from the PreviewExpired event of a Grasshopper document object.
    /// </summary>
    /// <param name="documentObject">
    /// The document object to unsubscribe from.
    /// </param>
    private void UnhookPreviewExpired(IGH_DocumentObject documentObject)
    {
        documentObject.ObjectChanged -= this.OnGrasshopperObjectChanged;
    }

    /// <summary>
    /// Handles the ObjectChanged event for a Grasshopper document object.
    /// </summary>
    private void OnGrasshopperObjectChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
    {
        if (e.Type == GH_ObjectEventType.Preview)
        {
            this.PreviewExpired?.Invoke(this,
                new GrasshopperObjectModifiedEventArgs(sender));
        }
    }

    /// <summary>
    /// Subscribes to events in the specified Grasshopper document.
    /// </summary>
    /// <param name="document">
    /// The Grasshopper document to subscribe to.
    /// </param>
    private void AddDocumentSubscriptions(GH_Document document)
    {
        document.ObjectsAdded += this.OnObjectsAdded;
        document.ObjectsDeleted += this.OnObjectsDeleted;
        document.SolutionEnd += this.OnSolutionEnd;

        foreach (var ghDocumentObject in document.Objects)
        {
            this.HookPreviewExpired(ghDocumentObject);
        }

        _selectionTracker = new GrasshopperSelectionTracker(document);
        _selectionTracker.ObjectsSelected += this.OnObjectsSelected;
        _selectionTracker.ObjectsDeselected += this.OnObjectsDeselected;
    }

    /// <summary>
    /// Removes subscriptions to events in the current Grasshopper document.
    /// </summary>
    private void RemoveDocumentSubscriptions()
    {
        if (this.ActiveDoc == null) return;

        this.ActiveDoc.ObjectsAdded -= this.OnObjectsAdded;
        this.ActiveDoc.ObjectsDeleted -= this.OnObjectsDeleted;
        this.ActiveDoc.SolutionEnd -= this.OnSolutionEnd;

        foreach (var obj in this.ActiveDoc.Objects)
        {
            this.UnhookPreviewExpired(obj);
        }

        if (_selectionTracker == null) return;

        _selectionTracker.ObjectsSelected -= this.OnObjectsSelected;
        _selectionTracker.ObjectsDeselected -= this.OnObjectsDeselected;

        _selectionTracker.Dispose();
        _selectionTracker = null;
    }

    /// <summary>
    /// Triggers the preview expired event when the attributes of a Grasshopper document
    /// object change, for example when the component is selected or deselected. This
    /// ensures that the preview geometry is updated to reflect the selection state of
    /// the component.
    /// </summary>
    private void OnObjectsSelected(object sender, IGrasshopperSelectionEventArgs e)
    {
        this.ComponentSelectionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Triggers the preview expired event when the attributes of a Grasshopper document
    /// object change, for example when the component is selected or deselected. This
    /// ensures that the preview geometry is updated to reflect the selection state of
    /// the component.
    /// </summary>
    private void OnObjectsDeselected(object sender, IGrasshopperSelectionEventArgs e)
    {
        this.ComponentSelectionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the event when a Grasshopper solution ends, this triggers the recalculation
    /// of the autocad previews.
    /// </summary>
    private void OnSolutionEnd(object sender, GH_SolutionEventArgs e)
    {
        this.RaisePreviewExpired(e.Document);
    }

    /// <summary>
    /// Raises <see cref="PreviewExpired"/> for every object in <paramref name="document"/>
    /// whose preview is shown.
    /// </summary>
    private void RaisePreviewExpired(GH_Document document)
    {
        foreach (var ghDocumentObject in document.Objects)
        {
            if (ghDocumentObject is not IGH_PreviewObject { Hidden: false })
                continue;

            this.PreviewExpired?.Invoke(this,
                new GrasshopperObjectModifiedEventArgs(ghDocumentObject));
        }
    }

    /// <summary>
    /// Handles the event when the active Grasshopper document changes.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The event data.
    /// </param>
    /// <remarks>
    /// Grasshopper raises no <c>ObjectsDeleted</c> event for the objects of a document it
    /// closes or switches away from, so <see cref="ActiveDocumentChanged"/> is raised for
    /// their previews to be cleared. The new document's previews are then requested
    /// straight away, as a document which has already been solved, such as one switched
    /// back to, raises no <c>SolutionEnd</c> to request them.
    /// </remarks>
    private void OnDocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
    {
        this.RemoveDocumentSubscriptions();

        this.ActiveDoc = e.NewDocument;

        // The editor exists by the time it shows a document.
        _windowManager.TryAttach();

        this.ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);

        if (this.ActiveDoc != null)
        {
            this.AddDocumentSubscriptions(this.ActiveDoc);

            this.RaisePreviewExpired(this.ActiveDoc);
        }
    }

    /// <summary>
    /// Validates that the Grasshopper library is loaded into the Grasshopper component server.
    /// </summary>
    /// <param name="startUpLogger">
    /// The logger to record validation messages.
    /// </param>
    public void ValidateGrasshopperLibrary(IStartUpLogger startUpLogger)
    {
        this.LoadGrasshopper(startUpLogger);
    }

    /// <summary>
    /// Recomputes the Grasshopper solution in the active Grasshopper document.
    /// </summary>
    public void RecomputeSolution()
    {
        if (this.ActiveDoc is null) return;

        this.ActiveDoc.NewSolution(true);
    }

    /// <summary>
    /// Disables the Grasshopper solver, preventing solutions from being recomputed.
    /// </summary>
    public void DisableSolver()
    {
        Grasshopper.Kernel.GH_Document.EnableSolutions = false;
    }

    /// <summary>
    /// Enables the Grasshopper solver, allowing solutions to be recomputed.
    /// </summary>
    public void EnableSolver()
    {
        Grasshopper.Kernel.GH_Document.EnableSolutions = true;
    }

    /// <summary>
    /// Clears volatile data from a parameter without disposing the underlying objects.
    /// This prevents RhinoCore from accessing disposed memory during its own disposal.
    /// </summary>
    private void ClearParamData(IGH_Param param)
    {
        try
        {
            param.ClearData();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to clear param data: {ex.Message}");
        }
    }

    /// <summary>
    /// Shuts down the Grasshopper instance, releasing resources and removing
    /// subscriptions.
    /// </summary>
    public void Shutdown()
    {
        System.Diagnostics.Debug.WriteLine("=== GrasshopperInstance.Shutdown() START ===");

        this.RemoveDocumentSubscriptions();

        _windowManager.DisplayStateChanged -= this.OnEditorDisplayStateChanged;

        _windowManager.Dispose();

        if (_activeCanvas != null)
        {
            _activeCanvas.DocumentChanged -= this.OnDocumentChanged;
            _activeCanvas = null;
        }

        Grasshopper.Instances.CanvasCreated -= this.OnCanvasCreated;

        System.Diagnostics.Debug.WriteLine("=== GrasshopperInstance.Shutdown() END ===");
    }
}

