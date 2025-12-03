namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the Grasshopper preview buttons states. This is used to update the UI when the
/// preview mode changes. Ensures only one button is selected at a time.
/// </summary>
public interface IGrasshopperPreviewButtonManager
{
    /// <summary>
    /// Sets the preview mode button states based on the provided mode.
    /// </summary>
    /// <param name="mode"></param>
    void SetPreviewMode(GrasshopperPreviewMode mode);
}