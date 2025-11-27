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

}