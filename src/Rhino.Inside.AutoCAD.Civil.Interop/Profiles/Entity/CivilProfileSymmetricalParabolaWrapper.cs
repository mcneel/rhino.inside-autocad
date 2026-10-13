using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;
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
public class CivilProfileSymmetricalParabolaWrapper : CivilProfileEntityWrapper, ICivilProfileSymmetricalParabola
{
    private readonly ProfileParabolaSymmetric _parabola;

    /// <inheritdoc />
    public double KValue { get; }

    /// <inheritdoc />
    public double PVIStation { get; }

    /// <inheritdoc />
    public double PVIElevation { get; }

    /// <inheritdoc />
    public double HighLowPointStation { get; }

    /// <inheritdoc />
    public double HighLowPointElevation { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CivilProfileSymmetricalParabolaWrapper"/>.
    /// </summary>
    /// <param name="parabola"></param>
    /// <param name="entityIndex">The index of this entity in the profile's entity collection.</param>
    public CivilProfileSymmetricalParabolaWrapper(ProfileParabolaSymmetric parabola, int entityIndex)
        : base(parabola, entityIndex)
    {
        _parabola = parabola;
        this.KValue = parabola.K;
        this.PVIStation = parabola.PVIStation;
        this.PVIElevation = parabola.PVIElevation;
        this.HighLowPointStation = parabola.HighLowPointStation;
        this.HighLowPointElevation = parabola.HighLowPointElevation;

    }

    /// <summary>
    /// Calculates the elevation at a given station along the symmetric parabolic
    /// vertical curve.
    /// </summary>
    /// <remarks>
    /// Uses the standard vertical curve equation measured from the BVC:
    /// elevation = e0 + g1*x + (g2 - g1)*x^2 / (2L), where x is the distance from the
    /// BVC station, g1 is the grade from the BVC to the PVI, g2 is the grade from the
    /// PVI to the EVC, and L is the curve length. Grades are derived from the BVC, PVI
    /// and EVC geometry, so sag and crest curves are both handled. Stations outside the
    /// curve are extrapolated along the same parabola.
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

        var x = station - startStation;

        return startElevation + gradeIn * x + (gradeOut - gradeIn) * x * x / (2.0 * length);
    }

    /// <summary>
    /// Creates a duplicate of this profile parabola wrapper.
    /// </summary>
    /// <returns>A new instance with copied data.</returns>
    public override CivilProfileEntityWrapper ShallowClone()
    {
        return new CivilProfileSymmetricalParabolaWrapper(_parabola, this.EntityIndex);
    }

    /// <summary>
    /// Converts this symmetric parabola to an exact Rhino curve in the
    /// (station, elevation) plane.
    /// </summary>
    /// <remarks>
    /// A parabolic vertical curve is exactly a quadratic Bezier whose control points
    /// are the BVC, the PVI (the tangent intersection) and the EVC, so the result is a
    /// degree-2 NURBS curve with those three control points. It passes exactly through
    /// the entity's start and end points with tangents along the incoming and outgoing
    /// grades. No sampling is involved.
    /// </remarks>
    public override RhinoCurve ToRhinoCurve()
    {
        var points = new List<RhinoPoint3d>
        {
            CreatePoint(this.Start.Station, this.Start.Elevation),
            CreatePoint(this.PVIStation, this.PVIElevation),
            CreatePoint(this.End.Station, this.End.Elevation)
        };

        return RhinoNurbsCurve.Create(false, 2, points);
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
        return $"Profile Parabola (Start:[{this.Start}] - End:[{this.End}], K: {this.KValue:F2}, PVI: {this.PVIStation:F2} @ {this.PVIElevation:F2})";
    }
}
