namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the Support Dialog lifecycle, including showing and hiding the dialog.
/// </summary>
public interface ISupportDialogManager : IDisposable
{
    /// <summary>
    /// Shows the Support Dialog with the default tab selected.
    /// </summary>
    void Show();

    /// <summary>
    /// Shows the Support Dialog with the specified tab selected.
    /// </summary>
    /// <param name="tab">The tab to display.</param>
    void Show(SupportDialogTab tab);

    /// <summary>
    /// Hides the Support Dialog.
    /// </summary>
    void Hide();
}
