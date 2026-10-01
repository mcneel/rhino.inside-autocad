using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperWindowManager"/>
/// <remarks>
/// Watches the managed editor, <see cref="Grasshopper.Instances.DocumentEditor"/>, through
/// its own <see cref="Control.VisibleChanged"/> and <see cref="Control.Resize"/> events, so
/// no window subclassing, P/Invoke or polling is needed.
/// <para>
/// The editor does not exist yet when its canvas is created, so <see cref="ScheduleAttach"/>
/// looks for it once AutoCAD is next idle, and <see cref="TryAttach"/> can be called
/// wherever the editor is known to exist. Both are idempotent, and a replaced editor is
/// let go in favour of the new one.
/// </para>
/// </remarks>
public class GrasshopperWindowManager : IGrasshopperWindowManager
{
    /// <summary>
    /// The editor whose events are subscribed to, or null while none has been found.
    /// </summary>
    private Form? _editor;

    /// <summary>
    /// True while a look for the editor is waiting for AutoCAD to be idle.
    /// </summary>
    private bool _attachPending;

    private bool _disposed;

    /// <summary>
    /// Contains failures raised by <see cref="DisplayStateChanged"/> handlers, which are
    /// reached from the editor's window procedure or the AutoCAD idle loop.
    /// </summary>
    private readonly IAutocadGuard _autocadGuard = new AutocadGuard();

    /// <inheritdoc />
    public event EventHandler? DisplayStateChanged;

    /// <inheritdoc />
    public bool IsMinimised { get; private set; }

    /// <inheritdoc />
    public bool IsHidden { get; private set; }

    /// <summary>
    /// Writes a message to the debug output under the manager's name.
    /// </summary>
    private static void Trace(string message)
    {
        System.Diagnostics.Debug.WriteLine($"GrasshopperWindowManager: {message}");
    }

    /// <summary>
    /// Looks for the editor once AutoCAD is next idle, unless a look is already pending.
    /// </summary>
    /// <remarks>
    /// Used when the canvas is created, which happens before the editor holding it is
    /// assigned to <see cref="Grasshopper.Instances.DocumentEditor"/>.
    /// </remarks>
    public void ScheduleAttach()
    {
        if (_attachPending || _disposed) return;

        _attachPending = true;

        Application.Idle += this.OnIdle;
    }

    /// <summary>
    /// Looks for the editor, once, after <see cref="ScheduleAttach"/>.
    /// </summary>
    private void OnIdle(object? sender, EventArgs e)
    {
        Application.Idle -= this.OnIdle;

        _attachPending = false;

        _autocadGuard.Run(() => this.TryAttach(), nameof(this.OnIdle));
    }

    /// <summary>
    /// Subscribes to the events of the editor <see cref="Grasshopper.Instances.DocumentEditor"/>
    /// returns, unless it is already subscribed to, letting go of a previous editor that has
    /// been replaced, and reads its display state.
    /// </summary>
    /// <returns>True when an editor is attached once this returns.</returns>
    /// <remarks>
    /// Reading <see cref="Grasshopper.Instances.DocumentEditor"/> never creates the editor.
    /// </remarks>
    public bool TryAttach()
    {
        if (_disposed) return false;

        Form? editor = Grasshopper.Instances.DocumentEditor;

        if (editor == null || editor.IsDisposed)
        {
            Trace("attach: editor not created yet");

            return false;
        }

        if (ReferenceEquals(editor, _editor)) return true;

        this.Detach("replaced");

        editor.VisibleChanged += this.OnEditorVisibleChanged;
        editor.Resize += this.OnEditorResize;
        editor.FormClosed += this.OnEditorFormClosed;
        editor.Disposed += this.OnEditorDisposed;

        _editor = editor;

        Trace($"attach: editor found (Visible {editor.Visible}, WindowState {editor.WindowState})");

        this.UpdateDisplayState("attach");

        return true;
    }

    /// <summary>
    /// Unsubscribes from the editor's events, if one is attached, and reports neither
    /// state, as there is no editor to be out of sight.
    /// </summary>
    /// <param name="reason">Why the editor is let go, for the debug output.</param>
    private void Detach(string reason)
    {
        var editor = _editor;

        if (editor == null) return;

        editor.VisibleChanged -= this.OnEditorVisibleChanged;
        editor.Resize -= this.OnEditorResize;
        editor.FormClosed -= this.OnEditorFormClosed;
        editor.Disposed -= this.OnEditorDisposed;

        _editor = null;

        Trace($"detach: {reason}");

        this.SetDisplayState(false, false, "detach");
    }

    /// <summary>
    /// Reads whether the attached editor is minimised or hidden from the editor itself.
    /// </summary>
    /// <param name="source">What led to the read, for the debug output.</param>
    private void UpdateDisplayState(string source)
    {
        var editor = _editor;

        if (editor == null || editor.IsDisposed)
        {
            this.SetDisplayState(false, false, source);

            return;
        }

        var isMinimised = editor.WindowState == FormWindowState.Minimized;

        var isHidden = editor.Visible == false;

        this.SetDisplayState(isMinimised, isHidden, source);
    }

    /// <summary>
    /// Sets <see cref="IsMinimised"/> and <see cref="IsHidden"/>, raising
    /// <see cref="DisplayStateChanged"/> when either changes.
    /// </summary>
    /// <param name="isMinimised">Whether the editor is minimised.</param>
    /// <param name="isHidden">Whether the editor is hidden.</param>
    /// <param name="source">What led to the state being set, for the debug output.</param>
    private void SetDisplayState(bool isMinimised, bool isHidden, string source)
    {
        if (this.IsMinimised == isMinimised && this.IsHidden == isHidden) return;

        Trace($"DisplayStateChanged ({source}): " +
              $"isMinimised {this.IsMinimised} -> {isMinimised}, isHidden {this.IsHidden} -> {isHidden}");

        this.IsMinimised = isMinimised;

        this.IsHidden = isHidden;

        _autocadGuard.Run(() => this.DisplayStateChanged?.Invoke(this, EventArgs.Empty),
            nameof(this.DisplayStateChanged));
    }

    /// <summary>
    /// Handles the editor being shown or hidden, which closing and reopening it does.
    /// </summary>
    private void OnEditorVisibleChanged(object? sender, EventArgs e)
    {
        Trace($"VisibleChanged: Visible {_editor?.Visible}");

        this.UpdateDisplayState("VisibleChanged");
    }

    /// <summary>
    /// Handles the editor being resized, which minimising and restoring it does.
    /// </summary>
    private void OnEditorResize(object? sender, EventArgs e)
    {
        Trace($"Resize: WindowState {_editor?.WindowState}");

        this.UpdateDisplayState("Resize");
    }

    /// <summary>
    /// Lets go of the editor when it is closed for good, rather than hidden.
    /// </summary>
    private void OnEditorFormClosed(object? sender, FormClosedEventArgs e)
    {
        this.Detach("FormClosed");
    }

    /// <summary>
    /// Lets go of the editor when it is disposed.
    /// </summary>
    private void OnEditorDisposed(object? sender, EventArgs e)
    {
        this.Detach("Disposed");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;

        if (_attachPending)
        {
            Application.Idle -= this.OnIdle;

            _attachPending = false;
        }

        this.DisplayStateChanged = null;

        this.Detach("disposed");

        _disposed = true;
    }
}
