namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines the contract for a unit scale that associates a unit name with its conversion ratio relative to meters.
/// </summary>
/// <remarks>
/// Unit scales combine a human-readable <see cref="Name"/> with a precise <see cref="Ratio"/> for unit conversions.
/// The ratio represents how many meters equal one unit (e.g., for inches, the ratio is 254/10000 = 0.0254 meters per inch).
/// </remarks>
/// <seealso cref="IRatio"/>
public interface IUnitScale
{
    /// <summary>
    /// Gets the display name of this unit scale (e.g., "Meters", "Inches", "Feet").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the <see cref="IRatio"/> representing how many meters equal one unit of this scale.
    /// </summary>
    /// <remarks>
    /// The ratio maintains both numerator and denominator separately to preserve precision during conversions.
    /// For example, inches use a ratio of 254/10000 rather than the floating-point approximation 0.0254.
    /// </remarks>
    IRatio Ratio { get; }

    /// <summary>
    /// Determines whether this unit scale is equal to another based on their
    /// ratios within a specified tolerance.
    /// </summary>
    bool IsEqualTo(IUnitScale other);
}