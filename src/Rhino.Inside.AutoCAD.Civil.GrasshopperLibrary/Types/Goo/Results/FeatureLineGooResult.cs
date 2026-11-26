using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// Represents the result of extracting data from a Civil 3D Feature Line.
/// </summary>
public class FeatureLineGooResult : GooResultBase
{
    /// <summary>
    /// Gets the properties of the Feature Line.
    /// </summary>
    public GH_CivilFeatureLineProperties? PropertiesGoo { get; }

    /// <summary>
    /// Gets the feature line as a Rhino curve.
    /// </summary>
    public Curve? Curve { get; }

    /// <summary>
    /// Gets the PI points of the Feature Line.
    /// </summary>
    public List<Point3d> PIPoints { get; }

    /// <summary>
    /// Gets the elevation points of the Feature Line.
    /// </summary>
    public List<Point3d> ElevationPoints { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureLineGooResult"/> class.
    /// </summary>
    /// <param name="propertiesGoo">The properties of the Feature Line.</param>
    /// <param name="curve">The feature line as a Rhino curve.</param>
    /// <param name="piPoints">The PI points of the Feature Line.</param>
    /// <param name="elevationPoints">The elevation points of the Feature Line.</param>
    public FeatureLineGooResult(
        GH_CivilFeatureLineProperties? propertiesGoo,
        Curve? curve,
        List<Point3d> piPoints,
        List<Point3d> elevationPoints)
    {
        PropertiesGoo = propertiesGoo;
        Curve = curve;
        PIPoints = piPoints;
        ElevationPoints = elevationPoints;
    }

    /// <summary>
    /// Gets a failed result instance.
    /// </summary>
    public static FeatureLineGooResult Failed => new(null, null, new List<Point3d>(), new List<Point3d>()) { IsSuccess = false };
}
