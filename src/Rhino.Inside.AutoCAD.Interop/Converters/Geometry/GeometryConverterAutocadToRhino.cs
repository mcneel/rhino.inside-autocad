using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoEllipse = Rhino.Geometry.Ellipse;
using RhinoInterval = Rhino.Geometry.Interval;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoPlane = Rhino.Geometry.Plane;
using RhinoPoint2d = Rhino.Geometry.Point2d;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;
using RhinoVector3d = Rhino.Geometry.Vector3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A geometry converter class for converting between AutoCAD and Rhino geometry
/// types. The converter provides singleton access to the converter instance
/// for global use. The converter uses the <see cref="IUnitSystemManager"/>
/// to convert all units to <see cref="IUnitSystemManager.RhinoUnits"/> units.
/// </summary>
public partial class GeometryConverter
{
    /// <summary>
    /// Converts the given <see cref="Curve2d"/> to a Rhino
    /// <see cref="RhinoCurve"/>.
    /// </summary>
    public RhinoCurve? ToRhinoType(Curve2d curve)
    {
        switch (curve)
        {
            case LineSegment2d line:
                return this.ToRhinoType(line);

            case Line2d line2d:
                return this.ToRhinoType(line2d);

            case CircularArc2d circularArc2d:
                var arc = this.ToRhinoType(circularArc2d);

                var arcCurve = new RhinoArcCurve(arc);

                return arcCurve;

            case SplineEntity2d splineEntity2d:
                return this.ToRhinoType(splineEntity2d);

            case EllipticalArc2d ellipticalArc2d:
                var ellipse = this.ToRhinoType(ellipticalArc2d, out var interval);

                var nurbs = ellipse.ToNurbsCurve();

                var nurbsTrimmed = nurbs.Trim(interval);

                var ellipseNurbs = nurbsTrimmed == null ? nurbs : (RhinoNurbsCurve)nurbsTrimmed;

                var endPoint = this.ToRhinoType3d(ellipticalArc2d.EndPoint);

                var startParameter = interval.T0;
                var endParameter = interval.T1;

                var samplePoints = ellipticalArc2d.GetSamplePoints(startParameter, endParameter, interval.Length).ToList();

                var hasSamplePoints = samplePoints.Count > 0;

                var endTangent = hasSamplePoints
                    ? this.ToRhinoType3d(samplePoints.Last().GetDerivative(1, endParameter))
                    : new RhinoVector3d(0, 0, 0);

                ellipseNurbs.SetEndCondition(true, RhinoNurbsCurve.NurbsCurveEndConditionType.Position, endPoint, endTangent);

                return ellipseNurbs;

            default:
                return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="Solid3d"/> to an array of Rhino <see cref="RhinoBrep"/>s.
    /// </summary>
    public RhinoBrep[] ToRhinoType(Solid3d solid)
    {

        var addedObjects = new List<RhinoBrep>();

        try
        {
            var tempFolder = Path.GetTempPath();

            var pathLocation = $@"{tempFolder}AWI\Converters\";

            Directory.CreateDirectory(pathLocation);

            var rhinoFilePath = $@"{pathLocation}autoCadToRhino.dxf";

            using var exportDatabase = new Database(true, true);

            if (solid.ObjectId.IsValid)
            {
                var sourceIds = new ObjectIdCollection();
                sourceIds.Add(solid.ObjectId);

                var sourceDatabase = solid.ObjectId.Database;

                Autodesk.AutoCAD.DatabaseServices.ObjectId exportModelSpaceId;
                using (var readOnlyTrans = exportDatabase.TransactionManager.StartTransaction())
                {
                    var blockTable = readOnlyTrans.GetObject(exportDatabase.BlockTableId, OpenMode.ForRead) as BlockTable;

                    exportModelSpaceId = blockTable[BlockTableRecord.ModelSpace];

                    readOnlyTrans.Commit();
                }

                var mapping = new IdMapping();

                sourceDatabase.WblockCloneObjects(sourceIds, exportModelSpaceId, mapping,
                    DuplicateRecordCloning.Replace, false);
            }
            else
            {
                using var transaction = exportDatabase.TransactionManager.StartTransaction();

                var blockTable = transaction.GetObject(exportDatabase.BlockTableId, OpenMode.ForRead) as BlockTable;

                var blockTableRecord = transaction.GetObject(blockTable![BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                var clonedSolid = solid.Clone() as Autodesk.AutoCAD.DatabaseServices.Entity;

                clonedSolid!.SetDatabaseDefaults(exportDatabase);

                blockTableRecord!.AppendEntity(clonedSolid);

                transaction.AddNewlyCreatedDBObject(clonedSolid, true);

                transaction.Commit();
            }

            exportDatabase.DxfOutEx(rhinoFilePath, 16, DwgVersion.AC1024, false, 0);

            using var headless = Rhino.RhinoDoc.CreateHeadless(null);

            headless.Import(rhinoFilePath);

            var selectedObjects = headless.Objects;

            foreach (var selectedObject in selectedObjects)
            {
                if (selectedObject.Geometry is not RhinoBrep brep) continue;

                addedObjects.Add(brep.DuplicateBrep());
            }
        }
        catch (System.Exception ex)
        {

        }

        return addedObjects.ToArray();
    }

    /// <summary>
    /// Converts a <see cref="LineSegment2d"/> to a <see cref="RhinoLineCurve"/>.
    /// </summary>
    /// <remarks>
    /// A line with a defined start and end point (not infinite).
    /// </remarks>
    public RhinoLineCurve ToRhinoType(LineSegment2d lineSegment2d)
    {
        var startPoint = this.ToRhinoType(lineSegment2d.StartPoint);
        var endPoint = this.ToRhinoType(lineSegment2d.EndPoint);

        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts a <see cref="Line2d"/> to a <see cref="RhinoLineCurve"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="Line2d"/> are infinite which isn't supported in Rhino. The
    /// <see cref="Line2d.PointOnLine"/> is used as the start point and the
    /// endpoint is the unit translation of its direction - so a 1.0 length line
    /// in the <see cref="IUnitSystemManager.RhinoUnits"/>.
    /// </remarks>
    public RhinoLineCurve ToRhinoType(Line2d line2d)
    {
        var hasStartPoint = line2d.HasStartPoint;

        var startPoint = hasStartPoint ? this.ToRhinoType(line2d.StartPoint) : this.ToRhinoType(line2d.PointOnLine);

        var hasEndPoint = line2d.HasEndPoint;

        RhinoPoint2d endPoint;
        if (hasEndPoint)
        {
            endPoint = this.ToRhinoType(line2d.EndPoint);
        }
        else
        {
            var vector3d = this.ToRhinoType3d(line2d.Direction);

            var translation = Rhino.Geometry.Transform.Translation(vector3d);

            endPoint = new RhinoPoint2d(startPoint);

            endPoint.Transform(translation);
        }

        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// Converts a <see cref="NurbCurve2d"/> to a <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    public RhinoNurbsCurve ToRhinoType(NurbCurve2d nurbCurve2d)
    {
        return this.ToRhinoType((SplineEntity2d)nurbCurve2d);
    }

    /// <summary>
    /// Converts a <see cref="SplineEntity2d"/> to a <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    public RhinoNurbsCurve ToRhinoType(SplineEntity2d spline2d)
    {
        var pointCount = spline2d.NumControlPoints;

        var rhinoPoints = new List<RhinoPoint3d>();
        for (var i = 0; i < pointCount; i++)
        {
            var controlPoint = spline2d.GetControlPointAt(i);

            var point3d = this.ToRhinoType3d(controlPoint);

            rhinoPoints.Add(point3d);
        }

        var isPeriodic = spline2d.IsPeriodic(out _);

        var rhinoSpline = RhinoNurbsCurve.CreateSubDFriendly(rhinoPoints, false, isPeriodic);

        return rhinoSpline;
    }

    /// <summary>
    /// Converts a <see cref="CircularArc2d"/> to a <see cref="RhinoCurve"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="RhinoInterval"/> is returned as there is no direct way
    /// to convert 
    /// </remarks>
    public RhinoEllipse ToRhinoType(EllipticalArc2d ellipticalArc2d, out RhinoInterval interval)
    {
        var centrePoint = this.ToRhinoType3d(ellipticalArc2d.Center);
        var majorAxis = this.ToRhinoType3d(ellipticalArc2d.MajorAxis);
        var minorAxis = this.ToRhinoType3d(ellipticalArc2d.MinorAxis);

        var majorRadius = _unitSystemManager.ToRhinoLength(ellipticalArc2d.MajorRadius);
        var minorRadius = _unitSystemManager.ToRhinoLength(ellipticalArc2d.MinorRadius);

        var plane = new RhinoPlane(centrePoint, majorAxis, minorAxis);

        var rhinoEllipse = new RhinoEllipse(plane, majorRadius, minorRadius);

        var ellipticalArc2dInterval = ellipticalArc2d.GetInterval();

        interval = new RhinoInterval(ellipticalArc2dInterval.LowerBound, ellipticalArc2dInterval.UpperBound);

        return rhinoEllipse;
    }

    /// <summary>
    /// Converts a <see cref="CircularArc2d"/> to a <see cref="RhinoArc"/>.
    /// </summary>
    public RhinoArc ToRhinoType(CircularArc2d circularArc2d)
    {
        var arcStartPoint = circularArc2d.StartPoint;

        var startPoint = this.ToRhinoType3d(circularArc2d.StartPoint);
        var endPoint = this.ToRhinoType3d(circularArc2d.EndPoint);

        // Vector has to be negated as the Rhino arc is drawn in the opposite direction.
        var tangentVector = circularArc2d.GetTangent(arcStartPoint).Direction.Negate();

        var rhinoVector = this.ToRhinoType3d(tangentVector);

        var rhinoArc = new RhinoArc(startPoint, rhinoVector, endPoint);

        return rhinoArc;
    }

    /// <summary>
    /// Converts a <see cref="CompositeCurve2d"/> to a <see cref="RhinoPolyCurve"/>.
    /// </summary>
    public RhinoPolyCurve ToRhinoType(CompositeCurve2d compositeCurve2d)
    {
        var curves = compositeCurve2d.GetCurves();

        var rhinoPolyCurve = new RhinoPolyCurve();
        foreach (var curve2d in curves)
        {
            var rhinoCurve = this.ToRhinoType(curve2d);

            if (rhinoCurve != null)
                rhinoPolyCurve.Append(rhinoCurve);
        }

        return rhinoPolyCurve;
    }
}