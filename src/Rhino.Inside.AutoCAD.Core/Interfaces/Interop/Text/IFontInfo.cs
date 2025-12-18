namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents font information, including the font name and style attributes.
/// </summary>
public interface IFontInfo
{
    /// <summary>
    /// Gets the name of the font.
    /// </summary>
    string FontName { get; }

    /// <summary>
    /// Gets a value indicating whether the font is bold.
    /// </summary>
    bool Bold { get; }

    /// <summary>
    /// Gets a value indicating whether the font is italicized.
    /// </summary>
    bool Italic { get; }
}