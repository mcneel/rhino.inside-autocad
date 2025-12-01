using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Windows.Media.Imaging;

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IButtonIconReplacer"/>
public class ButtonIconReplacer : IButtonIconReplacer
{
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

    /// <inheritdoc />
    public void Replace(string buttonFilePath)
    {
        var ribbon = Autodesk.Windows.ComponentManager.Ribbon;

        foreach (var tab in ribbon.Tabs)
        {
            foreach (var panel in tab.Panels)
            {
                foreach (var item in panel.Source.Items)
                {
                    if (item is Autodesk.Windows.RibbonButton button && button.Id == this.ButtonId)
                    {

                        button.ShowImage = true;
                        button.Image = this.ResizeImage(buttonFilePath, _smallIconSize, _smallIconSize);

                        button.LargeImage = this.ResizeImage(buttonFilePath, _largeIconSize, _largeIconSize);
                    }
                }
            }
        }
    }
}