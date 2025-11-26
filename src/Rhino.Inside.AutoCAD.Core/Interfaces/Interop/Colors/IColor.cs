namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a color with red, green, and blue components.
/// </summary>
public interface IColor
{
    /// <summary>
    /// The red component of the <see cref="IColor"/>.
    /// </summary>
    byte Red { get; }

    /// <summary>
    /// The green component of the <see cref="IColor"/>.
    /// </summary>
    byte Green { get; }

    /// <summary>
    /// The blue component of the <see cref="IColor"/>.
    /// </summary>
    byte Blue { get; }

    /// <summary>
    /// The alpha component of the <see cref="IColor"/>.
    /// </summary>
    byte Alpha { get; }
}