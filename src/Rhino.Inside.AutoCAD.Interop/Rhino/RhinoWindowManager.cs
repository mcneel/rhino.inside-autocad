using Rhino.Input;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Runtime.InteropServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoWindowManager"/>
public class RhinoWindowManager : IRhinoWindowManager
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr windowHandle, int windowShowStyle);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr windowHandle);

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(IntPtr windowHandle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_CBT = 5;
    private const int HCBT_ACTIVATE = 5;
    private const int HCBT_MINMAX = 1;
    private const int HCBT_SYSCOMMAND = 8;

    private const int SC_MASK = 0xFFF0;
    private const int SC_CLOSE = 0xF060;

    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;
    private const int SW_MINIMIZE = 6;
    private const int SW_SHOWMINNOACTIVE = 7;
    private const int SW_RESTORE = 9;
    private const int SW_FORCEMINIMIZE = 11;

    private IntPtr _mainWindow;
    private IntPtr _hookHandle = IntPtr.Zero;
    private HookProc? _hookDelegate;
    private bool _disposed;

    /// <summary>
    /// True while a re-read of the window's display state is waiting for AutoCAD to be idle.
    /// </summary>
    private bool _displayStateCheckPending;

    /// <summary>
    /// Contains failures raised by <see cref="DisplayStateChanged"/> handlers, which are
    /// reached from the hook or the AutoCAD idle loop, both called from native code.
    /// </summary>
    private readonly IAutocadGuard _autocadGuard = new AutocadGuard();

    /// <inheritdoc />
    public event EventHandler? DisplayStateChanged;

    /// <inheritdoc />
    public bool IsMinimised { get; private set; }

    /// <inheritdoc />
    public bool IsHidden { get; private set; }

    public RhinoWindowManager()
    {
        _mainWindow = IntPtr.Zero;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Reads the window's display state straight away, so <see cref="IsHidden"/> is true
    /// from the start when Rhino is created with its window hidden.
    /// </remarks>
    public void SetWindow(IntPtr mainWindow)
    {
        _mainWindow = mainWindow;

        this.UpdateDisplayState(nameof(this.SetWindow));
    }

    /// <summary>
    /// Sets <see cref="IsMinimised"/> and <see cref="IsHidden"/>, raising
    /// <see cref="DisplayStateChanged"/> when either changes.
    /// </summary>
    /// <param name="isMinimised">Whether the main window is minimised.</param>
    /// <param name="isHidden">Whether the main window is hidden.</param>
    /// <param name="source">What led to the state being set, for the debug output.</param>
    private void SetDisplayState(bool isMinimised, bool isHidden, string source)
    {
        if (this.IsMinimised == isMinimised && this.IsHidden == isHidden) return;

        System.Diagnostics.Debug.WriteLine(
            $"RhinoWindowManager.DisplayStateChanged ({source}): " +
            $"isMinimised {this.IsMinimised} -> {isMinimised}, isHidden {this.IsHidden} -> {isHidden}");

        this.IsMinimised = isMinimised;

        this.IsHidden = isHidden;

        _autocadGuard.Run(() => this.DisplayStateChanged?.Invoke(this, EventArgs.Empty),
            nameof(this.DisplayStateChanged));
    }

    /// <summary>
    /// Reads whether the main window is minimised or hidden from the window itself.
    /// </summary>
    /// <param name="source">What led to the read, for the debug output.</param>
    /// <remarks>
    /// Without a window nothing is known, and neither state is reported.
    /// </remarks>
    private void UpdateDisplayState(string source)
    {
        if (_mainWindow == IntPtr.Zero)
        {
            this.SetDisplayState(false, false, source);

            return;
        }

        var isMinimised = IsIconic(_mainWindow);

        var isHidden = IsWindowVisible(_mainWindow) == false;

        this.SetDisplayState(isMinimised, isHidden, source);
    }

    /// <summary>
    /// Re-reads the main window's display state once AutoCAD is next idle.
    /// </summary>
    /// <remarks>
    /// Used from the hook, which is called before the window is changed: by the time
    /// AutoCAD is idle the change, such as Rhino hiding the window in place of closing it,
    /// has happened. Requests made while one is pending are merged into it.
    /// </remarks>
    private void ScheduleDisplayStateCheck()
    {
        if (_displayStateCheckPending || _disposed) return;

        _displayStateCheckPending = true;

        Application.Idle += this.OnIdle;
    }

    /// <summary>
    /// Re-reads the main window's display state, once, after a scheduled check.
    /// </summary>
    private void OnIdle(object? sender, EventArgs e)
    {
        Application.Idle -= this.OnIdle;

        _displayStateCheckPending = false;

        _autocadGuard.Run(() => this.UpdateDisplayState("idle"), nameof(this.OnIdle));
    }

    /// <inheritdoc />
    public void HideWindow()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.Hide);

        this.UpdateDisplayState(nameof(this.HideWindow));
    }

    /// <inheritdoc />
    public void BringToFront()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.Restore);
        BringWindowToTop(_mainWindow);
        SetForegroundWindow(_mainWindow);

        this.UpdateDisplayState(nameof(this.BringToFront));
    }

    /// <inheritdoc />
    public void ShowWindow()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.Show);

        this.UpdateDisplayState(nameof(this.ShowWindow));
    }

    /// <inheritdoc />
    public void ShowWindowNoActivate()
    {
        if (_mainWindow == IntPtr.Zero)
            return;

        ShowWindow(_mainWindow, (int)WindowShowStyle.ShowNA);

        this.UpdateDisplayState(nameof(this.ShowWindowNoActivate));
    }

    /// <inheritdoc />
    public void InstallActivationHook()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RhinoWindowManager));

        if (_hookHandle != IntPtr.Zero)
            return;

        if (_mainWindow == IntPtr.Zero)
            return;

        _hookDelegate = new HookProc(this.CbtHookProc);

        var windowThreadId = GetWindowThreadProcessId(_mainWindow, out _);

        _hookHandle = SetWindowsHookEx(
            WH_CBT,
            _hookDelegate,
            IntPtr.Zero,
            windowThreadId);

        if (_hookHandle == IntPtr.Zero)
            _hookDelegate = null;
    }

    /// <inheritdoc />
    public void UninstallActivationHook()
    {
        if (_hookHandle == IntPtr.Zero)
            return;

        UnhookWindowsHookEx(_hookHandle);
        _hookHandle = IntPtr.Zero;
        _hookDelegate = null;
    }

    /// <summary>
    /// Tracks a minimise or restore request for the main window, raising
    /// <see cref="DisplayStateChanged"/> when it changes <see cref="IsMinimised"/>.
    /// </summary>
    /// <param name="lParam">
    /// The <c>HCBT_MINMAX</c> hook parameter, whose low word is the <c>ShowWindow</c> command.
    /// </param>
    /// <remarks>
    /// Only observes: the request is never blocked. Any other show command, such as a hide,
    /// leaves the state as it was. The real state is read again once AutoCAD is idle.
    /// </remarks>
    private void OnMinMax(IntPtr lParam)
    {
        var showCommand = (int)(lParam.ToInt64() & 0xFFFF);

        this.ScheduleDisplayStateCheck();

        bool isMinimised;

        switch (showCommand)
        {
            case SW_MINIMIZE:
            case SW_SHOWMINIMIZED:
            case SW_SHOWMINNOACTIVE:
            case SW_FORCEMINIMIZE:
                isMinimised = true;
                break;
            case SW_RESTORE:
            case SW_SHOWNORMAL:
            case SW_SHOWMAXIMIZED:
                isMinimised = false;
                break;
            default:
                System.Diagnostics.Debug.WriteLine(
                    $"RhinoWindowManager.HCBT_MINMAX: showCommand={showCommand} ignored, " +
                    $"isMinimised={this.IsMinimised}, isHidden={this.IsHidden}");
                return;
        }

        System.Diagnostics.Debug.WriteLine(
            $"RhinoWindowManager.HCBT_MINMAX: showCommand={showCommand}, " +
            $"isMinimised {this.IsMinimised} -> {isMinimised}, isHidden={this.IsHidden}");

        this.SetDisplayState(isMinimised, this.IsHidden, nameof(HCBT_MINMAX));
    }

    /// <summary>
    /// Schedules a re-read of the display state when a window on the main thread is about
    /// to be closed through its system menu or close button.
    /// </summary>
    /// <param name="wParam">The <c>HCBT_SYSCOMMAND</c> hook parameter, the system command.</param>
    /// <remarks>
    /// <c>HCBT_SYSCOMMAND</c> does not say which window the command is for, so it is not
    /// matched to the main window: the check reads the main window's real state, which
    /// is left as it was when another window is closed. Rhino hides its main window in
    /// place of closing it, after the hook has returned.
    /// </remarks>
    private void OnSysCommand(IntPtr wParam)
    {
        var command = (int)(wParam.ToInt64() & SC_MASK);

        if (command != SC_CLOSE) return;

        System.Diagnostics.Debug.WriteLine(
            $"RhinoWindowManager.HCBT_SYSCOMMAND: SC_CLOSE, " +
            $"activeWindowIsMain={GetActiveWindow() == _mainWindow}, " +
            $"isMinimised={this.IsMinimised}, isHidden={this.IsHidden}");

        this.ScheduleDisplayStateCheck();
    }

    private IntPtr CbtHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode == HCBT_MINMAX && wParam == _mainWindow)
        {
            this.OnMinMax(lParam);

            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        if (nCode == HCBT_SYSCOMMAND)
        {
            this.OnSysCommand(wParam);

            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        // Any activation may follow the main window being shown or hidden, by whatever
        // route, so its state is read again once AutoCAD is idle.
        if (nCode == HCBT_ACTIVATE)
        {
            this.ScheduleDisplayStateCheck();
        }

        if (nCode >= 0 && nCode == HCBT_ACTIVATE && wParam == _mainWindow)
        {
            // If window is already visible, always allow activation (normal interaction)
            if (IsWindowVisible(_mainWindow))
            {
                return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
            }

            // Window is hidden - check if Rhino needs user input
            if (RhinoDoc.ActiveDoc is RhinoDoc rhinoDoc && RhinoGet.InGet(rhinoDoc))
            {
                // InGet is true - show window and allow activation
                this.ShowWindowNoActivate();
            }
            else
            {
                // Window hidden, InGet false - block activation so Rhino retries later
                return (IntPtr)1;
            }
        }

        return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }



    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        this.UninstallActivationHook();
        this.DisplayStateChanged = null;

        // Only when disposing: the finalizer runs on another thread, away from AutoCAD.
        if (disposing && _displayStateCheckPending)
        {
            Application.Idle -= this.OnIdle;
            _displayStateCheckPending = false;
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~RhinoWindowManager()
    {
        this.Dispose(false);
    }
}
