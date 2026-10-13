using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoLine = Rhino.Geometry.Line;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoPoint3d = Rhino.Geometry.Point3d;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <summary>
/// Wraps a parabola (vertical curve) entity extracted from a Civil 3D Profile.
/// </summary>
/// <remarks>
/// This wrapper provides access to parabola-specific properties like K value,
/// PVI station and elevation, and high/low point, in addition to the base entity properties.
/// </remarks>
public class CivilProfileAsymmetricalParabolaWrapper : CivilProfileEntityWrapper, ICivilProfileAsymmetricalParabola
{
    private readonly ProfileParabolaAsymmetric _parabola;
    /// <inheritdoc />
    public double PVIStation { get; }

    /// <inheritdoc />
    public double PVIElevation { get; }

    /// <inheritdoc />
    public double HighLowPointStation { get; }

    /// <inheritdoc />
    public double HighLowPointElevation { get; }

    /// <inheritdoc />
    public double GradeOut { get; }

    /// <inheritdoc />
    public double GradeIn { get; }

    /// <inheritdoc />
    public double AsymmetricLength1 { get; }

    /// <inheritdoc />
    public double AsymmetricLength2 { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CivilProfileAsymmetricalParabolaWrapper"/>.
    /// </summary>
    /// <param name="parabola"></param>
    /// <param name="entityIndex">The index of this entity in the profile's entity collection.</param>
    public CivilProfileAsymmetricalParabolaWrapper(ProfileParabolaAsymmetric parabola, int entityIndex)
        : base(parabola, entityIndex)
    {
        _parabola = parabola;

        this.PVIStation = parabola.PVIStation;
        this.PVIElevation = parabola.PVIElevation;
        this.HighLowPointStation = parabola.HighLowPointStation;
        this.HighLowPointElevation = parabola.HighLowPointElevation;
        this.GradeIn = parabola.GradeIn;
        this.GradeOut = parabola.GradeOut;
        this.AsymmetricLength1 = parabola.AsymmetricLength1;
        this.AsymmetricLength2 = parabola.AsymmetricLength2;

    }

    /// <summary>
    /// Calculates the elevation at a given station along the asymmetric parabolic
    /// vertical curve.
    /// </summary>
    /// <remarks>
    /// An asymmetric vertical curve is two parabolas joined tangentially at the CVC,
    /// which lies at the PVI station. With A = BVC, P = PVI and E = EVC, the CVC is
    /// C = B + L1/(L1 + L2)*(D - B), where B and D are the midpoints of A-P and P-E,
    /// L1 is the BVC-to-PVI length and L2 is the PVI-to-EVC length. The slope at C is
    /// the slope of B-D. Each half is evaluated as
    /// elevation = yStart + gStart*x + (gEnd - gStart)*x^2 / (2*Lhalf), where x is
    /// measured from the start of that half. Grades are derived from the BVC, PVI and
    /// EVC geometry, so sag and crest curves are both handled.
    /// </remarks>
    /// <param name="station">The station at which to evaluate the elevation.</param>
    /// <returns>The elevation of the curve at <paramref name="station"/>.</returns>
    public double CalculateElevation(double station)
    {
        var startStation = this.Start.Station;
        var startElevation = this.Start.Elevation;

        var length1 = this.PVIStation - startStation;
        var length2 = this.End.Station - this.PVIStation;
        var length = this.End.Station - startStation;

        if (length1 <= 0.0 || length2 <= 0.0)
        {
            // Degenerate curve: interpolate linearly between the end points.
            return length > 0.0
                ? startElevation + (this.End.Elevation - startElevation) * (station - startStation) / length
                : startElevation;
        }

        var gradeIn = (this.PVIElevation - startElevation) / length1;
        var gradeOut = (this.End.Elevation - this.PVIElevation) / length2;

        // Slope at the CVC is the slope of the chord between the tangent midpoints,
        // which spans half the total curve length.
        var midpointInElevation = 0.5 * (startElevation + this.PVIElevation);
        var midpointOutElevation = 0.5 * (this.PVIElevation + this.End.Elevation);
        var gradeCvc = (midpointOutElevation - midpointInElevation) / (0.5 * length);

        if (station <= this.PVIStation)
        {
            var x1 = station - startStation;
            return startElevation + gradeIn * x1 + (gradeCvc - gradeIn) * x1 * x1 / (2.0 * length1);
        }

        var cvcElevation = startElevation + 0.5 * length1 * (gradeIn + gradeCvc);
        var x2 = station - this.PVIStation;
        return cvcElevation + gradeCvc * x2 + (gradeOut - gradeCvc) * x2 * x2 / (2.0 * length2);
    }

    /// <summary>
    /// Creates a duplicate of this profile parabola wrapper.
    /// </summary>
    /// <returns>A new instance with copied data.</returns>
    public override CivilProfileEntityWrapper ShallowClone()
    {
        return new CivilProfileAsymmetricalParabolaWrapper(_parabola, this.EntityIndex);
    }

    /// <summary>
    /// Converts this asymmetric parabola to an exact Rhino curve in the
    /// (station, elevation) plane.
    /// </summary>
    /// <remarks>
    /// The curve is two quadratic Beziers, [A, B, C] and [C, D, E], joined at the CVC.
    /// A = BVC, P = PVI, E = EVC, B and D are the midpoints of A-P and P-E, and
    /// C = B + L1/(L1 + L2)*(D - B) lies at the PVI station. The halves are built as a
    /// single degree-2 NURBS curve with control points A, B, C, D, E and a double
    /// interior knot at L1, making it tangent-continuous at C. It passes exactly through
    /// the entity's start and end points with tangents along the incoming and outgoing
    /// grades. No sampling is involved. A degenerate curve, where the PVI is not strictly
    /// between the end stations, is returned as a straight line.
    /// </remarks>
    public override RhinoCurve ToRhinoCurve()
    {
        var startStation = this.Start.Station;
        var endStation = this.End.Station;

        var length1 = this.PVIStation - startStation;
        var length2 = endStation - this.PVIStation;

        var a = CreatePoint(startStation, this.Start.Elevation);
        var e = CreatePoint(endStation, this.End.Elevation);

        if (length1 <= 0.0 || length2 <= 0.0)
            return new RhinoLineCurve(new RhinoLine(a, e));

        var p = CreatePoint(this.PVIStation, this.PVIElevation);
        var b = 0.5 * (a + p);
        var d = 0.5 * (p + e);
        var c = b + length1 / (length1 + length2) * (d - b);

        const int degree = 2;
        var curve = new RhinoNurbsCurve(3, false, degree + 1, 5);

        curve.Points.SetPoint(0, a);
        curve.Points.SetPoint(1, b);
        curve.Points.SetPoint(2, c);
        curve.Points.SetPoint(3, d);
        curve.Points.SetPoint(4, e);

        var totalLength = length1 + length2;
        curve.Knots[0] = 0.0;
        curve.Knots[1] = 0.0;
        curve.Knots[2] = length1;
        curve.Knots[3] = length1;
        curve.Knots[4] = totalLength;
        curve.Knots[5] = totalLength;

        return curve;
    }

    /// <summary>
    /// Creates a Rhino point from a station and elevation, converted to Rhino units.
    /// </summary>
    private static RhinoPoint3d CreatePoint(double station, double elevation)
    {
        return new RhinoPoint3d(
            UnitConverter.ToRhinoLength(station),
            UnitConverter.ToRhinoLength(elevation),
            0);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Profile Parabola (Start:[{this.Start}] - End:[{this.End}], PVI: {this.PVIStation:F2} @ {this.PVIElevation:F2})";
    }
}
