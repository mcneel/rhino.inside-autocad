using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class containing message strings, such as warnings, for
/// UI-bound purposes.
/// </summary>
public class MessageConstants
{
    /// <summary>
    /// The error message displayed when Rhino fails to start.
    /// </summary>
    public const string RhinoStartFailureMessage = "Unable to start Rhino, ensure you have Rhino installed and a valid licence. To validate this  \n" +
                                                   "try running the Rhino application outside of AutoCAD and ensure it is  fully working. \n" +
                                                   "If this issue persists contact us at support@bimorph.com \n {0}";

    /// <summary>
    /// A Void message.
    /// </summary>
    public const string Void = "VOID";

    /// <summary>
    /// An error message for the <see cref="IAutoCadInstance"/> for when a readonly document
    /// is used.
    /// </summary>
    public const string ReadOnlyNotSupported = "Warning: Read only documents are not supported. Open your file with Write enabled to run the application.";

    /// <summary>
    /// An error message for the <see cref="IAutoCadInstance"/> for when unsupported units
    /// are used.
    /// </summary>
    public const string FileUnitsNotSupported = "Warning: unsupported document file units ({0}). Set a valid metric or imperial unit system and try again.";

    /// <summary>
    /// An error message when the LoadGHA method is not found via reflection.
    /// </summary>
    public const string LoadGhaMethodNotFound =
        "GH_ComponentServer.LoadGHA could not be resolved. The Grasshopper SDK this Rhino " +
        "ships has changed shape and the Rhino.Inside.AutoCAD components cannot be registered.";

    /// <summary>
    /// Diagnostic recording the Rhino installations found on this machine.
    /// </summary>
    /// <remarks>
    /// The placeholder receives the formatted installations. This is the first place to look
    /// when a Rhino version the user expects to be offered is missing from the startup
    /// dialog.
    /// </remarks>
    /// <seealso cref="RhinoInstallationDescriptionFormat"/>
    public const string RhinoInstallationsFoundFormat = "Rhino installations found: {0}";

    /// <summary>
    /// The format of a single located Rhino installation.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the display name, the registry version key, the RhinoCommon file
    /// version, and the resolved RhinoCommon path.
    /// </remarks>
    /// <seealso cref="RhinoInstallationsFoundFormat"/>
    public const string RhinoInstallationDescriptionFormat = "{0} [{1}] RhinoCommon {2} at {3}";

    /// <summary>
    /// Diagnostic recording the installations passed over for being older than the minimum
    /// RhinoCommon version.
    /// </summary>
    /// <remarks>
    /// The first placeholder receives the formatted installations, the second the minimum
    /// version. Explains why an installed Rhino is missing from the startup dialog.
    /// </remarks>
    /// <seealso cref="RhinoInstallationDescriptionFormat"/>
    public const string RhinoOutdatedInstallationsSkippedFormat =
        "Rhino installations skipped for being older than {1}: {0}";

    /// <summary>
    /// Stands in for the installation list when no Rhino version was found.
    /// </summary>
    /// <seealso cref="RhinoInstallationsFoundFormat"/>
    public const string NoRhinoInstallationsFound = "none";

    /// <summary>
    /// The message reported when the component libraries fail to register, leaving
    /// Grasshopper running without the Rhino.Inside.AutoCAD tabs.
    /// </summary>
    /// <remarks>
    /// The placeholder receives the exception message. The failure happens inside a
    /// Grasshopper event, where an unreported exception would otherwise be invisible.
    /// </remarks>
    public const string GrasshopperLibraryLoadFailedFormat =
        "Rhino.Inside.AutoCAD could not register its Grasshopper components, so its tabs " +
        "will be missing from the canvas. See the log in " +
        "%AppData%\\RhinoInsideAutocad\\Logs for details. {0}";

    /// <summary>
    /// Diagnostic recording which Grasshopper the plugin bound to, and where its own
    /// component libraries are being loaded from.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the Grasshopper assembly full name, its location, and the
    /// Rhino.Inside.AutoCAD assemblies folder. The location is the quickest confirmation
    /// that the assembly resolvers bound to the intended Rhino installation.
    /// </remarks>
    public const string GrasshopperHostDiagnosticFormat =
        "Grasshopper host: {0}\n  located at: {1}\n  component libraries folder: {2}";

    /// <summary>
    /// Diagnostic recording the resolved LoadGHA overload.
    /// </summary>
    /// <remarks>The placeholder receives the method signature.</remarks>
    public const string LoadGhaResolvedFormat = "Resolved GH_ComponentServer.{0}";

    /// <summary>
    /// Diagnostic recording a component library about to be loaded.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the library path, whether the file exists, and the
    /// <c>GH_ExternalFileType</c> Grasshopper infers for it. The file type is derived from
    /// the extension, so a library shipped as <c>.dll</c> rather than <c>.gha</c> can be
    /// classified as Unknown rather than Assembly.
    /// </remarks>
    public const string GrasshopperLibraryDiagnosticFormat =
        "Loading component library {0} (exists: {1}, Grasshopper file type: {2})";

    /// <summary>
    /// Diagnostic recording what LoadGHA reported.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the return value and the library file name. LoadGHA reports
    /// refusal by returning false rather than by throwing, so discarding this hides the
    /// most common failure.
    /// </remarks>
    public const string LoadGhaReturnedFormat = "LoadGHA returned {0} for {1}";

    /// <summary>
    /// Diagnostic recording the loading exceptions Grasshopper collected.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the exception count and the formatted entries. Grasshopper
    /// records why it rejected a library here instead of throwing.
    /// </remarks>
    public const string GrasshopperLoadingExceptionsFormat =
        "Grasshopper recorded {0} loading exception(s):\n{1}";

    /// <summary>
    /// The format of a single Grasshopper loading exception entry.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the type, name and message of a <c>GH_LoadingException</c>,
    /// which does not override ToString.
    /// </remarks>
    public const string GrasshopperLoadingExceptionFormat = "  [{0}] {1}: {2}";

    /// <summary>
    /// Diagnostic recording that a component library exposed its types successfully.
    /// </summary>
    /// <remarks>Placeholders receive the assembly full name and the type count.</remarks>
    public const string GrasshopperLibraryTypesLoadedFormat =
        "{0} exposed {1} types";

    /// <summary>
    /// The message reported when a component library cannot expose its types.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the assembly full name and the distinct loader exception
    /// messages. This is the failure that leaves the canvas open with no tabs and no error:
    /// Grasshopper reflects over the assembly to discover components, and registers nothing
    /// when that throws. The loader exceptions name the member that moved between Rhino
    /// versions.
    /// </remarks>
    public const string GrasshopperLibraryTypeLoadFailedFormat =
        "{0} could not expose its types, so Grasshopper will register no components from " +
        "it. This normally means it was built against a different Rhino SDK than the one " +
        "loaded. Loader exceptions:\n{1}";

    /// <summary>
    /// Diagnostic recording the outcome of registering a component library.
    /// </summary>
    /// <remarks>
    /// Placeholders receive the library file name, the registered library count before and
    /// after, and whether the library is registered afterwards.
    /// </remarks>
    public const string GrasshopperLibraryRegisteredFormat =
        "Registered {0}: library count {1} -> {2}, present: {3}";

    /// <summary>
    /// Diagnostic recording that a component library was already registered.
    /// </summary>
    /// <remarks>The placeholder receives the library file name.</remarks>
    public const string GrasshopperLibraryAlreadyRegisteredFormat =
        "{0} is already registered with the component server; skipping";

    /// <summary>
    /// An error message when Grasshopper fails to initialize.
    /// </summary>
    public const string GrasshopperInitializationFailed = "Failed to initialize Grasshopper";

    /// <summary>
    /// An error message when the GH_AutocadGeometricGoo type cannot be found.
    /// </summary>
    public const string GooBaseTypeNotFound = "GH_AutocadGeometricGoo type not found. Ensure Rhino.Inside.Autocad.GrasshopperLibrary is loaded.";

    /// <summary>
    /// The registry key for accessing user profile information in AutoCAD, used to
    /// determine if the current profile is a pure AutoCAD profile or not.
    /// </summary>
    public const string UserRegistryProductRootKeyProfiles = "\\Profiles";

    /// <summary>
    /// The registry key name used to identify if the current AutoCAD profile is
    /// a pure AutoCAD profile.
    /// </summary>
    public const string IsPureAcadProfile = "IsPureAcadProfile";
}