using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class containing message strings, such as warnings, for
/// UI-bound purposes.
/// </summary>
public class MessageConstants
{
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

}
