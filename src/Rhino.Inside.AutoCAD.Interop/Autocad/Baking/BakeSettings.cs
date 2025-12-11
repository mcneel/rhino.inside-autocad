using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Represents settings to apply when baking objects to AutoCAD.
/// </summary>
public class BakeSettings : IBakeSettings
{
    /// <inheritdoc/>
    public IAutocadLayerTableRecord? Layer { get; }

    /// <inheritdoc/>
    public IAutocadLinetypeTableRecord? LineType { get; }

    /// <inheritdoc/>
    public IColor? Color { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BakeSettings"/> class.
    /// </summary>
    /// <param name="layer">The layer to assign to baked entities.</param>
    /// <param name="lineType">The line type to assign to baked entities.</param>
    /// <param name="color">The color to assign to baked entities.</param>
    public BakeSettings(
        IAutocadLayerTableRecord? layer = null,
        IAutocadLinetypeTableRecord? lineType = null,
        IColor? color = null)
    {
        this.Layer = layer;
        this.LineType = lineType;
        this.Color = color;
    }
}
