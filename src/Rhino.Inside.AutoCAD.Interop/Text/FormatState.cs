using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IFormatState"/>
public class FormatState : IFormatState
{

    /// <inheritdoc />
    public string FontName { get; set; }

    /// <inheritdoc />
    public bool Bold { get; set; }

    /// <inheritdoc />
    public bool Italic { get; set; }

    /// <inheritdoc />
    public bool Underline { get; set; }

    /// <inheritdoc />
    IFormatState IFormatState.Clone() => this.Clone();

    /// <summary>
    /// Creates a copy of the current formatting state.
    /// </summary>
    public FormatState Clone() => new FormatState
    {
        Bold = this.Bold,
        Italic = this.Italic,
        Underline = this.Underline,
        FontName = this.FontName
    };
}