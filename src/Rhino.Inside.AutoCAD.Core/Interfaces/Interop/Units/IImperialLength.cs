namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an imperial length with major (feet) and minor (inches)
/// length components. The total length is calculated as the sum of
/// the <see cref="Feet"/> + <see cref="Inches"/>.
/// </summary>
public interface IImperialLength
{
    /// <summary>
    /// The event raised when either the <see cref="Feet"/> or
    /// <see cref="Inches"/> are changed.
    /// </summary>
    event EventHandler? LengthChanged;

    /// <summary>
    /// The major length component in decimal <see cref="UnitSystem.Feet"/>.
    /// </summary>
    IUnitLength Feet { get; }

    /// <summary>
    /// The minor length component in decimal <see cref="UnitSystem.Inches"/>.
    /// </summary>
    IUnitLength Inches { get; }

    /// <summary>
    /// Sets the <see cref="Feet"/> and <see cref="Inches"/> to match the
    /// <paramref name="candidateLength"/>.
    /// </summary>
    void SetTo(IImperialLength candidateLength);
}