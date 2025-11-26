namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a rotation in degrees which can be returned
/// in radians.
/// </summary>
public interface IDegreeAngle
{
    /// <summary>
    /// Event raised when the <see cref="Value"/> changes.
    /// </summary>
    event EventHandler? AngleChanged;

    /// <summary>
    /// The angle in degrees.
    /// </summary>
    double Value { get; set; }

    /// <summary>
    /// The min angle <see cref="Value"/> in radians that can be set.
    /// </summary>
    double MinAngle { get; }

    /// <summary>
    /// The max angle <see cref="Value"/> in radians that can be set.
    /// </summary>
    double MaxAngle { get; }

    /// <summary>
    /// Returns the <see cref="Value"/> in radians.
    /// </summary>
    double AsRadians();

    /// <summary>
    /// Sets the degree <see cref="Value"/> with the <paramref name="radians"/>
    /// value.
    /// </summary>
    void SetRadians(double radians);
}