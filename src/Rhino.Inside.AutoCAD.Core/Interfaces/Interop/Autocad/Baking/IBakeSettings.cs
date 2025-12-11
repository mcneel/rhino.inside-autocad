namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines settings to apply when baking objects to AutoCAD.
/// </summary>
public interface IBakeSettings
{
    /// <summary>
    /// Gets the layer to assign to baked entities. Null means use default layer (0).
    /// </summary>
    IAutocadLayerTableRecord? Layer { get; }

    /// <summary>
    /// Gets the line type to assign to baked entities. Null means use ByLayer.
    /// </summary>
    IAutocadLinetypeTableRecord? LineType { get; }

    /// <summary>
    /// Gets the color to assign to baked entities. Null means use ByLayer.
    /// </summary>
    IColor? Color { get; }
}
