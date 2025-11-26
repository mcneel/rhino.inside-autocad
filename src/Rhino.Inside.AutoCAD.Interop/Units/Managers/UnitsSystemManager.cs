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
    public UnitSystem InternalUnits { get; }

    /// <inheritdoc/>
    public UnitSystem DocumentUnits { get; }

    /// <summary>
    /// Constructs a new <see cref="UnitSystemManager"/>.
    /// </summary>
    public UnitSystemManager(UnitSystem documentUnits, UnitSystem internalUnits)
    {
        _conversionLengthFactor = UnitConstants.LengthConversionFactors[documentUnits][internalUnits];

        _conversionAreaFactor = UnitConstants.AreaConversionFactors[documentUnits][internalUnits];

        this.InternalUnits = internalUnits;

        this.DocumentUnits = documentUnits;
    }

    /// <inheritdoc/>
    public double ToInternalLength(double length)
    {
        return length * _conversionLengthFactor;
    }

    /// <inheritdoc/>
    public double ToInternalLength(IUnitLength length)
    {
        var multiplier = UnitConstants.LengthConversionFactors[length.UnitSystem][this.InternalUnits];

        return length.Value * multiplier;
    }

    /// <inheritdoc/>
    public double ToInternalLength(IImperialLength imperialLength)
    {
        var majorLength = this.ToInternalLength(imperialLength.Feet);

        var minorLength = this.ToInternalLength(imperialLength.Inches);

        return majorLength + minorLength;
    }

    /// <inheritdoc/>
    public double ToInternalArea(double area)
    {
        return _conversionAreaFactor * area;
    }

    /// <inheritdoc/>
    public double ToDocumentLength(double internalLength)
    {
        return internalLength / _conversionLengthFactor;
    }

    /// <inheritdoc/>
    public double ToDocumentLength(IUnitLength length)
    {
        var multiplier = UnitConstants.LengthConversionFactors[length.UnitSystem][this.DocumentUnits];

        return length.Value * multiplier;
    }

    /// <inheritdoc/>
    public double ToDocumentArea(double internalArea)
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