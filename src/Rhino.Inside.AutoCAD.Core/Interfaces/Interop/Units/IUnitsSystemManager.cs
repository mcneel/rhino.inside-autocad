namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which defines units conversion between <see cref="IAutocadDocument.UnitSystem"/>
/// and the <see cref="RhinoUnits"/> <see cref="UnitSystem"/>.
/// </summary>
public interface IUnitSystemManager
{
    /// <summary>
    /// The <see cref="UnitSystem"/> used internally for all applications.
    /// </summary>
    /// <remarks>
    /// The <see cref="RhinoUnits"/> units are what are used for Rhino Inside.
    /// </remarks>
    UnitSystem RhinoUnits { get; }

    /// <summary>
    /// The <see cref="UnitSystem"/> of the active <see cref="IAutocadDocument"/>.
    /// </summary>
    /// <remarks>
    /// The value returns the same value as <see cref="IAutocadDocument.UnitSystem"/>.
    /// </remarks>
    UnitSystem AutoCadUnits { get; }

    /// <summary>
    /// Converts the given <paramref name="length"/> from <see cref="IAutocadDocument.UnitSystem"/>
    /// to the <see cref="RhinoUnits"/> units.
    /// </summary>
    double ToRhinoLength(double length);

    /// <summary>
    /// Converts the given <see cref="IUnitLength"/> to the <see cref=
    /// "RhinoUnits"/> units.
    /// </summary>
    double ToRhinoLength(IUnitLength length);

    /// <summary>
    /// Converts the given <paramref name="area"/> from <see cref=
    /// "IAutocadDocument.UnitSystem"/> to the <see cref="RhinoUnits"/> units.
    /// </summary>
    double ToRhinoArea(double area);

    /// <summary>
    /// Converts the given <paramref name="internalLength"/> from <see cref="IAutocadDocument.UnitSystem"/>.
    /// </summary>
    double ToAutoCadLength(double internalLength);

    /// <summary>
    /// Converts the given <paramref name="length"/> to the <see cref="AutoCadUnits"/>
    /// units.
    /// </summary>
    double ToAutoCadLength(IUnitLength length);

    /// <summary>
    /// Converts the given <paramref name="internalArea"/> to <see cref="IAutocadDocument.UnitSystem"/>.
    /// </summary>
    double ToAutoCadArea(double internalArea);

    /// <summary>
    /// Converts the given <paramref name="radians"/> to degrees.
    /// </summary>
    double ToDegrees(double radians);

    /// <summary>
    /// Converts the given <paramref name="degrees"/> to radians.
    /// </summary>
    double ToRadians(double degrees);

    /// <summary>
    /// Converts the <paramref name="value"/> provided in the <paramref name="source"/>
    /// <see cref="UnitSystem"/> to the <paramref name="destination"/> <see cref="UnitSystem"/>.
    /// </summary>
    double Convert(double value, UnitSystem source, UnitSystem destination);
}