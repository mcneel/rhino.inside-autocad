using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Interop;

public class GeometryConstants
{
    /// <summary>
    /// The fit tolerance used for nurbs and spline curves.
    /// </summary>
    public const double FitTolerance = 0.001;

    /// <summary>
    /// Normalized length of the <see cref="Curve"/> middle <see cref="Point3d"/>.
    /// </summary>
    public const double NormalizedMidLength = 0.5;

    /// <summary>
    /// Tolerance threshold for zero length comparison. values below this threshold
    /// are considered zero.
    /// </summary>
    public const double ZeroTolerance = 0.0001;

    /// <summary>
    /// The tolerance in radians which can be used to compare two angles for
    /// equality. Differences below this threshold are considered equal.  
    /// </summary>
    public const double RadianAngleTolerance = 0.0008726646;

    /// <summary>
    /// Absolute zero value.
    /// </summary>
    public const double AbsoluteZeroValue = 0d;

}