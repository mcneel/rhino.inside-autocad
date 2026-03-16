using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IUnitScale"/>
/// <remarks>
/// This struct provides predefined unit scales for common measurement systems (metric, imperial, astronomical)
/// as well as support for custom unit definitions. Each unit scale contains a <see cref="Name"/> and a
/// <see cref="Ratio"/> representing meters per unit, enabling precise unit conversions without floating-point
/// precision loss.
/// </remarks>
/// <seealso cref="Ratio"/>
/// <seealso cref="RatioPerMeterDictionary"/>
/// <seealso cref="UnitSystem"/>
public readonly struct UnitScale : IUnitScale
{
    /// <summary>
    /// The default name used for custom unit definitions.
    /// </summary>
    private static readonly string _customUnitsName = $"{UnitSystem.CustomUnits}";

    #region Special Values

    /// <summary>
    /// Represents an unset or uninitialized unit scale.
    /// </summary>
    /// <remarks>
    /// The default value has a null <see cref="Name"/> and a <see cref="Ratio"/> of (0, 0) which evaluates to NaN.
    /// </remarks>
    public static readonly UnitScale Unset = default;

    /// <summary>
    /// Represents a unit scale indicating no specific units are defined.
    /// </summary>
    /// <remarks>
    /// Uses a <see cref="Ratio"/> of (-1, -1) from <see cref="RatioPerMeterDictionary"/> to indicate an invalid state.
    /// </remarks>
    public static readonly UnitScale None = new UnitScale(default, RatioPerMeterDictionary.Lookup(UnitSystem.None));

    #endregion

    #region Metric Units

    /// <summary>
    /// Unit scale for angstroms (10⁻¹⁰ meters).
    /// </summary>
    public static readonly UnitScale Angstroms = new UnitScale(UnitSystem.Angstroms);

    /// <summary>
    /// Unit scale for nanometers (10⁻⁹ meters).
    /// </summary>
    public static readonly UnitScale Nanometers = new UnitScale(UnitSystem.Nanometers);

    /// <summary>
    /// Unit scale for microns/micrometers (10⁻⁶ meters).
    /// </summary>
    public static readonly UnitScale Microns = new UnitScale(UnitSystem.Microns);

    /// <summary>
    /// Unit scale for millimeters (10⁻³ meters).
    /// </summary>
    public static readonly UnitScale Millimeters = new UnitScale(UnitSystem.Millimeters);

    /// <summary>
    /// Unit scale for centimeters (10⁻² meters).
    /// </summary>
    public static readonly UnitScale Centimeters = new UnitScale(UnitSystem.Centimeters);

    /// <summary>
    /// Unit scale for decimeters (10⁻¹ meters).
    /// </summary>
    public static readonly UnitScale Decimeters = new UnitScale(UnitSystem.Decimeters);

    /// <summary>
    /// Unit scale for meters (the SI base unit of length).
    /// </summary>
    public static readonly UnitScale Meters = new UnitScale(UnitSystem.Meters);

    /// <summary>
    /// Unit scale for dekameters (10¹ meters).
    /// </summary>
    public static readonly UnitScale Dekameters = new UnitScale(UnitSystem.Dekameters);

    /// <summary>
    /// Unit scale for hectometers (10² meters).
    /// </summary>
    public static readonly UnitScale Hectometers = new UnitScale(UnitSystem.Hectometers);

    /// <summary>
    /// Unit scale for kilometers (10³ meters).
    /// </summary>
    public static readonly UnitScale Kilometers = new UnitScale(UnitSystem.Kilometers);

    /// <summary>
    /// Unit scale for megameters (10⁶ meters).
    /// </summary>
    public static readonly UnitScale Megameters = new UnitScale(UnitSystem.Megameters);

    /// <summary>
    /// Unit scale for gigameters (10⁹ meters).
    /// </summary>
    public static readonly UnitScale Gigameters = new UnitScale(UnitSystem.Gigameters);

    #endregion

    #region Imperial Units

    /// <summary>
    /// Unit scale for microinches (10⁻⁶ inches).
    /// </summary>
    public static readonly UnitScale Microinches = new UnitScale(UnitSystem.Microinches);

    /// <summary>
    /// Unit scale for mils/thousandths of an inch (10⁻³ inches).
    /// </summary>
    public static readonly UnitScale Mils = new UnitScale(UnitSystem.Mils);

    /// <summary>
    /// Unit scale for inches (exactly 25.4 millimeters).
    /// </summary>
    public static readonly UnitScale Inches = new UnitScale(UnitSystem.Inches);

    /// <summary>
    /// Unit scale for feet (12 inches, exactly 0.3048 meters).
    /// </summary>
    public static readonly UnitScale Feet = new UnitScale(UnitSystem.Feet);

    /// <summary>
    /// Unit scale for yards (3 feet, exactly 0.9144 meters).
    /// </summary>
    public static readonly UnitScale Yards = new UnitScale(UnitSystem.Yards);

    /// <summary>
    /// Unit scale for statute miles (5,280 feet, exactly 1,609.344 meters).
    /// </summary>
    public static readonly UnitScale Miles = new UnitScale(UnitSystem.Miles);

    /// <summary>
    /// Unit scale for nautical miles (exactly 1,852 meters).
    /// </summary>
    public static readonly UnitScale NauticalMiles = new UnitScale(UnitSystem.NauticalMiles);

    #endregion

    #region Typographic Units

    /// <summary>
    /// Unit scale for printer points (1/72 inch).
    /// </summary>
    public static readonly UnitScale PrinterPoints = new UnitScale(UnitSystem.PrinterPoints);

    /// <summary>
    /// Unit scale for printer picas (1/6 inch, or 12 points).
    /// </summary>
    public static readonly UnitScale PrinterPicas = new UnitScale(UnitSystem.PrinterPicas);

    #endregion

    #region Astronomical Units

    /// <summary>
    /// Unit scale for astronomical units (mean Earth-Sun distance, approximately 149.6 billion meters).
    /// </summary>
    public static readonly UnitScale AstronomicalUnits = new UnitScale(UnitSystem.AstronomicalUnits);

    /// <summary>
    /// Unit scale for light-years (distance light travels in one Julian year).
    /// </summary>
    public static readonly UnitScale LightYears = new UnitScale(UnitSystem.LightYears);

    /// <summary>
    /// Unit scale for parsecs (parallax of one arcsecond, approximately 3.26 light-years).
    /// </summary>
    public static readonly UnitScale Parsecs = new UnitScale(UnitSystem.Parsecs);

    #endregion

    #region Regional/Historical Units

    /// <summary>
    /// Unit scale for US survey feet (1200/3937 meters exactly).
    /// </summary>
    /// <remarks>
    /// The US survey foot differs slightly from the international foot and is used in
    /// some US land survey and mapping applications.
    /// </remarks>
    internal static readonly UnitScale UsSurveyFeet = new UnitScale("US survey feet", new Ratio(1_200.0, 3_937.0));

    /// <summary>
    /// Unit scale for shaku (traditional Japanese unit of length, 10/33 meters).
    /// </summary>
    /// <remarks>
    /// The shaku is approximately 30.3 centimeters and is still used in traditional Japanese contexts.
    /// </remarks>
    internal static readonly UnitScale Shaku = new UnitScale("shaku", new Ratio(10.0, 33.0));

    #endregion

    #region Properties

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IRatio Ratio { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitScale"/> struct with a custom name and ratio.
    /// </summary>
    /// <param name="name">
    /// The display name for this unit scale.
    /// </param>
    /// <param name="metersPerUnit">
    /// A <see cref="Ratio"/> representing how many meters equal one unit.
    /// </param>
    public UnitScale(string name, Ratio metersPerUnit)
    {
        this.Name = name;
        this.Ratio = metersPerUnit;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitScale"/> struct from a <see cref="UnitSystem"/> value.
    /// </summary>
    /// <param name="system">
    /// The unit system to create a scale for.
    /// </param>
    /// <remarks>
    /// The <see cref="Name"/> is set to the string representation of the <see cref="UnitSystem"/>,
    /// and the <see cref="Ratio"/> is looked up from <see cref="RatioPerMeterDictionary"/>.
    /// </remarks>
    public UnitScale(UnitSystem system) : this(system.ToString(), RatioPerMeterDictionary.Lookup(system)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitScale"/> struct with a custom ratio.
    /// </summary>
    /// <param name="metersPerUnit">
    /// A <see cref="Ratio"/> representing how many meters equal one unit.
    /// </param>
    /// <remarks>
    /// The <see cref="Name"/> is automatically set to "CustomUnits".
    /// </remarks>
    public UnitScale(Ratio metersPerUnit) : this(_customUnitsName, metersPerUnit) { }

    #endregion

    /// <inheritdoc/>
    public bool IsEqualTo(IUnitScale other)
    {
        return this.Name == other.Name &&
               this.Ratio.Antecedent == other.Ratio.Antecedent &&
               this.Ratio.Consequent == other.Ratio.Consequent;
    }

    /// <inheritdoc/>
    public override string ToString() => this.Name;

}
