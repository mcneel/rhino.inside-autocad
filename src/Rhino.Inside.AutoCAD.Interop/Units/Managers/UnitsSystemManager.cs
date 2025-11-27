using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IUnitSystemManager"/>
public class UnitSystemManager : IUnitSystemManager
{
    // The conversion factor used to convert from the document units system
    // to the internal units system lengths.
    private readonly double _conversionLengthFactor;
    private readonly double _conversionAreaFactor;

    private readonly double _pi = Math.PI;

    /// <inheritdoc/>
    public UnitSystem RhinoUnits { get; }

    /// <inheritdoc/>
    public UnitSystem AutoCadUnits { get; }

    /// <summary>
    /// Constructs a new <see cref="UnitSystemManager"/>.
    /// </summary>
    public UnitSystemManager(UnitSystem autocadUnits, UnitSystem rhinoUnits)
    {
        _conversionLengthFactor = UnitConstants.LengthConversionFactors[autocadUnits][rhinoUnits];

        _conversionAreaFactor = UnitConstants.AreaConversionFactors[autocadUnits][rhinoUnits];

        this.RhinoUnits = rhinoUnits;

        this.AutoCadUnits = autocadUnits;
    }

    /// <inheritdoc/>
    public double ToRhinoLength(double length)
    {
        return length * _conversionLengthFactor;
    }

    /// <inheritdoc/>
    public double ToRhinoLength(IUnitLength length)
    {
        var multiplier = UnitConstants.LengthConversionFactors[length.UnitSystem][this.RhinoUnits];

        return length.Value * multiplier;
    }

    /// <inheritdoc/>
    public double ToRhinoLength(IImperialLength imperialLength)
    {
        var majorLength = this.ToRhinoLength(imperialLength.Feet);

        var minorLength = this.ToRhinoLength(imperialLength.Inches);

        return majorLength + minorLength;
    }

    /// <inheritdoc/>
    public double ToRhinoArea(double area)
    {
        return _conversionAreaFactor * area;
    }

    /// <inheritdoc/>
    public double ToAutoCadLength(double internalLength)
    {
        return internalLength / _conversionLengthFactor;
    }

    /// <inheritdoc/>
    public double ToAutoCadLength(IUnitLength length)
    {
        var multiplier = UnitConstants.LengthConversionFactors[length.UnitSystem][this.AutoCadUnits];

        return length.Value * multiplier;
    }

    /// <inheritdoc/>
    public double ToAutoCadArea(double internalArea)
    {
        return internalArea / _conversionAreaFactor;
    }

    /// <inheritdoc/>
    public double Convert(double value, UnitSystem source, UnitSystem destination)
    {
        var multiplier = UnitConstants.LengthConversionFactors[source][destination];

        return value * multiplier;
    }

    /// <inheritdoc/>
    public double ToDegrees(double radians)
    {
        return (180.0 / _pi) * radians;
    }

    /// <inheritdoc/>
    public double ToRadians(double degrees)
    {
        return (_pi / 180.0) * degrees;
    }
}