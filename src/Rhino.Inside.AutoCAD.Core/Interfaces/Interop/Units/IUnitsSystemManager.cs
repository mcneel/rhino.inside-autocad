namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which defines units conversion between <see cref="IDocument.UnitSystem"/>
/// and the <see cref="InternalUnits"/> <see cref="UnitSystem"/>.
/// </summary>
public interface IUnitSystemManager
{
    /// <summary>
    /// The <see cref="UnitSystem"/> used internally for all applications.
    /// </summary>
    /// <remarks>
    /// The <see cref="InternalUnits"/> units are what are used for Rhino Inside.
    /// </remarks>
    UnitSystem InternalUnits { get; }

    /// <summary>
    /// The <see cref="UnitSystem"/> of the active <see cref="IDocument"/>.
    /// </summary>
    /// <remarks>
    /// The value returns the same value as <see cref="IDocument.UnitSystem"/>.
    /// </remarks>
    UnitSystem DocumentUnits { get; }

    /// <summary>
    /// Converts the given <paramref name="length"/> from <see cref="IDocument.UnitSystem"/>
    /// to the <see cref="InternalUnits"/> units.
    /// </summary>
    double ToInternalLength(double length);

    /// <summary>
    /// Converts the given <see cref="IUnitLength"/> to the <see cref=
    /// "InternalUnits"/> units.
    /// </summary>
    double ToInternalLength(IUnitLength length);

    /// <summary>
    /// Converts the given <see cref="IImperialLength"/> to the <see cref=
    /// "InternalUnits"/> units.
    /// </summary>
    double ToInternalLength(IImperialLength imperialLength);

    /// <summary>
    /// Converts the given <paramref name="area"/> from <see cref=
    /// "IDocument.UnitSystem"/> to the <see cref="InternalUnits"/> units.
    /// </summary>
    double ToInternalArea(double area);

    /// <summary>
    /// Converts the given <paramref name="internalLength"/> from <see cref="IDocument.UnitSystem"/>.
    /// </summary>
    double ToDocumentLength(double internalLength);

    /// <summary>
    /// Converts the given <paramref name="length"/> to the <see cref="DocumentUnits"/>
    /// units.
    /// </summary>
    double ToDocumentLength(IUnitLength length);

    /// <summary>
    /// Converts the given <paramref name="internalArea"/> to <see cref="IDocument.UnitSystem"/>.
    /// </summary>
    double ToDocumentArea(double internalArea);

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