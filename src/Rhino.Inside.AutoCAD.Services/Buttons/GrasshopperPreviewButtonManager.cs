using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IGrasshopperPreviewButtonManager"/>
public class GrasshopperPreviewButtonManager : IGrasshopperPreviewButtonManager
{
    private const string _offButtonId = ApplicationConstants.OffButtonId;
    private const string _shadedButtonId = ApplicationConstants.ShadedButtonId;
    private const string _wireframeButtonId = ApplicationConstants.WireframeButtonId;
    private const string _offButtonUnselected = ApplicationConstants.OffButtonUnselected;
    private const string _offButtonSelected = ApplicationConstants.OffButtonSelected;
    private const string _shadedButtonUnselected = ApplicationConstants.ShadedButtonUnselected;
    private const string _shadedButtonSelected = ApplicationConstants.ShadedButtonSelected;
    private const string _wireframeButtonUnselected = ApplicationConstants.WireframeButtonUnselected;
    private const string _wireframeButtonSelected = ApplicationConstants.WireframeButtonSelected;

    /// <summary>
    /// Updates the button with the given ID by replacing its icon with the new image.
    /// </summary>
    private void UpdateButton(string buttonId, string unselectedImagePath)
    {
        var buttonReplacer = new ButtonIconReplacer(buttonId);

        buttonReplacer.Replace(unselectedImagePath);
    }


    /// <inheritdoc/>
    public void SetPreviewMode(GrasshopperPreviewMode mode)
    {
        switch (mode)
        {
            case GrasshopperPreviewMode.Off:
                this.UpdateButton(_offButtonId, _offButtonSelected);
                this.UpdateButton(_shadedButtonId, _shadedButtonUnselected);
                this.UpdateButton(_wireframeButtonId, _wireframeButtonUnselected);
                break;
            case GrasshopperPreviewMode.Shaded:
                this.UpdateButton(_offButtonId, _offButtonUnselected);
                this.UpdateButton(_shadedButtonId, _shadedButtonSelected);
                this.UpdateButton(_wireframeButtonId, _wireframeButtonUnselected);
                break;
            case GrasshopperPreviewMode.Wireframe:
                this.UpdateButton(_offButtonId, _offButtonUnselected);
                this.UpdateButton(_shadedButtonId, _shadedButtonUnselected);
                this.UpdateButton(_wireframeButtonId, _wireframeButtonSelected);
                break;
        }
    }
}