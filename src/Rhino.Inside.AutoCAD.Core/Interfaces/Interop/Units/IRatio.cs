namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines the contract for a ratio represented as a pair of double-precision floating-point numbers.
/// </summary>
/// <remarks>
/// A ratio maintains both the numerator (<see cref="Antecedent"/>) and denominator (<see cref="Consequent"/>)
/// separately to preserve precision during arithmetic operations.
/// </remarks>
public interface IRatio
{
    #region Core Properties

    /// <summary>
    /// Gets the numerator (dividend) of the ratio.
    /// </summary>
    /// <seealso cref="Consequent"/>
    /// <seealso cref="Quotient"/>
    double Antecedent { get; }

    /// <summary>
    /// Gets the denominator (divisor) of the ratio.
    /// </summary>
    /// <seealso cref="Antecedent"/>
    /// <seealso cref="Quotient"/>
    double Consequent { get; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets the quotient of <see cref="Antecedent"/> divided by <see cref="Consequent"/>.
    /// </summary>
    /// <remarks>
    /// This is the standard division result (a / b).
    /// </remarks>
    /// <seealso cref="Remainder"/>
    /// <seealso cref="EuclideanQuotient"/>
    double Quotient { get; }

    /// <summary>
    /// Gets the remainder of <see cref="Antecedent"/> divided by <see cref="Consequent"/>.
    /// </summary>
    /// <remarks>
    /// This is the standard modulo result (a % b).
    /// </remarks>
    /// <seealso cref="Quotient"/>
    /// <seealso cref="EuclideanRemainder"/>
    double Remainder { get; }

    /// <summary>
    /// Gets the Euclidean quotient, which is the <see cref="Quotient"/> rounded to the nearest integer.
    /// </summary>
    /// <seealso cref="Quotient"/>
    /// <seealso cref="EuclideanRemainder"/>
    double EuclideanQuotient { get; }

    /// <summary>
    /// Gets the IEEE remainder of <see cref="Antecedent"/> divided by <see cref="Consequent"/>.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Math.IEEERemainder"/> which returns the remainder resulting from
    /// the division of a specified number by another specified number.
    /// </remarks>
    /// <seealso cref="Remainder"/>
    /// <seealso cref="EuclideanQuotient"/>
    double EuclideanRemainder { get; }

    #endregion

    #region State Properties

    /// <summary>
    /// Gets a value indicating whether this ratio represents a negative value.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Quotient"/> is negative; otherwise, <c>false</c>.
    /// Returns <c>false</c> for NaN values.
    /// </value>
    /// <seealso cref="IsPositive"/>
    bool IsNegative { get; }

    /// <summary>
    /// Gets a value indicating whether this ratio represents a positive value.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Quotient"/> is positive; otherwise, <c>false</c>.
    /// Returns <c>false</c> for NaN values.
    /// </value>
    /// <seealso cref="IsNegative"/>
    bool IsPositive { get; }

    /// <summary>
    /// Gets a value indicating whether this ratio represents a finite value.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Quotient"/> is finite (not infinity and not NaN); otherwise, <c>false</c>.
    /// </value>
    /// <seealso cref="IsInfinity"/>
    /// <seealso cref="IsNaN"/>
    bool IsFinite { get; }

    /// <summary>
    /// Gets a value indicating whether this ratio evaluates to positive or negative infinity.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Quotient"/> is <see cref="double.PositiveInfinity"/>
    /// or <see cref="double.NegativeInfinity"/>; otherwise, <c>false</c>.
    /// </value>
    /// <seealso cref="IsPositiveInfinity"/>
    /// <seealso cref="IsNegativeInfinity"/>
    bool IsInfinity { get; }

    /// <summary>
    /// Gets a value indicating whether this ratio evaluates to positive infinity.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Quotient"/> is <see cref="double.PositiveInfinity"/>; otherwise, <c>false</c>.
    /// </value>
    /// <seealso cref="IsNegativeInfinity"/>
    /// <seealso cref="IsInfinity"/>
    bool IsPositiveInfinity { get; }

    /// <summary>
    /// Gets a value indicating whether this ratio evaluates to negative infinity.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Quotient"/> is <see cref="double.NegativeInfinity"/>; otherwise, <c>false</c>.
    /// </value>
    /// <seealso cref="IsPositiveInfinity"/>
    /// <seealso cref="IsInfinity"/>
    bool IsNegativeInfinity { get; }

    /// <summary>
    /// Gets a value indicating whether this ratio is not a number (NaN).
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Quotient"/> is NaN; otherwise, <c>false</c>.
    /// </value>
    /// <seealso cref="IsFinite"/>
    bool IsNaN { get; }

    #endregion
}