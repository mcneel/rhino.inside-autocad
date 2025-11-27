using Autodesk.AutoCAD.DatabaseServices;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Runtime.InProcess;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.Applications;

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