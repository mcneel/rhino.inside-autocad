using Autodesk.Windows;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows.Media.Imaging;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IButtonIconReplacer"/>
public class ButtonIconReplacer : IButtonIconReplacer
{
    private const string _rhinoInsideTabName = ApplicationConstants.RhinoInsideTabName;
    private const int _smallIconSize = ApplicationConstants.SmallIconSize;
    private const int _largeIconSize = ApplicationConstants.LargeIconSize;

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

    /// <inheritdoc />
    public void Replace(string buttonFilePath)
    {
        var ribbon = Autodesk.Windows.ComponentManager.Ribbon;

        foreach (var tab in ribbon.Tabs)
        {
            if (tab.Title != _rhinoInsideTabName) continue;

            foreach (var panel in tab.Panels)
            {
                foreach (var item in panel.Source.Items)
                {
                    if (item is Autodesk.Windows.RibbonButton button && button.Id == this.ButtonId)
                    {
                        this.UpdateButton(buttonFilePath, button);
                    }

                    if (item is Autodesk.Windows.RibbonRowPanel subPanel)
                    {
                        foreach (var subPanelItem in subPanel.Items)
                        {
                            if (subPanelItem is Autodesk.Windows.RibbonButton subButton &&
                                subButton.Id == this.ButtonId)
                            {
                                this.UpdateButton(buttonFilePath, subButton);
                            }
                        }
                    }
                }
            }
        }
    }
}