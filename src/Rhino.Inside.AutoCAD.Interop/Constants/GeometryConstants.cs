using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides standard tolerance and precision constants for geometric operations.
/// </summary>
/// <remarks>
/// These constants define the precision thresholds used throughout the interop layer
/// when converting geometry between Rhino and AutoCAD, performing comparisons,
/// and fitting curves to control points.
/// </remarks>
public class GeometryConstants
{
    /// <summary>
    /// The fit tolerance used when approximating NURBS and spline curves.
    /// </summary>
    /// <remarks>
    /// This tolerance (0.001) controls how closely a fitted curve must match the original
    /// geometry. Smaller values produce more accurate but potentially more complex curves.
    /// </remarks>
    /// <seealso cref="NurbsCurve"/>
    public const double FitTolerance = 0.001;

    /// <summary>
    /// The normalized parameter value representing the midpoint of a <see cref="Curve"/>.
    /// </summary>
    /// <remarks>
    /// A value of 0.5 corresponds to the exact center along the curve's domain,
    /// commonly used when evaluating the midpoint <see cref="Point3d"/> of a curve.
    /// </remarks>
    public const double NormalizedMidLength = 0.5;

    /// <summary>
    /// Tolerance threshold for determining if a length is effectively zero.
    /// </summary>
    /// <remarks>
    /// Values below this threshold (0.0001) are treated as zero-length,
    /// which helps avoid division-by-zero errors and degenerate geometry conditions.
    /// </remarks>
    /// <seealso cref="VertexTolerance"/>
    public const double ZeroTolerance = 0.0001;

    /// <summary>
    /// A minimal tolerance for ratio and proportion comparisons.
    /// </summary>
    /// <remarks>
    /// This extremely small value (1e-10) accounts for floating-point precision errors
    /// when comparing ratios or normalized values that should theoretically be equal.
    /// </remarks>
    public const double RatioTolerance = 1e-10;

    /// <summary>
    /// Tolerance for comparing vertex positions, such as <see cref="Mesh"/> vertices or control points.
    /// </summary>
    /// <remarks>
    /// Points within this distance (0.0001) are considered coincident.
    /// Used for vertex welding, duplicate detection, and geometric comparisons.
    /// </remarks>
    public const double VertexTolerance = 0.0001;

    /// <summary>
    /// The tolerance used when determining if a curve segment is too short to be represented as a line or arc.
    /// </summary>
    public const double ShortCurveTolernace = 0.0005;

    /// <summary>
    /// The angular tolerance used when fitting curves and comparing angles, expressed in radians.
    /// </summary>
    public const double AngleTolernace = Math.PI / 1800.0;

    /// <summary>
    /// The chord tolerance used when sampling a curve into a preview polyline, as a fraction
    /// of the curve's bounding box diagonal.
    /// </summary>
    /// <remarks>
    /// Value: 0.001. Relative to the curve's own size rather than absolute, so a preview is
    /// equally faithful in millimetre and metre models and a long alignment is not sampled
    /// at a model tolerance meant for small details. Only affects the preview, never baking.
    /// </remarks>
    /// <seealso cref="PreviewAngleTolerance"/>
    public const double PreviewChordToleranceRatio = 0.001;

    /// <summary>
    /// The angle tolerance, in radians, used when sampling a curve into a preview polyline.
    /// </summary>
    /// <remarks>
    /// Value: 2 degrees, so a full circle previews with about 180 segments. This bounds the
    /// segment count on large arcs, where <see cref="PreviewChordToleranceRatio"/> alone
    /// would allow visibly faceted curves.
    /// </remarks>
    /// <seealso cref="PreviewChordToleranceRatio"/>
    public const double PreviewAngleTolerance = Math.PI / 90.0;

    /// <summary>
    /// The minimum segment length used when sampling a curve into a preview polyline.
    /// </summary>
    /// <remarks>
    /// Value: 0, meaning no minimum, so short curves are never collapsed.
    /// </remarks>
    public const double PreviewMinimumSegmentLength = 0.0;

    /// <summary>
    /// The maximum segment length used when sampling a curve into a preview polyline.
    /// </summary>
    /// <remarks>
    /// Value: 0, meaning no maximum, so straight spans stay a single segment.
    /// </remarks>
    public const double PreviewMaximumSegmentLength = 0.0;

    /// <summary>
    /// The display density used when meshing a <see cref="SubD"/> for preview.
    /// </summary>
    /// <remarks>
    /// Value: 2, which divides each SubD face into a 4 by 4 grid of quads. Higher values
    /// quadruple the shell size per step for little visual gain in a preview.
    /// </remarks>
    public const int PreviewSubDDisplayDensity = 2;

    /// <summary>
    /// The meshing density used when meshing a Brep or surface for preview, from 0 (coarse
    /// and fast) to 1 (dense and slow).
    /// </summary>
    /// <remarks>
    /// Value: 0, the coarsest, matching Rhino's "jagged and faster" render mesh. Only used
    /// when the geometry carries no cached render mesh, which is typical of Grasshopper
    /// output, so meshing speed matters more than smoothness.
    /// </remarks>
    public const double PreviewMeshDensity = 0.0;

}