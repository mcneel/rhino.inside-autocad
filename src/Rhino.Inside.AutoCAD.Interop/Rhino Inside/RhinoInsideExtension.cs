using Autodesk.AutoCAD.DatabaseServices;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.ApplicationSettings;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Runtime.InProcess;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Launches the Rhino inside instance.
/// </summary>
public class RhinoInsideExtension
{
    private static RhinoCore? _rhinoCore;

    /// <summary>
    /// True if Rhino is installed otherwise false.
    /// </summary>
    private static readonly bool _rhinoInstallDirectoryExists;

    /// <summary>
    /// The <see cref="RhinoInsideExtension"/> singleton instance.
    /// </summary>
    public static RhinoInsideExtension Instance { get; }

    /// <summary>
    /// The active <see cref="RhinoDoc"/>.
    /// </summary>
    public RhinoDoc? ActiveDoc { get; private set; }

    /// <summary>
    /// The <see cref="IValidationLogger"/>.
    /// </summary>
    public IValidationLogger ValidationLogger { get; }

    /// <summary>
    /// Gets the Rhino system directory in the local machines registry.
    /// </summary>
    static readonly string _systemDir = (string)Microsoft.Win32.Registry.GetValue
    (
        @"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", string.Empty
    );

    /// <summary>
    /// Constructs a new <see cref="RhinoInsideExtension"/> instance.
    /// </summary>
    private RhinoInsideExtension()
    {
        this.ValidationLogger = new ValidationLogger();
    }

    /// <summary>
    /// Uses assembly resolver to load the Rhino assembly once per app domain.
    /// </summary>
    static RhinoInsideExtension()
    {
        Instance = new RhinoInsideExtension();

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
    /// Converts a <see cref="UnitSystem"/> to a <see cref="UnitSystem"/>.
    /// </summary>
    private UnitSystem GetRhinoUnitSystem(UnitSystem unitSystem)
    {
        var unitSystemResult = Enum.TryParse(unitSystem.ToString(), out UnitSystem rhinoUnitSystem);

        return unitSystemResult ? rhinoUnitSystem : UnitSystem.None;
    }

    /// <summary>
    /// Initializes the Rhino Inside instance and returns true if it has successfully
    /// launched otherwise false indicating a failure.
    /// </summary>
    public void Initialize(UnitSystem internalUnits, IApplicationDirectories applicationDirectories, RhinoInsideMode mode)
    {
        if (_rhinoInstallDirectoryExists & _rhinoCore == null)
        {
            try
            {
                var schemeName =
                    $"Inside-{HostApplicationServices.Current.Product}-{HostApplicationServices.Current.releaseMarketVersion}";

                var style = mode == RhinoInsideMode.Headless ? WindowStyle.Hidden : WindowStyle.Normal;

                var args = new[]
                {
                    $"/scheme={schemeName}"
                };

                if (mode != RhinoInsideMode.WithSplash)
                {
                    args = new[] { "/nosplash" }.Concat(args).ToArray();
                }

                _rhinoCore ??= new RhinoCore(args, style);

                var rhinoDoc = mode == RhinoInsideMode.Headless
                    ? RhinoDoc.CreateHeadless($"{applicationDirectories.Resources}Large Objects - Millimeters.3dm")
                    : RhinoDoc.ActiveDoc;

                FileSettings.AutoSaveEnabled = false;

                rhinoDoc.ModelUnitSystem = this.GetRhinoUnitSystem(internalUnits);

                this.ActiveDoc = rhinoDoc;
            }
            catch
            {
                this.ValidationLogger.AddMessage("Failed to initialize Rhino.Inside.");

                throw;
            }
        }
    }

    /// <summary>
    /// The steps to take to shutdown this plugin.
    /// </summary>
    public void Shutdown()
    {
        this.ActiveDoc?.Dispose();

        _rhinoCore?.Dispose();
    }
}

