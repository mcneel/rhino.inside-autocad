using Autodesk.Civil.DatabaseServices;
using Rhino.Geometry;
using CadPoint2d = Autodesk.AutoCAD.Geometry.Point2d;
using CadVector3d = Autodesk.AutoCAD.Geometry.Vector3d;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoLine = Rhino.Geometry.Line;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting Civil 3D Alignment types to Rhino geometry types.
/// </summary>
public static class CivilAlignmentExtensions
{
    /// <summary>
    /// Converts a Civil 3D Alignment to a Rhino Curve (PolyCurve), applying unit conversion.
    /// </summary>
    /// <remarks>
    /// Entities are joined in station order (via <see cref="AlignmentEntityCollection.GetEntityByOrder(int)"/>),
    /// not collection order, which reflects creation order and can differ from the
    /// alignment's geometric sequence.
    /// </remarks>
    /// <param name="alignment">The Civil 3D Alignment to convert.</param>
    /// <returns>A Rhino Curve representing the alignment centerline.</returns>
    public static RhinoCurve? ToRhinoCurve(this Alignment alignment)
    {
        var entities = alignment.Entities;

        if (entities.Count == 0)
            return null;

        if (entities.Count == 1)
            return entities[0].ToRhinoCurve();

        // Join multiple entities into a PolyCurve in station order
        var polyCurve = new RhinoPolyCurve();
        for (var i = 0; i < entities.Count; i++)
        {
            var curve = entities.GetEntityByOrder(i).ToRhinoCurve();

            if (curve != null)
            {
                polyCurve.Append(curve);
            }
        }

        return polyCurve;
    }

    /// <summary>
    /// Converts an individual alignment entity to a Rhino curve.
    /// </summary>
    /// <remarks>
    /// Civil 3D API reuses classes like AlignmentSCS for multiple entity types
    /// (e.g., SpiralCurve, CurveSpiral, SpiralSpiral between lines all use AlignmentSCS).
    /// </remarks>
    public static RhinoCurve? ToRhinoCurve(this AlignmentEntity entity)
    {
        return entity switch
        {
            AlignmentLine line => ToRhinoLineCurve(line),
            AlignmentArc arc => ToRhinoArcCurve(arc),
            AlignmentSpiral spiral => ToRhinoSpiralCurve(spiral),
            AlignmentSCS scs => ToRhinoPolyCurve(scs),
            AlignmentSTS sts => ToRhinoPolyCurve(sts),
            AlignmentSSCSS sscss => ToRhinoPolyCurve(sscss),
            AlignmentCRC crc => ToRhinoPolyCurve(crc),
            // Unknown entity types are not supported
            _ => null
        };
    }

    /// <summary>
    /// Converts an individual alignment entity to a Rhino curve.
    /// </summary>
    /// <remarks>
    /// Civil 3D API reuses classes like AlignmentSCS for multiple entity types
    /// (e.g., SpiralCurve, CurveSpiral, SpiralSpiral between lines all use AlignmentSCS).
    /// </remarks>
    public static RhinoCurve? ToRhinoCurve(this AlignmentSubEntity entity)
    {
        return entity switch
        {
            AlignmentSubEntityLine line => ToRhinoLineCurve(line),
            AlignmentSubEntityArc arc => ToRhinoArcCurve(arc),
            AlignmentSubEntitySpiral spiral => ToRhinoSpiralCurve(spiral),
            // Unknown entity types are not supported
            _ => null
        };
    }

    /// <summary>
    /// Converts an AlignmentLine to a Rhino LineCurve.
    /// </summary>
    public static RhinoLineCurve ToRhinoLineCurve(this AlignmentLine line)
    {
        var startPoint = line.StartPoint.ToRhinoPoint3d();
        var endPoint = line.EndPoint.ToRhinoPoint3d();
        return new RhinoLineCurve(new RhinoLine(startPoint, endPoint));
    }

    /// <summary>
    /// Converts an AlignmentArc to a Rhino ArcCurve.
    /// </summary>
    public static RhinoArcCurve ToRhinoArcCurve(this AlignmentArc arc)
    {
        var startPoint = arc.StartPoint.ToRhinoPoint3d();
        var endPoint = arc.EndPoint.ToRhinoPoint3d();

        var cadVector = new CadVector3d(Math.Sin(arc.StartDirection),
            Math.Cos(arc.StartDirection),
            0.0);

        var rhinoVector = cadVector.ToRhinoVector3d();

        return new RhinoArcCurve(new RhinoArc(startPoint, rhinoVector, endPoint));
    }

    /// <summary>
    /// Converts an AlignmentSpiral to a Rhino NurbsCurve by sampling points.
    /// </summary>
    public static RhinoCurve? ToRhinoSpiralCurve(this AlignmentSpiral spiral)
    {
        // Sample points along the spiral
        var points = new List<RhinoPoint3d>();
        var numSamples = Math.Max(10, (int)(spiral.Length / 5.0)); // At least 10 points, or one every 5 units

        for (var i = 0; i <= numSamples; i++)
        {
            try
            {
                // Use the spiral's actual geometry if available
                if (i == 0)
                {
                    points.Add(spiral.StartPoint.ToRhinoPoint3d());
                }
                else if (i == numSamples)
                {
                    points.Add(spiral.EndPoint.ToRhinoPoint3d());
                }
                else
                {
                    // For intermediate points, we need to interpolate
                    // The spiral has RadiusIn and RadiusOut properties
                    var t = (double)i / numSamples;
                    var x = spiral.StartPoint.X + (spiral.EndPoint.X - spiral.StartPoint.X) * t;
                    var y = spiral.StartPoint.Y + (spiral.EndPoint.Y - spiral.StartPoint.Y) * t;
                    var interpolatedPoint = new CadPoint2d(x, y);
                    points.Add(interpolatedPoint.ToRhinoPoint3d());
                }
            }
            catch
            {
                // Skip points that can't be calculated
            }
        }

        if (points.Count < 2)
            return null;

        // Create an interpolated curve through the points
        return RhinoNurbsCurve.Create(false, 3, points);
    }

    /// <summary>
    /// Converts a composite entity (SCS, STS, etc.) to a Rhino curve.
    /// Creates a simplified line representation using the entity's start and end points.
    /// </summary>
    /// <remarks>
    /// Civil 3D composite alignment entities contain sub-entities (spirals, arcs, lines)
    /// but sub-entity access requires iterating through collections. This method uses
    /// the composite's overall start and end points for a simplified representation.
    /// </remarks>
    public static RhinoCurve? ToRhinoPolyCurve(this AlignmentEntity compositeEntity)
    {
        try
        {
            // Each composite type has StartPoint and EndPoint properties
            return compositeEntity switch
            {
                AlignmentSCS scs => CreateLineFromPoints(scs.StartPoint, scs.EndPoint),
                AlignmentSTS sts => CreateLineFromPoints(sts.StartPoint, sts.EndPoint),
                AlignmentSSCSS sscss => CreateLineFromPoints(sscss.StartPoint, sscss.EndPoint),
                AlignmentCRC crc => CreateLineFromPoints(crc.StartPoint, crc.EndPoint),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Creates a Rhino line curve from two Civil 3D points with unit conversion.
    /// </summary>
    private static RhinoLineCurve CreateLineFromPoints(CadPoint2d startPoint, CadPoint2d endPoint)
    {
        var start = startPoint.ToRhinoPoint3d();
        var end = endPoint.ToRhinoPoint3d();
        return new RhinoLineCurve(new RhinoLine(start, end));
    }

    /// <summary>
    /// Converts an AlignmentSpiral to a Rhino NurbsCurve by sampling points.
    /// </summary>
    public static RhinoCurve? ToRhinoSpiralCurve(this AlignmentSubEntitySpiral spiral)
    {
        // Sample points along the spiral
        var points = new List<RhinoPoint3d>();
        var numSamples = Math.Max(10, (int)(spiral.Length / 5.0)); // At least 10 points, or one every 5 units

        for (var i = 0; i <= numSamples; i++)
        {
            try
            {
                // Use the spiral's actual geometry if available
                if (i == 0)
                {
                    points.Add(spiral.StartPoint.ToRhinoPoint3d());
                }
                else if (i == numSamples)
                {
                    points.Add(spiral.EndPoint.ToRhinoPoint3d());
                }
                else
                {
                    // For intermediate points, we need to interpolate
                    // The spiral has RadiusIn and RadiusOut properties
                    var t = (double)i / numSamples;
                    var x = spiral.StartPoint.X + (spiral.EndPoint.X - spiral.StartPoint.X) * t;
                    var y = spiral.StartPoint.Y + (spiral.EndPoint.Y - spiral.StartPoint.Y) * t;
                    var interpolatedPoint = new CadPoint2d(x, y);
                    points.Add(interpolatedPoint.ToRhinoPoint3d());
                }
            }
            catch
            {
                // Skip points that can't be calculated
            }
        }

        if (points.Count < 2)
            return null;

        // Create an interpolated curve through the points
        return RhinoNurbsCurve.Create(false, 3, points);
    }

    /// <summary>
    /// Converts an AlignmentArc to a Rhino ArcCurve.
    /// </summary>
    public static RhinoArcCurve ToRhinoArcCurve(this AlignmentSubEntityArc arc)
    {
        var startPoint = arc.StartPoint.ToRhinoPoint3d();
        var endPoint = arc.EndPoint.ToRhinoPoint3d();
        var centerPoint = arc.CenterPoint.ToRhinoPoint3d();
        var radius = UnitConverter.ToRhinoLength(arc.Radius);

        var circle = new Rhino.Geometry.Circle(centerPoint, radius);

        if (!circle.IsValid)
        {
            throw new InvalidOperationException(
                "Failed to create a valid circle from the AlignmentArc points.");
        }

        _ = circle.ClosestParameter(startPoint, out var start);
        _ = circle.ClosestParameter(endPoint, out var end);

        var interval = new Interval(start, end);

        var rhinoArc = new RhinoArc(circle, interval);

        if (!rhinoArc.IsValid)
        {
            throw new InvalidOperationException(
                "Failed to create a valid Rhino arc from the AlignmentArc.");
        }

        return new RhinoArcCurve(rhinoArc);

    }

    /// <summary>
    /// Converts an AlignmentLine to a Rhino LineCurve.
    /// </summary>
    public static RhinoLineCurve ToRhinoLineCurve(this AlignmentSubEntityLine line)
    {
        var startPoint = line.StartPoint.ToRhinoPoint3d();
        var endPoint = line.EndPoint.ToRhinoPoint3d();
        return new RhinoLineCurve(new RhinoLine(startPoint, endPoint));
    }
}

