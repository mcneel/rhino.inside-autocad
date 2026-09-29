namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the dialog which tells the user their Rhino is too old and asks them to update
/// it.
/// </summary>
/// <remarks>
/// Shown during AutoCAD's startup, in place of loading the plugin, when every installed
/// Rhino is older than the minimum RhinoCommon version. Like the version selection dialog
/// it blocks its caller until the user closes it.
/// </remarks>
/// <seealso cref="IRhinoVersionSelection"/>
/// <seealso cref="IRhinoVersionDialogManager"/>
public interface IRhinoUpdateDialogManager
{
    /// <summary>
    /// Shows the dialog and blocks until the user closes it.
    /// </summary>
    /// <param name="message">The message explaining why Rhino must be updated.</param>
    /// <param name="downloadUrl">The page the update button opens.</param>
    void Show(string message, string downloadUrl);
}
