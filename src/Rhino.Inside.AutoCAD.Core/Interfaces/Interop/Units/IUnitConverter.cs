namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides unit conversion between AutoCAD and Rhino unit systems.
/// </summary>
/// <remarks>
/// Handles bidirectional conversion of length values between the active
/// <see cref="IAutocadDocument"/> unit system and the internal Rhino unit system.
/// This is essential for geometry interoperability where AutoCAD and Rhino
/// may be configured with different unit systems (e.g., millimeters vs inches).
/// </remarks>
/// <seealso cref="IAutocadDocument"/>
public interface IUnitConverter
{
    /// <summary>
    /// Gets the <see cref="IUnitScale"/> used internally by Rhino.
    /// </summary>
    /// <remarks>
    /// All Rhino Inside geometry operations use this unit system internally.
    /// Typically, matches the Rhino document's unit configuration.
    /// </remarks>
    IUnitScale RhinoUnits { get; }

    /// <summary>
    /// Gets the <see cref="IUnitScale"/> of the active AutoCAD document.
    /// </summary>
    /// <remarks>
    /// Reflects the current drawing's INSUNITS setting.
    /// Equivalent to <see cref="IAutocadDocument.UnitSystem"/>.
    /// </remarks>
    IUnitScale AutoCadUnits { get; }
}