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
    /// Unselects the button with the given ID by replacing its icon with the unselected image.
    /// </summary>
    private void UnselectButton(string buttonId, string unselectedImagePath)
    {
        var buttonReplacer = new ButtonIconReplacer(buttonId);

        buttonReplacer.Replace(unselectedImagePath);
    }

    /// <summary>
    /// Selects the button with the given ID by replacing its icon with the selected image.
    /// </summary>
    private void SelectButton(string buttonId, string selectedImagePath)
    {
        var buttonReplacer = new ButtonIconReplacer(buttonId);

        buttonReplacer.Replace(selectedImagePath);
    }

    /// <inheritdoc/>
    public void SetPreviewMode(GrasshopperPreviewMode mode)
    {
        switch (mode)
        {
            case GrasshopperPreviewMode.Off:
                this.SelectButton(_offButtonId, _offButtonSelected);
                this.UnselectButton(_shadedButtonId, _shadedButtonUnselected);
                this.UnselectButton(_wireframeButtonId, _wireframeButtonUnselected);
                break;
            case GrasshopperPreviewMode.Shaded:
                this.UnselectButton(_offButtonId, _offButtonUnselected);
                this.SelectButton(_shadedButtonId, _shadedButtonSelected);
                this.UnselectButton(_wireframeButtonId, _wireframeButtonUnselected);
                break;
            case GrasshopperPreviewMode.Wireframe:
                this.UnselectButton(_offButtonId, _offButtonUnselected);
                this.UnselectButton(_shadedButtonId, _shadedButtonUnselected);
                this.SelectButton(_wireframeButtonId, _wireframeButtonSelected);
                break;
        }
    }
}