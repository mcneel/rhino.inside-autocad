using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IColor"/>
public class InternalColor : IColor
{
    /// <inheritdoc/>
    public byte Red { get; }

    /// <inheritdoc/>
    public byte Green { get; }

    /// <inheritdoc/>
    public byte Blue { get; }

    /// <inheritdoc/>
    public byte Alpha { get; }

    /// <summary>
    /// Constructs a new <see cref="InternalColor"/>.
    /// </summary>
    public InternalColor(byte red, byte green, byte blue, byte alpha = 255)
    {
        this.Red = red;

        this.Green = green;

        this.Blue = blue;

        this.Alpha = alpha;
    }

    /// <summary>
    /// Constructs a new <see cref="InternalColor"/> from <see cref=
    /// "Autodesk.AutoCAD.Colors.Color"/>.
    /// </summary>
    public InternalColor(Autodesk.AutoCAD.Colors.Color color)
    {
        this.Red = color.Red;

        this.Green = color.Green;

        this.Blue = color.Blue;
    }

    /// <summary>
    /// Constructs a new <see cref="InternalColor"/> from <see cref="System.Drawing.Color"/>.
    /// </summary>
    public InternalColor(System.Drawing.Color color)
    {
        this.Red = color.R;

        this.Green = color.G;

        this.Blue = color.B;

        this.Alpha = color.A;
    }
}