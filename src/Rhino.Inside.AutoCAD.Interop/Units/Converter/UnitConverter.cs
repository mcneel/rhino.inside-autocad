using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IUnitConverter"/>
public class UnitConverter : IUnitConverter
{
    /// <summary>
    /// A cached conversion factor for length units between the AutoCAD and Rhino unit systems.
    /// </summary>
    private readonly double _conversionLengthFactor;

    /// <summary>
    /// Returns the <see cref="UnitConverter"/> singleton instance.
    /// </summary>
    public static UnitConverter? Instance { get; private set; }

    /// <inheritdoc/>
    public IUnitScale RhinoUnits { get; }

    /// <inheritdoc/>
    public IUnitScale AutoCadUnits { get; }

    /// <summary>
    /// Explicit static constructor to tell C# compiler
    /// not to mark type as beforefieldinit.
    /// </summary>
    static UnitConverter() { }

    /// <summary>
    /// Constructs a new <see cref="UnitConverter"/>.
    /// </summary>
    private UnitConverter(UnitSystem autocadUnits, UnitSystem rhinoUnits) : this(
        new UnitScale(autocadUnits), new UnitScale(rhinoUnits))
    {
    }

    /// <summary>
    /// Constructs a new <see cref="UnitConverter"/>.
    /// </summary>
    private UnitConverter(IUnitScale autocadUnits, IUnitScale rhinoUnits)
    {
        _conversionLengthFactor = this.GetConversionFactor(autocadUnits, rhinoUnits);

        this.RhinoUnits = rhinoUnits;

        this.AutoCadUnits = autocadUnits;
    }

    /// <summary>
    /// Initializes the <see cref="UnitConverter"/> singleton.
    /// </summary>
    public static void Initialize(UnitSystem autocadUnits, UnitSystem rhinoUnits)
    {
        Instance = new UnitConverter(autocadUnits, rhinoUnits);
    }

    /// <summary>
    /// Initializes the <see cref="UnitConverter"/> singleton.
    /// </summary>
    public static void Initialize(IUnitScale autocadUnits, IUnitScale rhinoUnits)
    {
        Instance = new UnitConverter(autocadUnits, rhinoUnits);
    }

    /// <summary>
    /// Extracts the conversion factor between two unit scales based on their ratios
    /// to a common base unit (meters).
    /// </summary>
    private double GetConversionFactor(IUnitScale from, IUnitScale to)
    {
        if (from.Ratio == to.Ratio)
            return 1.0;

        var fromNumerator = from.Ratio.Antecedent;
        var fromDenominator = from.Ratio.Consequent;

        var toNumerator = to.Ratio.Antecedent;
        var toDenominator = to.Ratio.Consequent;

        var numerator = fromNumerator * toDenominator;
        var denominator = fromDenominator * toNumerator;

        return numerator / denominator;
    }

    /// <summary>
    /// Converts a length from AutoCAD units to Rhino units using the singleton instance.
    /// </summary>
    public static double ToRhinoLength(double length)
    {
        return length * Instance?._conversionLengthFactor
               ?? throw new InvalidOperationException("UnitConverter has not been initialized.");
    }

    /// <summary>
    /// Converts a length from Rhino units to AutoCAD units using the singleton instance.
    /// </summary>
    public static double ToAutoCadLength(double length)
    {
        return length / Instance?._conversionLengthFactor
               ?? throw new InvalidOperationException("UnitConverter has not been initialized.");
    }
}