namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Interface to manage the Rhino main window visibility, focus, and activation monitoring.
/// </summary>
/// <remarks>
/// <para>
/// This interface provides methods to control the visibility and focus state of the Rhino
/// main window when running Rhino inside another host application (e.g., AutoCAD).
/// </para>
/// <para>
/// <b>Window Activation Monitoring:</b>
/// </para>
/// <para>
/// The interface supports window activation monitoring via a Windows CBT
/// (Computer Based Training) hook. A CBT hook is a low-level Windows mechanism that
/// receives notifications about system events before they occur. The <c>HCBT_ACTIVATE</c>
/// notification (hook code 5) is sent by Windows when any window is about to be activated
/// (receive keyboard focus).
/// </para>
/// <para>
/// This pattern is similar to Rhino.Inside.Revit's <c>ComputerBasedTrainingHook</c> implementation,
/// which intercepts window activation to conditionally show or hide the Rhino window based on
/// whether Rhino is waiting for user input (e.g., during a <c>RhinoGet</c> operation).
/// </para>
/// <para>
/// When the Rhino window is about to be activated and Rhino is in a <c>RhinoGet</c> operation,
/// the window is automatically shown. This enables "headless" operation where the window
/// remains hidden until user interaction is actually required.
/// </para>
/// <para>
/// <b>Thread Requirements:</b>
/// </para>
/// <para>
/// CBT hooks must be installed on a thread that has a Windows message pump. When running
/// inside AutoCAD, the main UI thread satisfies this requirement. The hook callback executes
/// on the same thread that installed the hook.
/// </para>
/// <para>
/// <b>Lifecycle:</b>
/// </para>
/// <para>
/// Implementations must properly clean up the CBT hook when disposed. Failing to uninstall
/// the hook before the application exits can cause crashes or undefined behavior. The
/// <see cref="IDisposable.Dispose"/> method should always uninstall any active hooks.
/// </para>
/// </remarks>
/// <example>
/// Typical usage pattern:
/// <code>
/// // During initialization
/// windowManager.SetWindow(RhinoApp.MainWindowHandle());
/// windowManager.InstallActivationHook();
///
/// // During shutdown
/// windowManager.Dispose();
/// </code>
/// </example>
public interface IRhinoWindowManager : IDisposable
{
    /// <summary>
    /// Raised when <see cref="IsMinimised"/> or <see cref="IsHidden"/> changes.
    /// </summary>
    /// <remarks>
    /// Raised on the thread that owns the main window: when the window is set, after this
    /// manager shows, hides or restores it, from the activation hook installed by
    /// <see cref="InstallActivationHook"/> just before a minimise or restore, and once
    /// AutoCAD is idle after the hook sees a close request or an activation, which is how a
    /// window Rhino hides in place of closing is noticed.
    /// </remarks>
    event EventHandler? DisplayStateChanged;

    /// <summary>
    /// True while the Rhino main window is minimised.
    /// </summary>
    /// <remarks>
    /// Tracked from the minimise and restore requests the activation hook sees and from the
    /// window's own state, so it is only kept up to date while the hook is installed.
    /// </remarks>
    bool IsMinimised { get; }

    /// <summary>
    /// True while the Rhino main window is hidden, as it is when Rhino is created and after
    /// the user closes it, which Rhino.Inside answers by hiding the window.
    /// </summary>
    /// <remarks>
    /// Read from the window when it is set and on the same occasions as
    /// <see cref="IsMinimised"/>. False while no window has been set.
    /// </remarks>
    bool IsHidden { get; }

    /// <summary>
    /// Sets the Rhino main window handle that this manager will control.
    /// </summary>
    /// <param name="mainWindow">
    /// The native window handle (HWND) of the Rhino main window.
    /// Pass <see cref="IntPtr.Zero"/> to clear the current window reference.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method must be called before any other window operations can take effect.
    /// Typically called during Rhino initialization after <c>RhinoCore</c> is created.
    /// </para>
    /// <para>
    /// The window handle is obtained from <c>RhinoApp.MainWindowHandle()</c>.
    /// </para>
    /// <para>
    /// <b>Prerequisites:</b> None. This is typically the first method called on the manager.
    /// </para>
    /// <para>
    /// <b>Thread Safety:</b> This method should be called from the main UI thread.
    /// Calling from other threads may cause race conditions with the CBT hook.
    /// </para>
    /// </remarks>
    void SetWindow(IntPtr mainWindow);

    /// <summary>
    /// Shows the Rhino main window if one has been set via <see cref="SetWindow"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses the Win32 <c>ShowWindow</c> API with <c>SW_SHOW</c> (5) to make the window visible.
    /// This activates the window and displays it in its current size and position.
    /// </para>
    /// <para>
    /// If no window has been set (i.e., <see cref="SetWindow"/> was never called or was
    /// called with <see cref="IntPtr.Zero"/>), this method does nothing.
    /// </para>
    /// <para>
    /// <b>Win32 API:</b> <c>ShowWindow(hwnd, SW_SHOW)</c>
    /// </para>
    /// </remarks>
    void ShowWindow();

    /// <summary>
    /// Shows the Rhino main window without activating it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses the Win32 <c>ShowWindow</c> API with <c>SW_SHOWNA</c> (8) to make the window visible
    /// without changing the currently active window. This is useful when you need to make the
    /// window visible but don't want to steal focus from another application.
    /// </para>
    /// <para>
    /// This method is particularly useful when called from within a CBT hook callback during
    /// window activation. Using <see cref="ShowWindow"/> (which activates) from within the hook
    /// could cause re-entrancy issues since activation triggers the hook again.
    /// </para>
    /// <para>
    /// If no window has been set, this method does nothing.
    /// </para>
    /// <para>
    /// <b>Win32 API:</b> <c>ShowWindow(hwnd, SW_SHOWNA)</c>
    /// </para>
    /// </remarks>
    void ShowWindowNoActivate();

    /// <summary>
    /// Hides the Rhino main window if one has been set via <see cref="SetWindow"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses the Win32 <c>ShowWindow</c> API with <c>SW_HIDE</c> (0) to hide the window.
    /// The window remains in memory and retains its state; it can be shown again
    /// with <see cref="ShowWindow"/>.
    /// </para>
    /// <para>
    /// If no window has been set, this method does nothing.
    /// </para>
    /// <para>
    /// <b>Win32 API:</b> <c>ShowWindow(hwnd, SW_HIDE)</c>
    /// </para>
    /// </remarks>
    void HideWindow();

    /// <summary>
    /// Brings the Rhino main window to the front and gives it keyboard focus.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method performs three Win32 operations in sequence to ensure the window
    /// is fully visible and has focus:
    /// </para>
    /// <list type="number">
    /// <item>
    ///   <description>
    ///     <b>Restore:</b> If the window is minimized, restores it to its previous size
    ///     and position using <c>ShowWindow(hwnd, SW_RESTORE)</c>.
    ///   </description>
    /// </item>
    /// <item>
    ///   <description>
    ///     <b>Z-Order:</b> Brings the window to the top of the Z-order (above all other
    ///     windows) using <c>BringWindowToTop(hwnd)</c>.
    ///   </description>
    /// </item>
    /// <item>
    ///   <description>
    ///     <b>Focus:</b> Sets the window as the foreground window (gives it keyboard focus)
    ///     using <c>SetForegroundWindow(hwnd)</c>.
    ///   </description>
    /// </item>
    /// </list>
    /// <para>
    /// If no window has been set, this method does nothing.
    /// </para>
    /// <para>
    /// <b>Note:</b> Windows has restrictions on which applications can call
    /// <c>SetForegroundWindow</c>. Generally, only the foreground application can
    /// set another window as foreground. However, since this runs within the AutoCAD
    /// process (which is typically foreground), this should work correctly.
    /// </para>
    /// </remarks>
    void BringToFront();

    /// <summary>
    /// Installs a Windows CBT (Computer Based Training) hook to monitor window activation events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What is a CBT Hook?</b>
    /// </para>
    /// <para>
    /// A CBT hook is a Windows mechanism originally designed for Computer Based Training
    /// applications. It receives notifications about window management events <i>before</i>
    /// they occur, including:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>HCBT_ACTIVATE</c> (5): A window is about to be activated</description></item>
    /// <item><description><c>HCBT_CREATEWND</c> (3): A window is about to be created</description></item>
    /// <item><description><c>HCBT_DESTROYWND</c> (4): A window is about to be destroyed</description></item>
    /// <item><description><c>HCBT_MINMAX</c> (1): A window is about to be minimized/maximized</description></item>
    /// </list>
    /// <para>
    /// This implementation specifically listens for <c>HCBT_ACTIVATE</c> events targeting
    /// the Rhino main window.
    /// </para>
    /// <para>
    /// <b>Why Use This Pattern?</b>
    /// </para>
    /// <para>
    /// When running Rhino in "headless" mode inside AutoCAD, the Rhino window is initially
    /// hidden. However, certain operations (like <c>RhinoGet</c> point picking) require
    /// user interaction with the Rhino window. The CBT hook detects when the Rhino window
    /// is attempting to activate and automatically shows the window if Rhino is waiting
    /// for user input (i.e., <c>RhinoGet.InGet()</c> returns true).
    /// </para>
    /// <para>
    /// This is the same pattern used by Rhino.Inside.Revit's <c>ComputerBasedTrainingHook</c>.
    /// </para>
    /// <para>
    /// <b>Thread Requirements:</b>
    /// </para>
    /// <para>
    /// The hook must be installed on a thread with a Windows message pump. This is a
    /// thread-local hook (installed with <c>dwThreadId = GetCurrentThreadId()</c>), meaning
    /// it only receives events for windows owned by the current thread. When running inside
    /// AutoCAD, install from the main UI thread.
    /// </para>
    /// <para>
    /// <b>Lifecycle:</b>
    /// </para>
    /// <para>
    /// The hook remains active until <see cref="UninstallActivationHook"/> is called or the
    /// object is disposed. The hook is automatically uninstalled when <see cref="IDisposable.Dispose"/>
    /// is called.
    /// </para>
    /// <para>
    /// <b>Idempotency:</b>
    /// </para>
    /// <para>
    /// Calling this method multiple times is safe. If a hook is already installed, subsequent
    /// calls do nothing. Similarly, if no window has been set via <see cref="SetWindow"/>,
    /// the method returns without installing a hook (there's nothing to monitor).
    /// </para>
    /// <para>
    /// <b>Delegate Lifetime:</b>
    /// </para>
    /// <para>
    /// The hook callback delegate must remain alive for the entire duration the hook is
    /// installed. The implementation stores the delegate as a field to prevent garbage
    /// collection. If the delegate were collected while the hook is active, Windows would
    /// call invalid memory, causing an application crash.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set up the window manager
    /// windowManager.SetWindow(RhinoApp.MainWindowHandle());
    ///
    /// // Start monitoring - window will auto-show when RhinoGet.InGet() is true
    /// windowManager.InstallActivationHook();
    /// </code>
    /// </example>
    /// <seealso cref="UninstallActivationHook"/>
    void InstallActivationHook();

    /// <summary>
    /// Uninstalls the CBT hook that monitors window activation events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method removes the Windows CBT hook installed by <see cref="InstallActivationHook"/>.
    /// After calling this method, window activation will no longer be monitored.
    /// </para>
    /// <para>
    /// <b>Idempotency:</b>
    /// </para>
    /// <para>
    /// Calling this method when no hook is installed is safe and has no effect.
    /// </para>
    /// <para>
    /// <b>Automatic Cleanup:</b>
    /// </para>
    /// <para>
    /// This method is automatically called by <see cref="IDisposable.Dispose"/>. Manual
    /// invocation is only needed if you want to temporarily disable activation monitoring
    /// without disposing the entire manager.
    /// </para>
    /// <para>
    /// <b>Resource Cleanup:</b>
    /// </para>
    /// <para>
    /// After uninstalling, the internal delegate reference is cleared, allowing it to be
    /// garbage collected. A new delegate will be created if <see cref="InstallActivationHook"/>
    /// is called again.
    /// </para>
    /// </remarks>
    /// <seealso cref="InstallActivationHook"/>
    void UninstallActivationHook();
}
