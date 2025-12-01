namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A custom page navigation service.
/// </summary>
public interface IPageNavigationService 
{
    /// <summary>
    /// The event handler raised when a page is navigated to.
    /// </summary>
    event EventHandler PageChanged;

    /// <summary>
    /// The current page object.
    /// </summary>
    object? CurrentPage { get; set; }

    /// <summary>
    /// The key of the current page.
    /// </summary>
    string CurrentPageKey { get; }

    /// <summary>
    /// Navigates to the page using the <paramref name="pageKey"/>.
    /// </summary>
    void NavigateTo(string pageKey, bool recordInHistory = true);
}