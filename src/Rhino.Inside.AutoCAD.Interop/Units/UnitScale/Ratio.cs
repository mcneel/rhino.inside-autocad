using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Runtime.CompilerServices;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRatio"/>
/// <remarks>
/// This struct provides precise ratio arithmetic by maintaining the numerator (<see cref="Antecedent"/>)
/// and denominator (<see cref="Consequent"/>) separately, avoiding precision loss that can occur
/// when working with floating-point quotients directly. It supports rationalization of decimal values,
/// reciprocal operations, and arithmetic with both ratios and scalar values.
/// </remarks>
/// <seealso cref="RatioPerMeterDictionary"/>
/// <seealso cref="UnitScale"/>
public readonly struct Ratio : IRatio, IEquatable<Ratio>
{
    /// <summary>
    /// The tolerance used for floating-point equality comparisons.
    /// </summary>
    private const double _tolerance = GeometryConstants.RatioTolerance;

    #region Static Fields

    /// <summary>
    /// Represents the smallest possible value of a <see cref="Ratio"/>.
    /// </summary>
    public static readonly Ratio MinValue = new Ratio(double.MinValue);

    /// <summary>
    /// Represents the largest possible value of a <see cref="Ratio"/>.
    /// </summary>
    public static readonly Ratio MaxValue = new Ratio(double.MaxValue);

    /// <summary>
    /// Represents the smallest positive <see cref="Ratio"/> value that is greater than zero.
    /// </summary>
    public static readonly Ratio Epsilon = new Ratio(double.Epsilon);

    /// <summary>
    /// Represents negative infinity as a <see cref="Ratio"/>.
    /// </summary>
    public static readonly Ratio NegativeInfinityValue = new Ratio(double.NegativeInfinity);

    /// <summary>
    /// Represents positive infinity as a <see cref="Ratio"/>.
    /// </summary>
    public static readonly Ratio PositiveInfinityValue = new Ratio(double.PositiveInfinity);

    /// <summary>
    /// Represents a value that is not a number (NaN).
    /// </summary>
    /// <remarks>
    /// The default value of <see cref="Ratio"/> results in NaN because both
    /// <see cref="Antecedent"/> and <see cref="Consequent"/> are zero.
    /// </remarks>
    public static readonly Ratio NaNValue = default;

    #endregion

    #region IEEE 754 Bit Masks

    /// <summary>
    /// Bit mask for extracting the sign bit from an IEEE 754 double-precision value.
    /// </summary>
    private const ulong SignMask = 0x8000_0000_0000_0000UL;

    /// <summary>
    /// Bit mask for extracting the exponent bits from an IEEE 754 double-precision value.
    /// </summary>
    private const ulong ExponentMask = 0x7FF0_0000_0000_0000UL;

    #endregion

    #region Private Static Helpers

    /// <summary>
    /// Determines whether the specified double value is finite.
    /// </summary>
    /// <param name="value">
    /// The double value to evaluate.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is finite; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsFiniteValue(double value)
    {
        return ((ulong)BitConverter.DoubleToInt64Bits(value) & ~SignMask) < ExponentMask;
    }

    /// <summary>
    /// Determines whether the specified double value is negative.
    /// </summary>
    /// <param name="value">
    /// The double value to evaluate.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is negative; otherwise, <c>false</c>.
    /// Returns <c>false</c> for NaN values.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNegativeValue(double value)
    {
        if (double.IsNaN(value)) return false;
        return ((ulong)BitConverter.DoubleToInt64Bits(value) & SignMask) != 0UL;
    }

    /// <summary>
    /// Determines whether the specified double value is positive.
    /// </summary>
    /// <param name="value">
    /// The double value to evaluate.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is positive; otherwise, <c>false</c>.
    /// Returns <c>false</c> for NaN values.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPositiveValue(double value)
    {
        if (double.IsNaN(value)) return false;
        return ((ulong)BitConverter.DoubleToInt64Bits(value) & SignMask) == 0UL;
    }

    #endregion

    #region Core Properties

    /// <inheritdoc/>
    public double Antecedent { get; }

    /// <inheritdoc/>
    public double Consequent { get; }

    #endregion

    #region Computed Properties

    /// <inheritdoc/>
    public double Quotient => this.Antecedent / this.Consequent;

    /// <inheritdoc/>
    public double Remainder => this.Antecedent % this.Consequent;

    /// <inheritdoc/>
    public double EuclideanQuotient => Math.Round(this.Antecedent / this.Consequent);

    /// <inheritdoc/>
    public double EuclideanRemainder => Math.IEEERemainder(this.Antecedent, this.Consequent);

    #endregion

    #region State Properties

    /// <inheritdoc/>
    public bool IsNegative => IsNegativeValue(this.Quotient);

    /// <inheritdoc/>
    public bool IsPositive => IsPositiveValue(this.Quotient);

    /// <inheritdoc/>
    public bool IsFinite => IsFiniteValue(this.Quotient);

    /// <inheritdoc/>
    public bool IsInfinity => double.IsInfinity(this.Quotient);

    /// <inheritdoc/>
    public bool IsPositiveInfinity => double.IsPositiveInfinity(this.Quotient);

    /// <inheritdoc/>
    public bool IsNegativeInfinity => double.IsNegativeInfinity(this.Quotient);

    /// <inheritdoc/>
    public bool IsNaN => double.IsNaN(this.Quotient);

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Ratio"/> struct with the specified value as the antecedent
    /// and 1.0 as the consequent.
    /// </summary>
    /// <param name="value">
    /// The value to use as the <see cref="Antecedent"/>.
    /// </param>
    /// <remarks>
    /// This constructor creates a ratio equivalent to the scalar value (value / 1.0).
    /// </remarks>
    public Ratio(double value)
    {
        this.Antecedent = value;
        this.Consequent = 1.0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ratio"/> struct with the specified antecedent and consequent.
    /// </summary>
    /// <param name="antecedent">
    /// The numerator (dividend) of the ratio.
    /// </param>
    /// <param name="consequent">
    /// The denominator (divisor) of the ratio.
    /// </param>
    public Ratio(double antecedent, double consequent)
    {
        this.Antecedent = antecedent;
        this.Consequent = consequent;
    }

    #endregion

    #region Equality

    /// <inheritdoc/>
    public override bool Equals(object other) => other is Ratio ratio && this.Equals(ratio);

    /// <inheritdoc/>
    /// <remarks>
    /// Equality is based on the <see cref="Quotient"/> value within tolerance, not the individual components.
    /// For example, 1/2 equals 2/4.
    /// </remarks>
    public bool Equals(Ratio other) => Math.Abs(this.Quotient - other.Quotient) < _tolerance;

    /// <inheritdoc/>
    public override int GetHashCode() => this.Quotient.GetHashCode();

    /// <summary>
    /// Determines whether two ratios are equal based on their quotients within tolerance.
    /// </summary>
    /// <param name="left">
    /// The first ratio to compare.
    /// </param>
    /// <param name="right">
    /// The second ratio to compare.
    /// </param>
    /// <returns>
    /// <c>true</c> if both ratios have quotients within <see cref="GeometryConstants.RatioTolerance"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Ratio left, Ratio right) => Math.Abs(left.Quotient - right.Quotient) < _tolerance;

    /// <summary>
    /// Determines whether two ratios are not equal based on their quotients.
    /// </summary>
    /// <param name="left">
    /// The first ratio to compare.
    /// </param>
    /// <param name="right">
    /// The second ratio to compare.
    /// </param>
    /// <returns>
    /// <c>true</c> if the ratios have quotients differing by more than <see cref="GeometryConstants.RatioTolerance"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Ratio left, Ratio right) => Math.Abs(left.Quotient - right.Quotient) >= _tolerance;

    #endregion
}
