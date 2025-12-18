using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IFontInfo"/>
public class FontInfo : IFontInfo
{
    /// <inheritdoc />
    public string FontName { get; set; }

    /// <inheritdoc />
    public bool Bold { get; set; }

    /// <inheritdoc />
    public bool Italic { get; set; }
}