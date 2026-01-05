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
    public const string RhinoStartFailureMessage = "Unable to start Rhino, ensure you have Rhino 8 installed and a valid licence. To validate this  \n" +
                                                   "try running the Rhino 8 application outside of AutoCAD and ensure it is  fully working. \n" +
                                                   "If this issue persists contact us at support@bimorph.com";

    /// <summary>
    /// A Void message.
    /// </summary>
    public const string Void = "VOID";

    /// <summary>
    /// An error message for the <see cref="IAutoCadInstance"/> for when an unsaved document
    /// is used.
    /// </summary>
    public const string UnsavedNotSupported = "Warning: Unsaved documents are not supported. Save your file to run the application.";

    /// <summary>
    /// An error message for the <see cref="IAutoCadInstance"/> for when a readonly document
    /// is used.
    /// </summary>
    public const string ReadOnlyNotSupported = "Warning: Read only documents are not supported. Open your file with Write enabled to run the application.";

    /// <summary>
    /// An error message for the <see cref="IAutoCadInstance"/> for when unsupported units
    /// are used.
    /// </summary
    public const string FileUnitsNotSupported = "Warning: unsupported document file units ({0}). Set a valid metric or imperial unit system and try again.";

    /// <summary>
    /// An error message when the LoadGHA method is not found via reflection.
    /// </summary>
    public const string LoadGhaMethodNotFound = "LoadGHA method not found";

    /// <summary>
    /// An error message when Grasshopper fails to initialize.
    /// </summary>
    public const string GrasshopperInitializationFailed = "Failed to initialize Grasshopper";

    /// <summary>
    /// An error message when the GH_AutocadGeometricGoo type cannot be found.
    /// </summary>
    public const string GooBaseTypeNotFound = "GH_AutocadGeometricGoo type not found. Ensure Rhino.Inside.Autocad.GrasshopperLibrary is loaded.";
}
