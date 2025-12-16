using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadArc = Autodesk.AutoCAD.DatabaseServices.Arc;
using CadCircle = Autodesk.AutoCAD.DatabaseServices.Circle;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using CadDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using CadEllipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using CadExtents3d = Autodesk.AutoCAD.DatabaseServices.Extents3d;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadNurbsSurface = Autodesk.AutoCAD.DatabaseServices.NurbSurface;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using CadObjectIdCollection = Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;
using CadPolygonMesh = Autodesk.AutoCAD.DatabaseServices.PolygonMesh;
using CadPolygonMeshVertex = Autodesk.AutoCAD.DatabaseServices.PolygonMeshVertex;
using CadPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using CadPolyMeshType = Autodesk.AutoCAD.DatabaseServices.PolyMeshType;
using CadSubDMesh = Autodesk.AutoCAD.DatabaseServices.SubDMesh;
using CadSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using CadVertex3dType = Autodesk.AutoCAD.DatabaseServices.Vertex3dType;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoBoundingBox = Rhino.Geometry.BoundingBox;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoEllipse = Rhino.Geometry.Ellipse;
using RhinoHatch = Rhino.Geometry.Hatch;
using RhinoInterval = Rhino.Geometry.Interval;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoNurbsSurface = Rhino.Geometry.NurbsSurface;
using RhinoPlane = Rhino.Geometry.Plane;
using RhinoPoint = Rhino.Geometry.Point;
using RhinoPoint2d = Rhino.Geometry.Point2d;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;
using RhinoSurface = Rhino.Geometry.Surface;
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
    /// Converts a <see cref="CadLine"/> to a
    /// <see cref="RhinoLineCurve"/>.
    /// </summary>
    public RhinoLineCurve ToRhinoType(CadLine line)
    {
        var startPoint = this.ToRhinoType(line.StartPoint);

        var endPoint = this.ToRhinoType(line.EndPoint);

        return new RhinoLineCurve(startPoint, endPoint);
    }

    /// <summary>
    /// The number of knots must be is equal to D + N - 1. Where D = degree of the curve and
    /// N = the number of control points. Some applications such as AutoCAD may have a
    /// different number  of knots (e.g D + N + 1). It is the responsibility of this class
    /// to ensure the correct number of Knots is managed. This is done by assessing the
    /// knots multiplicity. A knot is a full multiplicity knot if it is duplicated as many
    /// times as the curve has degrees. 
    /// </summary>
    private List<double> GetKnots(List<double> inputKnots, int degree,
        int numberOfControlPoints)
    {
        var targetNumberOfKnots = degree + numberOfControlPoints - 1;

        if (inputKnots.Count == targetNumberOfKnots)
            return inputKnots;

        var outputKnots = new List<double>(targetNumberOfKnots);

        var currentKnotValue = inputKnots[0];
        var currentValueCount = 1;
        outputKnots.Add(currentKnotValue);

        //Skip first as already added
        for (var index = 1; index < inputKnots.Count; index++)
        {
            var inputKnot = inputKnots[index];
            if (Math.Abs(currentKnotValue - inputKnot) > _zeroTolerance)
            {
                currentKnotValue = inputKnot;
                currentValueCount = 1;
                outputKnots.Add(inputKnot);
                continue;
            }

            currentValueCount++;

            if (currentValueCount > degree)
                continue;

            outputKnots.Add(inputKnot);
        }

        return outputKnots;
    }

    /// <summary>
    /// Converts a <see cref="Spline"/> to a <see cref="RhinoNurbsCurve"/>.
    /// </summary>
    public RhinoNurbsCurve ToRhinoType(Spline spline)
    {
        var nurbsData = spline.NurbsData;

        var point3dCollection = nurbsData.GetControlPoints();

        var weights = nurbsData.GetWeights();

        var weightsCount = weights.Count;

        var nurbsDataKnots = nurbsData.GetKnots().ToArray().ToList();

        var knots = this.GetKnots(nurbsDataKnots, nurbsData.Degree, point3dCollection.Count);

        var rhinoNurbsCurve =
            new RhinoNurbsCurve(spline.Degree, point3dCollection.Count);

        for (var index = 0; index < point3dCollection.Count; index++)
        {

            var point3d = this.ToRhinoType(point3dCollection[index]);

            var weight = index < weightsCount ? weights[index] : 1.0;

            rhinoNurbsCurve.Points.SetPoint(index, point3d, weight);
        }

        var knotCount = knots.Count;

        var rhinoKnotCollection = rhinoNurbsCurve.Knots;

        for (var d = 0; d < knotCount; d++)
        {
            rhinoKnotCollection[d] = knots[d];
        }

        var rhinoInterval = new Rhino.Geometry.Interval(spline.StartParam, spline.EndParam);

        var trimmedNurbs = rhinoNurbsCurve.Trim(rhinoInterval);

        return trimmedNurbs as RhinoNurbsCurve ?? rhinoNurbsCurve;

    }

    /// <summary>
    /// Converts a <see cref="CadEllipse"/> to a <see cref="RhinoEllipse"/>.
    /// </summary>  
    public RhinoEllipse ToRhinoType(CadEllipse ellipse)
    {
        var centrePoint = this.ToRhinoType(ellipse.Center);

        var startPoint = ellipse.GetPointAtParameter(ellipse.StartParam);
        var endPoint = ellipse.GetPointAtParameter(ellipse.EndParam);

        var startPoint3d = this.ToRhinoType(startPoint);
        var endPoint3d = this.ToRhinoType(endPoint);

        var rhinoEllipse = new RhinoEllipse(centrePoint, startPoint3d, endPoint3d);

        return rhinoEllipse;
    }

    /// <summary>
    /// Converts a <see cref="CadArc"/> to a <see cref="RhinoArc"/>.
    /// </summary>
    public RhinoArc ToRhinoType(CadArc arc)
    {
        var plane = this.ToRhinoType(arc.GetPlane());

        var radius = _unitSystemManager.ToRhinoLength(arc.Radius);

        var sweep = arc.TotalAngle;

        var rhinoArc = new RhinoArc(plane, radius, sweep)
        {
            StartAngle = arc.StartAngle,
            Angle = sweep
        };

        return rhinoArc;
    }

    /// <summary>
    /// Converts a <see cref="CadCircle"/> to a <see cref="RhinoCircle"/>.
    /// </summary>
    public RhinoCircle ToRhinoType(CadCircle circle)
    {
        var origin = this.ToRhinoType(circle.Center);

        var radius = _unitSystemManager.ToRhinoLength(circle.Radius);

        var rhinoCircle = new RhinoCircle(origin, radius);

        return rhinoCircle;
    }

    /// <summary>
    /// Converts a <see cref="CadCircle"/> to a <see cref="RhinoCircle"/>.
    /// </summary>
    public RhinoPolyCurve ToRhinoType(CadPolyline polyline)
    {
        var vertexCount = polyline.NumberOfVertices;

        var polyCurve = new RhinoPolyCurve();

        for (var index = 0; index < vertexCount; index++)
        {
            var segmentType = polyline.GetSegmentType(index);

            switch (segmentType)
            {
                case SegmentType.Line:
                    {
                        var lineSegment2d = polyline.GetLineSegment2dAt(index);

                        var lineCurve = this.ToRhinoType(lineSegment2d);

                        polyCurve.Append(lineCurve);

                        break;
                    }
                case SegmentType.Arc:
                    {
                        var arcSegment2d = polyline.GetArcSegment2dAt(index);

                        var arcCurve = this.ToRhinoType(arcSegment2d);

                        polyCurve.Append(arcCurve);

                        break;
                    }
                default: continue;
            }
        }
        return polyCurve;
    }

    /// <summary>
    /// Converts a <see cref="CadCurve"/> to a <see cref="RhinoCurve"/>.
    /// </summary>
    public RhinoCurve? ToRhinoType(CadCurve curve)
    {
        switch (curve)
        {
            case CadLine line:
                return this.ToRhinoType(line);

            case Spline spline:
                return this.ToRhinoType(spline);

            case CadEllipse ellipse:
                var rhinoEllipse = this.ToRhinoType(ellipse);
                return rhinoEllipse.ToNurbsCurve();

            case CadArc arc:
                var rhinoArc = this.ToRhinoType(arc);

                return rhinoArc.ToNurbsCurve();

            case CadCircle circle:
                var rhinoCircle = this.ToRhinoType(circle);

                return rhinoCircle.ToNurbsCurve();

            case CadPolyline polyline:
                return this.ToRhinoType(polyline);
            default:
                return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="CadDBPoint"/> to a <see cref="RhinoPoint"/>.
    /// </summary>
    public RhinoPoint ToRhinoType(CadDBPoint point)
    {
        var point3d = this.ToRhinoType(point.Position);

        return new RhinoPoint(point3d);
    }

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

            var pathLocation = $@"{tempFolder}RhinoInsideAutocad\Converters\";

            Directory.CreateDirectory(pathLocation);

            var rhinoFilePath = $@"{pathLocation}autoCadToRhino.dxf";

            using var exportDatabase = new Database(true, true);

            if (solid.ObjectId.IsValid)
            {
                var sourceIds = new CadObjectIdCollection();
                sourceIds.Add(solid.ObjectId);

                var sourceDatabase = solid.ObjectId.Database;

                CadObjectId exportModelSpaceId;
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

                var clonedSolid = solid.Clone() as CadEntity;

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

    /// <summary>
    /// Converts a Rhino mesh into an AutoCAD PolyFaceMesh object.
    /// </summary>
    public RhinoMesh ToRhinoType(PolyFaceMesh mesh)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = this.ToRhinoType(mesh, transactionManagerWrapper);

        transaction.Commit();

        return result;
    }

    /// <summary>
    /// Converts a Rhino mesh into an AutoCAD PolyFaceMesh object.
    /// </summary>
    public RhinoMesh ToRhinoType(PolyFaceMesh mesh, ITransactionManager transactionManager)
    {
        var rhinoMesh = new RhinoMesh();

        try
        {
            var transaction = transactionManager.Unwrap();

            foreach (CadObjectId id in mesh)
            {
                var dbObject = transaction.GetObject(id, OpenMode.ForRead);

                switch (dbObject)
                {
                    case PolyFaceMeshVertex polyFaceMeshVertex:
                        {
                            var rhinoVertex = this.ToRhinoType(polyFaceMeshVertex.Position);

                            rhinoMesh.Vertices.Add(rhinoVertex);
                            continue;
                        }
                    case FaceRecord face when face.GetVertexAt(3) != 0:
                        rhinoMesh.Faces.AddFace(face.GetVertexAt(0), face.GetVertexAt(1),
                            face.GetVertexAt(2), face.GetVertexAt(3));
                        continue;

                    case FaceRecord face:
                        rhinoMesh.Faces.AddFace(face.GetVertexAt(0), face.GetVertexAt(1),
                            face.GetVertexAt(2));
                        break;
                }
            }
        }
        catch (System.Exception ex)
        {

        }

        return rhinoMesh;
    }

    /// <summary>
    /// Converts a <see cref="RhinoBoundingBox"/> to a <see cref="CadExtents3d"/>.
    /// </summary>
    public RhinoBoundingBox ToRhinoType(CadExtents3d extents)
    {
        var min = this.ToRhinoType(extents.MinPoint);

        var max = this.ToRhinoType(extents.MaxPoint);

        return new RhinoBoundingBox(min, max);
    }

    /// <summary>
    /// Converts a Rhino mesh into an AutoCAD SubDMesh object.
    /// </summary>
    public RhinoMesh ToRhinoSubD(CadSubDMesh mesh)
    {
        var rhinoMesh = new RhinoMesh();

        foreach (CadPoint3d point in mesh.Vertices)
            rhinoMesh.Vertices.Add(this.ToRhinoType(point));

        var edges = 0;

        for (var x = 0; x < mesh.FaceArray.Count; x = x + edges + 1)
        {
            edges = mesh.FaceArray[x];

            var faces = new List<int>();

            for (var y = x + 1; y <= x + edges; y++)

            {
                var faceInt = mesh.FaceArray[y];

                faces.Add(faceInt);
            }

            if (faces.Count == 4)
            {
                rhinoMesh.Faces.AddFace(faces[0], faces[1], faces[2], faces[3]);
                continue;
            }
            rhinoMesh.Faces.AddFace(faces[0], faces[1], faces[2]);
        }

        return rhinoMesh;
    }

    /// <summary>
    /// Converts an AutoCAD <see cref="CadPolygonMesh"/> into a Rhino <see cref="RhinoNurbsSurface"/> object.
    /// </summary>
    public RhinoNurbsSurface ToRhinoType(CadPolygonMesh mesh)
    {
        var degree = 1;
        switch (mesh.PolyMeshType)
        {
            case CadPolyMeshType.BezierSurfaceMesh:
                degree = 2;
                break;
            case CadPolyMeshType.CubicSurfaceMesh:
                degree = 3;
                break;
            case CadPolyMeshType.QuadSurfaceMesh:
                degree = 4;
                break;
            default:
                break;
        }

        var controlPointsU = mesh.MSize;
        var controlPointsV = mesh.NSize;

        var points = new List<RhinoPoint3d>();

        foreach (var meshItem in mesh)
        {
            if (meshItem is not CadPolygonMeshVertex vertex || vertex.VertexType != CadVertex3dType.ControlVertex) continue;

            var convertedPoint = this.ToRhinoType(vertex.Position);

            points.Add(convertedPoint);
        }

        var rhinoSurface = RhinoNurbsSurface.CreateFromPoints(points, controlPointsU,
            controlPointsV, degree, degree);

        return rhinoSurface;
    }

    /// <summary>
    /// Converts a <see cref="CadNurbsSurface"/> to a <see cref="RhinoNurbsSurface"/>.
    /// </summary>
    public RhinoNurbsSurface ToRhinoType(CadNurbsSurface cadNurbsSurface)
    {
        var dimension = 3;
        var degreeU = cadNurbsSurface.DegreeInU + 1;
        var degreeV = cadNurbsSurface.DegreeInV + 1;
        var isRational = cadNurbsSurface.IsRational;
        var controlPointsU = cadNurbsSurface.NumberOfControlPointsInU;
        var controlPointsV = cadNurbsSurface.NumberOfControlPointsInV;

        var rhinoSurface = RhinoNurbsSurface.Create(dimension, isRational, degreeU, degreeV, controlPointsU, controlPointsV);

        // Correct Knots from AutoCAD Nurbs Specification
        for (var index = 1; index < cadNurbsSurface.UKnots.Count - 1; index++)
        {
            var uKnot = cadNurbsSurface.UKnots[index];
            rhinoSurface.KnotsU[index - 1] = uKnot;
        }

        for (var index = 1; index < cadNurbsSurface.VKnots.Count - 1; index++)
        {
            var vKnot = cadNurbsSurface.VKnots[index];
            rhinoSurface.KnotsV[index - 1] = vKnot;
        }

        for (var u = 0; u < cadNurbsSurface.NumberOfControlPointsInU; u++)
        {
            for (var v = 0; v < cadNurbsSurface.NumberOfControlPointsInV; v++)
            {
                var controlPoint = cadNurbsSurface.GetControlPointAt(u, v);

                var convertedPoint = this.ToRhinoType(controlPoint);

                var weight = cadNurbsSurface.GetWeight(u, v);

                var rhinoControlPoint =
                    new Rhino.Geometry.ControlPoint(convertedPoint, weight);
                rhinoSurface.Points.SetControlPoint(u, v, rhinoControlPoint);

            }
        }

        return rhinoSurface;
    }



    /// <summary>
    /// Converts a <see cref="CadSurface"/> to a <see cref="RhinoSurface"/>.
    /// </summary>
    public RhinoSurface[] ToRhinoType(CadSurface cadSurface)
    {
        var nurbs = cadSurface.ConvertToNurbSurface();

        var converted = new List<RhinoSurface>();
        foreach (var nurbsSurface in nurbs)
        {
            converted.Add(this.ToRhinoType(nurbsSurface));
        }

        return converted.ToArray();

    }
    /// <summary>
    /// Creates a new <see cref="Autodesk.AutoCAD.DatabaseServices.Polyline"/> from the provide <paramref name=
    /// "bulgeVertexCollection"/> iterating vertices and adding them to the <see
    /// cref="Autodesk.AutoCAD.DatabaseServices.Polyline"/>.
    /// </summary>
    private CadPolyline CreatePolyline(BulgeVertexCollection bulgeVertexCollection)
    {
        var polyline = new CadPolyline();

        for (var index = 0; index < bulgeVertexCollection.Count; index++)
        {
            var vertex = bulgeVertexCollection[index];

            polyline.AddVertexAt(index, vertex.Vertex, vertex.Bulge, _zeroWidth, _zeroWidth);
        }

        return polyline;
    }

    /// <summary>
    /// Returns a <see cref="RhinoPolyCurve"/> from a <paramref name="bulgeVertexCollection"/>.
    /// First, a <see cref="Polyline"/> is created from the <paramref name=
    /// "bulgeVertexCollection"/> then iterating over segments of the <see cref=
    /// "Polyline"/> it creates <see cref="RhinoLineCurve"/> or <see cref="RhinoArcCurve"/> and
    /// appends them to the <see cref="RhinoPolyCurve"/>.
    /// </summary>
    private RhinoPolyCurve PolyCurveFromVertices(BulgeVertexCollection bulgeVertexCollection)
    {
        var polyline = this.CreatePolyline(bulgeVertexCollection);

        var polyCurve = new RhinoPolyCurve();
        for (var index = 0; index < polyline.NumberOfVertices; index++)
        {
            var segmentType = polyline.GetSegmentType(index);

            switch (segmentType)
            {
                case SegmentType.Line:
                    {
                        var lineSegment2d = polyline.GetLineSegment2dAt(index);

                        var lineCurve = this.ToRhinoType(lineSegment2d);

                        polyCurve.Append(lineCurve);

                        break;
                    }
                case SegmentType.Arc:
                    {
                        var arcSegment2d = polyline.GetArcSegment2dAt(index);

                        var arcCurve = this.ToRhinoType(arcSegment2d);

                        polyCurve.Append(arcCurve);

                        break;
                    }
                default: continue;
            }
        }

        return polyCurve;
    }

    /// <summary>
    /// Converts a <see cref="Curve2dCollection"/> to a <see cref="RhinoPolyCurve"/>.
    /// </summary>
    public RhinoPolyCurve ToRhinoType(Curve2dCollection cadCurveCollection)
    {
        var rhinoPolyCurve = new RhinoPolyCurve();

        foreach (var curve2d in cadCurveCollection.OfType<Curve2d>())
        {
            var internalCurve = this.ToRhinoType(curve2d);

            rhinoPolyCurve.Append(internalCurve);
        }

        return rhinoPolyCurve;

    }

    /// <summary>
    /// Converts a <see cref="HatchLoop"/> to a <see cref="RhinoPolyCurve"/>.
    /// </summary>
    public RhinoPolyCurve ToRhinoType(HatchLoop cadHatchLoop)
    {
        var loopCurves = cadHatchLoop.Curves;

        var isPolyLine = cadHatchLoop.IsPolyline;

        return isPolyLine
            ? this.PolyCurveFromVertices(cadHatchLoop.Polyline)
            : this.ToRhinoType(loopCurves);
    }

    /// <summary>
    /// Converts a <see cref="CadHatch"/> to a <see cref="RhinoHatch"/>.
    /// </summary>
    public RhinoHatch ToRhinoType(CadHatch cadHatch)
    {
        var cadPlane = cadHatch.GetPlane();

        var scale = _unitSystemManager.ToRhinoLength(cadHatch.PatternScale);

        var rotation = cadHatch.PatternAngle;

        //TODO: Support Hatch pattens
        var pattenIndex = 1;

        var hatchPlane = this.ToRhinoType(cadPlane);

        var rhinoLoops = new List<RhinoPolyCurve>();
        for (var i = 0; i < cadHatch.NumberOfLoops; i++)
        {
            var hatchLoop = cadHatch.GetLoopAt(i);

            var loopType = hatchLoop.LoopType;

            if ((loopType & _externalType) != _externalType &&
                (loopType & _outermostType) != _outermostType) continue;

            var loop = this.ToRhinoType(hatchLoop);

            rhinoLoops.Add(loop);
        }

        return RhinoHatch.Create(hatchPlane, rhinoLoops.FirstOrDefault(), rhinoLoops.Skip(1),
              pattenIndex, rotation, scale);
    }
}
