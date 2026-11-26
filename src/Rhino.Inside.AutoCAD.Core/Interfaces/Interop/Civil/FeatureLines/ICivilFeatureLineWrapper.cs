using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a standalone (grading) Civil 3D Feature Line.
/// </summary>
/// <remarks>
/// Distinct from <see cref="ICivilCorridorFeatureLine"/>, which is extracted from a
/// corridor baseline and cannot be selected or edited on its own.
/// </remarks>
public interface ICivilFeatureLineWrapper
{
    /// <summary>
    /// Gets the properties of the feature line.
    /// </summary>
    ICivilFeatureLineProperties Properties { get; }

    /// <summary>
    /// Gets the feature line as a Rhino curve, with arcs preserved in plan.
    /// </summary>
    /// <returns>The curve, or null if the feature line has no usable geometry.</returns>
    Curve? GetCurve();

    /// <summary>
    /// Gets the PI (horizontal geometry) points of the feature line.
    /// </summary>
    List<Point3d> GetPIPoints();

    /// <summary>
    /// Gets the elevation points of the feature line (grade breaks between PIs).
    /// </summary>
    List<Point3d> GetElevationPoints();
}
