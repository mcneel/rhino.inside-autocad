namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Marker interface for WPF windows within the application.
/// </summary>
/// <remarks>
/// Provides a common abstraction for windows that need to be managed or referenced
/// in a loosely coupled manner. Implemented by UI windows such as the splash screen
/// and support dialog to enable dependency injection and testability.
/// </remarks>
/// <seealso cref="ILoadingScreenManager"/>
/// <seealso cref="ISupportDialogManager"/>
public interface IWindow
{
}