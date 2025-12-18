namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the formatting state for text, including font style and decoration.
/// </summary>
public interface IFormatState
{
    /// <summary>
    /// Gets the name of the font used for the text.
    /// </summary>
    string FontName { get; }

    /// <summary>
    /// Gets a value indicating whether the text is bold.
    /// </summary>
    bool Bold { get; }

    /// <summary>
    /// Gets a value indicating whether the text is italicized.
    /// </summary>
    bool Italic { get; }

    /// <summary>
    /// Gets a value indicating whether the text is underlined.
    /// </summary>
    bool Underline { get; }

    /// <summary>
    /// Creates a copy of the current formatting state.
    /// </summary>
    /// <returns>
    /// A new <see cref="IFormatState"/> instance with the same formatting properties.
    /// </returns>
    IFormatState Clone();
}