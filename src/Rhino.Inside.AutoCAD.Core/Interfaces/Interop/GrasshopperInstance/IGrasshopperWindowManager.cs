namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Watches the Grasshopper editor window and reports when it is minimised, restored,
/// hidden or shown.
/// </summary>
/// <remarks>
/// <para>
/// Used to hide the Grasshopper preview while the editor is out of sight, without changing
/// the user's own preview setting.
/// </para>
/// <para>
/// Grasshopper creates its editor lazily, the first time it is opened, and closing it hides
/// it in place of destroying it. Until the editor exists neither state is reported, so
/// nothing is held back on first launch.
/// </para>
/// <para>
/// <b>Lifecycle:</b> owned by <see cref="IGrasshopperInstance"/>, which disposes it when
/// Grasshopper shuts down. Disposing it lets go of the editor.
/// </para>
/// </remarks>
public interface IGrasshopperWindowManager : IDisposable
{
    /// <summary>
    /// Raised when <see cref="IsMinimised"/> or <see cref="IsHidden"/> changes.
    /// </summary>
    /// <remarks>
    /// Raised on the thread that owns the editor, which for Rhino.Inside is the AutoCAD
    /// main thread: when the editor is first found, and after it is shown, hidden,
    /// minimised or restored.
    /// </remarks>
    event EventHandler? DisplayStateChanged;

    /// <summary>
    /// True while the Grasshopper editor window is minimised. False before the editor has
    /// been opened.
    /// </summary>
    bool IsMinimised { get; }

    /// <summary>
    /// True while the Grasshopper editor window exists but is hidden, as it is after the
    /// user closes it, which hides the editor in place of destroying it. False before the
    /// editor has been opened.
    /// </summary>
    bool IsHidden { get; }
}
