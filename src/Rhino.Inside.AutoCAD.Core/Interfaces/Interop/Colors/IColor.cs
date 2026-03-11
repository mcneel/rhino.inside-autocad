namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an RGBA color used for entity and layer visualization.
/// </summary>
/// <remarks>
/// Provides a platform-independent color representation for converting between AutoCAD
/// colors (ACI or true color) and Rhino/Grasshopper colors. Used by layer and entity
/// wrappers such as <see cref="IAutocadLayerTableRecord"/> to expose color properties.
/// </remarks>
/// <seealso cref="IAutocadLayerTableRecord.Color"/>
public interface IColor
{
    /// <summary>
    /// Gets the red component of the color (0-255).
    /// </summary>
    byte Red { get; }

    /// <summary>
    /// Gets the green component of the color (0-255).
    /// </summary>
    byte Green { get; }

    /// <summary>
    /// Gets the blue component of the color (0-255).
    /// </summary>
    byte Blue { get; }

    /// <summary>
    /// Gets the alpha (opacity) component of the color (0-255).
    /// </summary>
    /// <remarks>
    /// A value of 255 represents fully opaque; 0 represents fully transparent.
    /// AutoCAD entities typically use fully opaque colors.
    /// </remarks>
    byte Alpha { get; }
}