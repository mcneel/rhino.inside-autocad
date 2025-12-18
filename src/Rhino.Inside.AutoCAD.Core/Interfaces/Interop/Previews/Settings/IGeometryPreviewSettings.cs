namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides settings for previewing geometry in AutoCAD.
/// </summary>
public interface IGeometryPreviewSettings
{
    /// <summary>
    /// Gets the color index used for the preview geometry.
    /// </summary>
    int ColorIndex { get; }

    /// <summary>
    /// Gets the transparency level used for the preview geometry.
    /// </summary>
    byte Transparency { get; }

    /// <summary>
    /// Gets the material ID used for the preview geometry.
    /// </summary>
    IObjectId MaterialId { get; }

    /// <summary>
    /// The name of the material to use.
    /// </summary>
    string MaterialName { get; }

    /// <summary>
    /// Creates the preview material in the AutoCAD database if it does not already exist.
    /// </summary>
    void CreateMaterial(IAutocadDocument document);
}