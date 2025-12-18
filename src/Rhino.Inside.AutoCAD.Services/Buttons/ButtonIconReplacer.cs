using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.Windows;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows.Media.Imaging;
using Exception = Autodesk.AutoCAD.BoundaryRepresentation.Exception;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IButtonIconReplacer"/>
public class ButtonIconReplacer : IButtonIconReplacer
{
    private const string _rhinoInsideTabName = ApplicationConstants.RhinoInsideTabName;
    private const int _smallIconSize = ApplicationConstants.SmallIconSize;
    private const int _largeIconSize = ApplicationConstants.LargeIconSize;
    private const string _rhinoInsideTabNotLoadedError = MessageConstants.RhinoInsideTabNotLoadedError;

    /// <inheritdoc />
    public string ButtonId { get; }

    /// <summary>
    /// Constructs a new <see cref="IButtonIconReplacer"/>
    /// </summary>
    public ButtonIconReplacer(string buttonId)
    {
        this.ButtonId = buttonId;
    }

    /// <summary>
    /// Creates a resized <see cref="BitmapImage"/> from the image.
    /// </summary>
    private BitmapImage ResizeImage(string imagePath, int width, int height)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(imagePath);
        bitmap.DecodePixelWidth = width;
        bitmap.DecodePixelHeight = height;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    /// <summary>
    /// Updates the button images.
    /// </summary>
    private void UpdateButton(string buttonFilePath, RibbonButton button)
    {
        button.ShowImage = true;

        button.Image = this.ResizeImage(buttonFilePath, _smallIconSize, _smallIconSize);

        button.LargeImage = this.ResizeImage(buttonFilePath, _largeIconSize, _largeIconSize);
    }

    /// <summary>
    /// Finds the Rhino Inside ribbon tab in the Autocad UI Ribbon.
    /// </summary>
    private bool FindRhinoInsideTab(out RibbonTab? rhinoInsideTab)
    {
        var ribbon = ComponentManager.Ribbon;

        foreach (var tab in ribbon.Tabs)
        {
            if (tab.Title != _rhinoInsideTabName) continue;

            rhinoInsideTab = tab;
            return true;
        }

        rhinoInsideTab = null;
        return false;
    }

    /// <summary>
    /// Finds the button in the Rhino Inside ribbon tab.
    /// </summary>
    private bool FindButton(RibbonTab rhinoInsideTab, out RibbonButton? ribbonButton)
    {
        foreach (var panel in rhinoInsideTab.Panels)
        {
            foreach (var item in panel.Source.Items)
            {
                if (item is RibbonButton button &&
                    button.Id == this.ButtonId)
                {
                    ribbonButton = button;
                    return true;
                }

                if (item is RibbonRowPanel subPanel)
                {
                    foreach (var subPanelItem in subPanel.Items)
                    {
                        if (subPanelItem is RibbonButton subButton &&
                            subButton.Id == this.ButtonId)
                        {
                            ribbonButton = subButton;
                            return true;
                        }
                    }
                }
            }
        }

        ribbonButton = null;
        return false;
    }

    /// <inheritdoc />
    public void Replace(string buttonFilePath)
    {
        if (this.FindRhinoInsideTab(out var rhinoInsideTab) == false)
        {
            throw new Exception(ErrorStatus.InvalidInput, _rhinoInsideTabNotLoadedError);
        }

        if (this.FindButton(rhinoInsideTab!, out var button))
        {
            this.UpdateButton(buttonFilePath, button!);
        }
    }
}