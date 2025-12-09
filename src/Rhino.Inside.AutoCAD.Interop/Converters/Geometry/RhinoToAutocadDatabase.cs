using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadArc = Autodesk.AutoCAD.DatabaseServices.Arc;
using CadCircle = Autodesk.AutoCAD.DatabaseServices.Circle;
using CadCircularArc3d = Autodesk.AutoCAD.Geometry.CircularArc3d;
using CadCompositeCurve3d = Autodesk.AutoCAD.Geometry.CompositeCurve3d;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using CadCurve3d = Autodesk.AutoCAD.Geometry.Curve3d;
using CadDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using CadEllipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using CadExtents3d = Autodesk.AutoCAD.DatabaseServices.Extents3d;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadNurbsSurface = Autodesk.AutoCAD.DatabaseServices.NurbSurface;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;
using CadPolyFaceMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;
using CadPolygonMesh = Autodesk.AutoCAD.DatabaseServices.PolygonMesh;
using CadPolygonMeshVertex = Autodesk.AutoCAD.DatabaseServices.PolygonMeshVertex;
using CadPolyline3d = Autodesk.AutoCAD.DatabaseServices.Polyline3d;
using CadPolyMeshType = Autodesk.AutoCAD.DatabaseServices.PolyMeshType;
using CadSolid3d = Autodesk.AutoCAD.DatabaseServices.Solid3d;
using CadSpline = Autodesk.AutoCAD.DatabaseServices.Spline;
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
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoNurbsSurface = Rhino.Geometry.NurbsSurface;
using RhinoPoint = Rhino.Geometry.Point;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;
using RhinoPolyLineCurve = Rhino.Geometry.PolylineCurve;
using RhinoSubD = Rhino.Geometry.SubD;
using RhinoSurface = Rhino.Geometry.Surface;

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
    /// Converts the given <see cref="RhinoCurve"/> to a list of AutoCAD
    /// <see cref="Geometry.Curve"/>s. Typically, the list contains one <see cref="Geometry.Curve"/>
    /// if successful, however it can contain more if the <see cref="RhinoPolyCurve"/>
    /// is converted. It is the <see cref="RhinoPolyCurve"/> conversion which drives
    /// the need to return a list as AutoCAD does not have a direct equivalent.
    /// </summary>
    public IList<CadCurve> ToAutoCadType(RhinoCurve curve)
    {
        switch (curve)
        {
            case RhinoLineCurve line:
                return [this.ToAutoCadType(line)];

            case RhinoArcCurve arc:
                {
                    if (arc.IsCompleteCircle == false)
                        return [this.ToAutoCadType(arc)];

                    var circle = new RhinoCircle(arc.Arc);

                    return [this.ToAutoCadType(circle)];
                }

            case RhinoNurbsCurve nurbsCurve:
                return [this.ToAutoCadType(nurbsCurve)];

            case RhinoPolyLineCurve polyLineCurve:
                return [this.ToAutoCadType(polyLineCurve)];

            case RhinoPolyCurve polyCurve:
                return this.ToAutoCadType(polyCurve);

            default:
                return [];
        }
    }

    /// <summary>
    /// Converts the given <see cref="RhinoCurve"/> into a single composite curve. This
    /// method provide a second processing step after the ToAutoCadType where each curve
    /// in the converted list is appended into a single geometric composite curve.
    /// CreateFromGeCurve can fail in many cases so this method should be used with caution.
    /// Where possible, use the list of curves returned by <see cref="ToAutoCadType(RhinoCurve)"/>
    /// </summary>
    public CadCurve ToAutoCadSingleCurve(RhinoCurve curve)
    {
        var listCurves = this.ToAutoCadType(curve);

        var curve3ds = new CadCurve3d[listCurves.Count];
        for (var index = 0; index < listCurves.Count; index++)
        {
            var convertedCurve = listCurves[index];

            var curve3d = convertedCurve.GetGeCurve();

            curve3ds[index] = curve3d;
        }

        var compositeCurve = new CadCompositeCurve3d(curve3ds);

        return CadCurve.CreateFromGeCurve(compositeCurve);
    }

    /// <summary>
    /// Converts a <see cref="RhinoLineCurve"/> to a <see cref="CadLine"/>.
    /// </summary>
    public CadLine ToAutoCadType(RhinoLineCurve line)
    {
        var startPoint = this.ToAutoCadType(line.PointAtStart);

        var endPoint = this.ToAutoCadType(line.PointAtEnd);

        return new CadLine(startPoint, endPoint);
    }

    /// <summary>
    /// Converts a <see cref="RhinoNurbsCurve"/> to a <see cref="CadSpline"/>.
    /// </summary>
    public CadSpline ToAutoCadType(RhinoNurbsCurve nurbsCurve)
    {
        var cadControlPoints = new CadPoint3dCollection();
        var weightCollection = new DoubleCollection();
        foreach (var rhinoControlPoint in nurbsCurve.Points)
        {
            var cadPoint = this.ToAutoCadType(rhinoControlPoint.Location);

            var weight = rhinoControlPoint.Weight;
            weightCollection.Add(weight);
            cadControlPoints.Add(cadPoint);
        }

        var rhinoKnotsArray = nurbsCurve.Knots.ToArray();
        var knotCollection = new DoubleCollection(rhinoKnotsArray.Length + 2);

        //Correct Knots from Rhino Nurbs Specification
        knotCollection.Add(rhinoKnotsArray.First());
        knotCollection.AddRange(rhinoKnotsArray);
        knotCollection.Add(rhinoKnotsArray.Last());

        var spline = new CadSpline(nurbsCurve.Degree, nurbsCurve.IsRational,
            nurbsCurve.IsClosed, nurbsCurve.IsPeriodic, cadControlPoints,
            knotCollection, weightCollection, _fitTolerance, _fitTolerance);

        spline.UpdateFitData();

        return spline;
    }

    /// <summary>
    /// Converts a <see cref="RhinoEllipse"/> to a <see cref="Geometry.Ellipse"/>.
    /// </summary>  
    public CadEllipse ToAutoCadType(RhinoEllipse ellipse)
    {
        var plane = ellipse.Plane;

        var centrePoint = this.ToAutoCadType(ellipse.Center);

        var normal = this.ToAutoCadType(plane.Normal);

        var majorAxis = this.ToAutoCadType(plane.XAxis);

        var radiusRatio = _unitSystemManager.ToAutoCadLength(ellipse.Radius1) /
                          _unitSystemManager.ToAutoCadLength(ellipse.Radius2);

        var cadEllipse = new CadEllipse(centrePoint, normal, majorAxis, radiusRatio, 0,
            2 * Math.PI);

        return cadEllipse;
    }

    /// <summary>
    /// Converts a <see cref="RhinoArc"/> to a <see cref="CadArc"/>.
    /// </summary>
    public CadArc ToAutoCadType(RhinoArc arc)
    {
        var center = this.ToAutoCadType(arc.Center);

        var plane = this.ToAutoCadType(arc.Plane);

        var normal = plane.Normal;

        var radius = _unitSystemManager.ToAutoCadLength(arc.Radius);

        var startAngle = arc.StartAngle;

        var endAngle = arc.EndAngle;

        var cadArc = new CadArc(center, normal, radius, startAngle, endAngle);

        return cadArc;
    }

    /// <summary>
    /// Converts a <see cref="RhinoArcCurve"/> to a <see cref="CadArc"/>.
    /// </summary>
    public CadArc ToAutoCadType(RhinoArcCurve arcCurve)
    {
        var midPoint = arcCurve.PointAtNormalizedLength(_midPointParam);

        var startPoint = this.ToAutoCadType(arcCurve.PointAtStart);
        var endPoint = this.ToAutoCadType(arcCurve.PointAtEnd);
        var pointOnArc = this.ToAutoCadType(midPoint);

        var circularArc = new CadCircularArc3d(startPoint, pointOnArc, endPoint);

        var cadArc = new CadArc();
        cadArc.SetFromGeCurve(circularArc);

        return cadArc;
    }

    /// <summary>
    /// Converts a <see cref="RhinoCircle"/> to a <see cref="CadCircle"/>.
    /// </summary>
    public CadCircle ToAutoCadType(RhinoCircle circle)
    {
        var center = this.ToAutoCadType(circle.Center);

        var normal = this.ToAutoCadType(circle.Normal);

        var radius = _unitSystemManager.ToRhinoLength(circle.Radius);

        var cadCircle =
            new Autodesk.AutoCAD.DatabaseServices.Circle(center, normal, radius);

        return cadCircle;
    }

    /// <summary>
    /// Converts a <see cref="RhinoPolyCurve"/> to a <see cref="CadPolyline3d"/>.
    /// </summary>
    public CadPolyline3d ToAutoCadType(RhinoPolyLineCurve polyLineCurve)
    {
        var pointCount = polyLineCurve.PointCount;

        var pointCollection = new CadPoint3dCollection();
        for (var j = 0; j < pointCount; j++)
        {
            var rhinoPoint = polyLineCurve.Point(j);

            var cadPoint = this.ToAutoCadType(rhinoPoint);

            pointCollection.Add(cadPoint);
        }

        return new CadPolyline3d(Poly3dType.SimplePoly, pointCollection,
            polyLineCurve.IsClosed);

    }

    /// <summary>
    /// Converts a <see cref="RhinoPolyCurve"/> to a List of <see cref="CadCurve"/>.
    /// </summary>
    public IList<CadCurve> ToAutoCadType(RhinoPolyCurve polyCurve)
    {
        var segmentCount = polyCurve.SegmentCount;

        var curves = new List<CadCurve>();
        for (var i = 0; i < segmentCount; i++)
        {
            var curve = polyCurve.SegmentCurve(i);

            var cadCurve = this.ToAutoCadType(curve);

            curves.AddRange(cadCurve);
        }

        return curves;
    }

    /// <summary>
    /// Converts a <see cref="RhinoSubD"/> into an <see cref="CadSubDMesh"/> object.
    /// </summary>
    public CadSubDMesh ToAutoCadType(RhinoSubD mesh)
    {
        var pointsCollection = new CadPoint3dCollection();

        var vertexMap = new List<SubDVertex>();

        var faceArray = new Int32Collection();

        for (var i = 0; i < mesh.Vertices.Count; i++)
        {
            var vertex = mesh.Vertices.Find(i);

            var cadPoint = this.ToAutoCadType(vertex.ControlNetPoint);

            pointsCollection.Add(cadPoint);

            vertexMap.Add(vertex);
        }

        foreach (var face in mesh.Faces)
        {
            var numberOfVertices = face.VertexCount;

            faceArray.Add(numberOfVertices);

            for (var i = 0; i < numberOfVertices; i++)
            {
                var vertex = face.VertexAt(i);

                var index = vertexMap.IndexOf(vertex);

                faceArray.Add(index);
            }
        }

        var subDMesh = new CadSubDMesh();

        subDMesh.SetDatabaseDefaults();

        subDMesh.SetSubDMesh(pointsCollection, faceArray, 0);

        return subDMesh;
    }

    /// <summary>
    /// Converts a <see cref="RhinoMesh"/> into an <see cref="CadSubDMesh"/> object.
    /// </summary>
    public CadSubDMesh ToAutoCadSubD(RhinoMesh mesh)
    {
        var pointsCollection = new CadPoint3dCollection();

        var faceArray = new Int32Collection();

        foreach (var point in mesh.Vertices)
            pointsCollection.Add(this.ToAutoCadType(point));

        foreach (var face in mesh.Faces)
        {
            var numberOfVertices = face.IsQuad ? 4 : 3;

            faceArray.Add(numberOfVertices);

            faceArray.Add(face.A);

            faceArray.Add(face.B);

            faceArray.Add(face.C);

            if (face.IsQuad)
            {
                faceArray.Add(face.D);
            }
        }

        var subDMesh = new CadSubDMesh();

        subDMesh.SetDatabaseDefaults();

        subDMesh.SetSubDMesh(pointsCollection, faceArray, 0);

        return subDMesh;
    }

    /// <summary>
    /// Converts a <see cref="RhinoMesh"/> into an <see cref="CadPolyFaceMesh"/> object.
    /// </summary>
    public CadPolyFaceMesh ToAutoCadType(RhinoMesh mesh)
    {

        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var database = activeDocument.Database;

        var polyFaceMesh = new PolyFaceMesh();

        try
        {
            using var documentLock = activeDocument.LockDocument();

            using var transaction = database.TransactionManager.StartTransaction();

            var blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

            var blockTableRecord = transaction.GetObject(blockTable![BlockTableRecord.ModelSpace],
                OpenMode.ForWrite) as BlockTableRecord;

            blockTableRecord.AppendEntity(polyFaceMesh);

            transaction.AddNewlyCreatedDBObject(polyFaceMesh, true);

            foreach (var point in mesh.Vertices)
            {
                var vertex = new PolyFaceMeshVertex(this.ToAutoCadType(point));

                polyFaceMesh.AppendVertex(vertex);
            }

            foreach (var face in mesh.Faces)
            {
                var faceRecord = new FaceRecord((short)face.A, (short)face.B, (short)face.C, (short)face.D);

                polyFaceMesh.AppendFaceRecord(faceRecord);
            }
            transaction.Abort();

        }
        catch (System.Exception ex)
        {

        }
        return polyFaceMesh;
    }

    /// <summary>
    /// Converts a <see cref="RhinoNurbsSurface"/> into an <see cref="CadPolygonMesh"/> object.
    /// </summary>
    public CadPolygonMesh ToAutoCadPolygonMesh(RhinoNurbsSurface nurbsSurface)
    {
        if (nurbsSurface.OrderU != nurbsSurface.OrderV)
        {
            throw new NotSupportedException("Only surfaces with the same order in U and V are supported.");
        }

        var polygonMeshType = CadPolyMeshType.SimpleMesh;

        switch (nurbsSurface.OrderU - 1)
        {
            case 2:
                polygonMeshType = CadPolyMeshType.BezierSurfaceMesh;
                break;
            case 3:
                polygonMeshType = CadPolyMeshType.CubicSurfaceMesh;
                break;
            case 4:
                polygonMeshType = CadPolyMeshType.QuadSurfaceMesh;
                break;
            default:
                break;
        }

        var pointCollection = new CadPoint3dCollection();

        foreach (var point in nurbsSurface.Points)
        {
            pointCollection.Add(this.ToAutoCadType(point.Location));
        }

        var polygonMesh = new CadPolygonMesh(polygonMeshType, nurbsSurface.Points.CountU,
            nurbsSurface.Points.CountV, pointCollection, nurbsSurface.IsClosed(1),
            nurbsSurface.IsClosed(0));

        return polygonMesh;
    }

    /// <summary>
    /// Converts a <see cref="RhinoNurbsSurface"/> to a <see cref="CadNurbsSurface"/>.
    /// </summary>
    public CadNurbsSurface ToAutoCadType(RhinoNurbsSurface surface)
    {
        var degreeU = surface.OrderU - 1;
        var degreeV = surface.OrderV - 1;
        var isRational = surface.IsRational;
        var controlPointsU = surface.Points.CountU;
        var controlPointsV = surface.Points.CountV;

        var uKnots = new KnotCollection();
        uKnots.Add(surface.KnotsU.First());
        foreach (var uKnot in surface.KnotsU)
        {
            uKnots.Add(uKnot);
        }
        uKnots.Add(surface.KnotsU.Last());

        var vKnots = new KnotCollection();
        vKnots.Add(surface.KnotsV.First());
        foreach (var vKnot in surface.KnotsV)
        {
            vKnots.Add(vKnot);
        }
        vKnots.Add(surface.KnotsV.Last());

        var controlPoints = new CadPoint3dCollection();
        var weights = new DoubleCollection();
        for (var u = 0; u < controlPointsU; u++)
        {
            for (var v = 0; v < controlPointsV; v++)
            {
                var controlPoint = surface.Points.GetControlPoint(u, v);

                var convertedPoint = this.ToAutoCadType(controlPoint.Location);

                var weight = controlPoint.Weight;

                if (isRational)
                    weights.Add(weight);

                controlPoints.Add(convertedPoint);

            }
        }

        var cadSurface = new CadNurbsSurface();

        cadSurface.Set(degreeU, degreeV, isRational, controlPointsU, controlPointsV, controlPoints, weights, uKnots, vKnots);

        return cadSurface;
    }

    /// <summary>
    /// Converts a <see cref="RhinoSurface"/> to a <see cref="CadSurface"/>.
    /// </summary>
    public CadSurface ToAutoCadType(RhinoSurface surface)
    {
        var nurbs = surface.ToNurbsSurface();

        return this.ToAutoCadType(nurbs);

    }

    /// <summary>
    /// Converts a <see cref="RhinoBrep"/> to an array of AutoCAD <see cref="CadSolid3d"/>s.
    /// Typically, this is just a single solid representing the Brep, but depending on
    /// the import processing it could be multiple solids.
    /// </summary>
    /// <remarks>
    /// We need to use a AutoCAD document to import the Rhino brep temporarily. It is
    /// deleted after the import so which document we use is not important.
    /// </remarks>
    public CadSolid3d[] ToAutoCadType(RhinoBrep brep)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var database = activeDocument.Database;
        var editor = activeDocument.Editor;

        var addedObjects = new List<CadSolid3d>();

        try
        {
            var tempFolder = Path.GetTempPath();

            var pathLocation = $@"{tempFolder}AWI\Converters\";

            Directory.CreateDirectory(pathLocation);

            var rhinoFilePath = $@"{pathLocation}rhinoToAutoCad.3dm";

            var result = Rhino.FileIO.File3dm.WriteOneObject(rhinoFilePath, brep);

            if (File.Exists(rhinoFilePath) == false || result == false)
            {
                return addedObjects.ToArray();
            }

            editor.Command("._IMPORT", rhinoFilePath, "");

            var selectLast = editor.SelectLast();

            var selectedObjects = selectLast?.Value;

            using var transaction = database.TransactionManager.StartTransaction();

            using var documentLock = activeDocument.LockDocument();

            for (var index = 0; index < selectedObjects!.Count; index++)
            {
                var selection = selectedObjects![index];
                var importedObject =
                    transaction.GetObject(selection.ObjectId, OpenMode.ForWrite);

                var clone = importedObject.Clone();

                if (clone is not Solid3d solid3d) continue;

                addedObjects.Add(solid3d);

                importedObject.Erase(true);

            }

            transaction.Commit();
        }
        catch (System.Exception ex)
        {
            editor.WriteMessage($"\nError Converting brep: {ex.Message}");
        }

        return addedObjects.ToArray();
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
    /// Converts a Rhino mesh into an AutoCAD PolyFaceMesh object.
    /// </summary>
    public RhinoMesh ToRhinoType(CadPolyFaceMesh mesh)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var database = activeDocument.Database;

        var rhinoMesh = new RhinoMesh();

        try
        {
            using var transaction = database.TransactionManager.StartTransaction();

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

            transaction.Commit();

        }
        catch (System.Exception ex)
        {

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
    /// Converts a <see cref="RhinoPoint"/> to a <see cref="CadDBPoint"/>.
    /// </summary>
    public CadDBPoint ToAutoCadType(RhinoPoint point)
    {
        var point3d = this.ToAutoCadType(point.Location);

        return new CadDBPoint(point3d);
    }
}