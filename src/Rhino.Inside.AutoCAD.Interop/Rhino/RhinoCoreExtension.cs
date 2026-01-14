using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using Rhino.Runtime.InProcess;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Launches the Rhino inside instance.
/// </summary>
public class RhinoCoreExtension : IRhinoCoreExtension
{
    private const string _rhinoRegistryKeyPath = ApplicationConstants.RhinoRegistryKeyPath;
    private const string _rhinoInstallPathValueName = ApplicationConstants.RhinoInstallPathValueName;
    private const string _rhinoPluginsFolderValueName = ApplicationConstants.RhinoPluginsFolderValueName;
    private const string _rhinoCommonAssemblyName = ApplicationConstants.RhinoCommonAssemblyName;
    private const string _grasshopperAssemblyName = ApplicationConstants.GrasshopperAssemblyName;
    private const string _grasshopperIoAssemblyName = ApplicationConstants.GrasshopperIOAssemblyName;
    private const string _rhinoCommonDllName = ApplicationConstants.RhinoCommonDllName;
    private const string _grasshopperDllRelativePath = ApplicationConstants.GrasshopperDllRelativePath;
    private const string _grasshopperIoDllRelativePath = ApplicationConstants.GrasshopperIoDllRelativePath;
    private const string _rhinoNoSplashArgument = ApplicationConstants.RhinoNoSplashArgument;
    private const string _rhinoSchemeArgumentFormat = ApplicationConstants.RhinoSchemeArgumentFormat;
    private const string _rhinoInsideSchemeNameFormat = ApplicationConstants.RhinoInsideSchemeNameFormat;
    private const string _rhinoNotInstalledErrorMessage = ApplicationConstants.RhinoNotInstalledErrorMessage;
    private const string _rhinoCoreInitializationFailedErrorMessage = ApplicationConstants.RhinoCoreInitializationFailedErrorMessage;

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
        _rhinoRegistryKeyPath, _rhinoInstallPathValueName, string.Empty
    );

    /// <summary>
    /// Gets the Rhino system directory in the local machines registry.
    /// </summary>
    static readonly string _pluginDir = (string)Microsoft.Win32.Registry.GetValue
    (
        _rhinoRegistryKeyPath, _rhinoPluginsFolderValueName, string.Empty
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

#if DEBUGNET8  || RELEASENET8
            RegisterAssemblyResolver(_rhinoCommonAssemblyName, Path.Combine(_systemDir, "netcore", _rhinoCommonDllName));
            RegisterAssemblyResolver("Rhino.UI", Path.Combine(_systemDir, "netcore", "Rhino.UI.dll"));
            RegisterAssemblyResolver("Mono.Cecil", Path.Combine(_systemDir, "netcore", "Mono.Cecil.dll"));
#else
       RegisterAssemblyResolver(_rhinoCommonAssemblyName, Path.Combine(_systemDir, _rhinoCommonDllName));
         
#endif

            RegisterAssemblyResolver(_grasshopperAssemblyName, Path.Combine(_pluginDir, _grasshopperDllRelativePath));

            RegisterAssemblyResolver(_grasshopperIoAssemblyName, Path.Combine(_pluginDir, _grasshopperIoDllRelativePath));
            RegisterAssemblyResolver("Eto", Path.Combine(_systemDir, "Eto.dll"));

        }
        else
        {
            Instance.ValidationLogger.AddMessage(_rhinoNotInstalledErrorMessage);
        }
    }

    /// <summary>
    /// Registers an assembly resolver for the specified assembly name.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to resolve.</param>
    /// <param name="assemblyPath">The path of the assembly to resolve.</param>
    private static void RegisterAssemblyResolver(string assemblyName, string assemblyPath)
    {
        ResolveEventHandler? resolver = null;

        AppDomain.CurrentDomain.AssemblyResolve += resolver = (_, args) =>
        {
            var requestedAssemblyName = new AssemblyName(args.Name).Name;

            if (requestedAssemblyName != assemblyName)
                return null;

            AppDomain.CurrentDomain.AssemblyResolve -= resolver;

            return Assembly.LoadFrom(assemblyPath);
        };
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
            var schemeName = string.Format(
                _rhinoInsideSchemeNameFormat,
                HostApplicationServices.Current.Product,
                HostApplicationServices.Current.releaseMarketVersion);

            var style = WindowStyle.Hidden;

            var autocadHandle = Autodesk.AutoCAD.ApplicationServices.Core.Application
                .MainWindow.Handle;

            var args = new List<string>()
            {
               _rhinoNoSplashArgument,
                string.Format(_rhinoSchemeArgumentFormat, schemeName)
            };

#if DEBUGNET8  || RELEASENET8
            args.Add("/netcore");
#else
        args.Add("/netfx");
#endif

            _rhinoCore ??= new RhinoCore(args.ToArray(), style, autocadHandle);

            var mainWindow = RhinoApp.MainWindowHandle();

            this.WindowManager.SetWindow(mainWindow);

            RhinoApp.Closing += this.OnClosing;

        }
        catch
        {
            this.ValidationLogger.AddMessage(_rhinoCoreInitializationFailedErrorMessage);

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
        RhinoApp.Exit(true);
        _rhinoCore?.Dispose();
    }
}