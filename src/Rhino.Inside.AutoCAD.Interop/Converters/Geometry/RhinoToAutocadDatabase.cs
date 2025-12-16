using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadArc = Autodesk.AutoCAD.DatabaseServices.Arc;
using CadCircle = Autodesk.AutoCAD.DatabaseServices.Circle;
using CadCircularArc3d = Autodesk.AutoCAD.Geometry.CircularArc3d;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using CadDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using CadEllipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadNurbsSurface = Autodesk.AutoCAD.DatabaseServices.NurbSurface;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;
using CadPolyFaceMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;
using CadPolygonMesh = Autodesk.AutoCAD.DatabaseServices.PolygonMesh;
using CadPolyline3d = Autodesk.AutoCAD.DatabaseServices.Polyline3d;
using CadPolyMeshType = Autodesk.AutoCAD.DatabaseServices.PolyMeshType;
using CadSolid3d = Autodesk.AutoCAD.DatabaseServices.Solid3d;
using CadSpline = Autodesk.AutoCAD.DatabaseServices.Spline;
using CadSubDMesh = Autodesk.AutoCAD.DatabaseServices.SubDMesh;
using CadSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoEllipse = Rhino.Geometry.Ellipse;
using RhinoHatch = Rhino.Geometry.Hatch;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoNurbsSurface = Rhino.Geometry.NurbsSurface;
using RhinoPoint = Rhino.Geometry.Point;
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
    /// Converts the given <see cref="RhinoCurve"/> to a list of AutoCAD
    /// <see cref="Geometry.Curve"/>s. Typically, the list contains one <see cref="Geometry.Curve"/>
    /// if successful, however it can contain more if the <see cref="RhinoPolyCurve"/>
    /// is converted. It is the <see cref="RhinoPolyCurve"/> conversion which drives
    /// the need to return a list as AutoCAD does not have a direct equivalent.
    /// </summary>
    public IList<Autodesk.AutoCAD.Geometry.Curve2d> ToAutoCadType2d(RhinoCurve curve)
    {
        switch (curve)
        {
            case RhinoLineCurve line:
                return [this.ToAutoCadType2d(line)];

            case RhinoArcCurve arc:
                {
                    if (arc.IsCompleteCircle == false)
                        return [this.ToAutoCadType2d(arc)];

                    var circle = new RhinoCircle(arc.Arc);

                    return [this.ToAutoCadType2d(circle)];
                }

            case RhinoNurbsCurve nurbsCurve:
                return [this.ToAutoCadType2d(nurbsCurve)];

            case RhinoPolyLineCurve polyLineCurve:
                return [this.ToAutoCadType2d(polyLineCurve)];

            case RhinoPolyCurve polyCurve:
                return this.ToAutoCadType2d(polyCurve);

            default:
                return [];
        }
    }

    /// <summary>
    /// Converts the given <see cref="RhinoCurve"/> into a single curve. If the
    /// conversion would have result in multiple curves (for example a <see cref="RhinoPolyCurve"/>)
    /// then it is converted to a single <see cref="RhinoNurbsCurve"/> first. This will produce
    /// a single AutoCAD curve, but may result in loss of fidelity.  Where possible, use the list
    /// of curves returned by <see cref="ToAutoCadType(RhinoCurve)"/>
    /// </summary>
    public CadCurve ToAutoCadSingleCurve(RhinoCurve curve)
    {
        var single = curve is RhinoPolyCurve ? curve.ToNurbsCurve() : curve;

        var listCurves = this.ToAutoCadType(single);

        if (listCurves.Count == 1)
            return listCurves[0];

        throw new System.Exception("Cannot convert Rhino curve to single AutoCAD curve.");
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
    /// Converts a <see cref="RhinoLineCurve"/> to a <see cref="LineSegment2d"/>.
    /// </summary>
    public LineSegment2d ToAutoCadType2d(RhinoLineCurve line)
    {
        var startPoint = this.ToAutoCadType2d(line.PointAtStart);

        var endPoint = this.ToAutoCadType2d(line.PointAtEnd);

        return new LineSegment2d(startPoint, endPoint);
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
    /// Converts a <see cref="RhinoNurbsCurve"/> to a <see cref="CadSpline"/>.
    /// </summary>
    public NurbCurve2d ToAutoCadType2d(RhinoNurbsCurve nurbsCurve)
    {
        var plane = Rhino.Geometry.Plane.WorldXY;

        var flatCurve = RhinoCurve.ProjectToPlane(nurbsCurve, plane).ToNurbsCurve();

        var fitPoints = new Point2dCollection();
        foreach (var rhinoPoint in flatCurve.Points)
        {
            var cadPoint = this.ToAutoCadType2d(rhinoPoint.Location);
            fitPoints.Add(cadPoint);
        }

        return new NurbCurve2d(fitPoints);

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
    /// Converts a <see cref="RhinoArcCurve"/> to a <see cref="CircularArc2d"/>.
    /// </summary>
    public CircularArc2d ToAutoCadType2d(RhinoArcCurve arcCurve)
    {
        var midPoint = arcCurve.PointAtNormalizedLength(_midPointParam);

        var startPoint = this.ToAutoCadType2d(arcCurve.PointAtStart);
        var endPoint = this.ToAutoCadType2d(arcCurve.PointAtEnd);
        var pointOnArc = this.ToAutoCadType2d(midPoint);

        var circularArc = new CircularArc2d(startPoint, pointOnArc, endPoint);

        return circularArc;
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
    /// Converts a <see cref="RhinoCircle"/> to a <see cref="CadCircle"/>.
    /// </summary>
    public CircularArc2d ToAutoCadType2d(RhinoCircle circle)
    {
        var center = this.ToAutoCadType2d(circle.Center);

        var radius = _unitSystemManager.ToRhinoLength(circle.Radius);

        var cadCircle = new Autodesk.AutoCAD.Geometry.CircularArc2d(center, radius);

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
    /// Converts a <see cref="RhinoPolyCurve"/> to a <see cref="CadPolyline3d"/>.
    /// </summary>
    public PolylineCurve2d ToAutoCadType2d(RhinoPolyLineCurve polyLineCurve)
    {
        var pointCount = polyLineCurve.PointCount;

        var pointCollection = new Point2dCollection();
        for (var j = 0; j < pointCount; j++)
        {
            var rhinoPoint = polyLineCurve.Point(j);

            var cadPoint = this.ToAutoCadType2d(rhinoPoint);

            pointCollection.Add(cadPoint);
        }

        return new PolylineCurve2d(pointCollection);

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

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = this.ToAutoCadType(mesh, transactionManagerWrapper);

        transaction.Commit();

        return result;
    }

    /// <summary>
    /// Converts a <see cref="RhinoMesh"/> into an <see cref="CadPolyFaceMesh"/> object.
    /// Autocad cad Mesh faces are 1-based indexing, so we need to add 1 to each index.
    /// </summary>
    public CadPolyFaceMesh ToAutoCadType(RhinoMesh mesh, ITransactionManager transactionManager)
    {
        var polyFaceMesh = new PolyFaceMesh();
        try
        {
            var transaction = transactionManager.Unwrap();

            var blockTable = transaction.GetObject(transactionManager.BlockTableId.Unwrap(), OpenMode.ForRead) as BlockTable;

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
                if (face.IsQuad)
                {
                    var quadFaceRecord = new FaceRecord((short)(face.A + 1), (short)(face.B + 1), (short)(face.C + 1), (short)(face.D + 1));

                    polyFaceMesh.AppendFaceRecord(quadFaceRecord);

                    continue;
                }

                var faceRecord = new FaceRecord((short)(face.A + 1), (short)(face.B + 1), (short)(face.C + 1), 0);

                polyFaceMesh.AppendFaceRecord(faceRecord);
            }

            polyFaceMesh.Erase(true);
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
    /// Converts a Rhino <see cref="RhinoBrep"/> in a collection of <see cref="CadNurbsSurface"/>s.
    /// This is used as a subsitute for breps in autocad for display.
    /// </summary>
    public CadNurbsSurface[] ToAutoCadType(RhinoBrep brep)
    {
        var cadFaces = new List<CadNurbsSurface>();

        foreach (var face in brep.Faces)
        {
            var trimmedSurface = face.DuplicateFace(false);

            var singleFace = trimmedSurface.Faces[0];

            var nurbs = singleFace.ToNurbsSurface();

            var cadSurface = this.ToAutoCadType(nurbs);

            cadFaces.Add(cadSurface);
        }

        return cadFaces.ToArray();
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
    public CadSolid3d[] ToAutoCadType2(RhinoBrep brep)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = this.ToAutoCadType2(brep, transactionManagerWrapper);

        transaction.Commit();

        return result;
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
    public CadSolid3d[] ToAutoCadType2(RhinoBrep brep, ITransactionManager transactionManager)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var editor = activeDocument.Editor;

        var addedObjects = new List<CadSolid3d>();

        try
        {
            var tempFolder = Path.GetTempPath();

            var pathLocation = $@"{tempFolder}RhinoInsideAutocad\Converters\";

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

            var transaction = transactionManager.Unwrap();

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
        }
        catch (System.Exception ex)
        {
            editor.WriteMessage($"\nError Converting brep: {ex.Message}");
        }

        return addedObjects.ToArray();
    }

    /// <summary>
    /// Converts a Rhino <see cref="Extrusion"/> to an array of AutoCAD <see cref="CadSolid3d"/>s.
    /// Typically, this is just a single solid representing the Extrusion, but depending on
    /// conversion it could be multiple solids.
    /// </summary>
    public CadSolid3d[] ToAutoCadType(Extrusion extrusion)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = this.ToAutoCadType(extrusion, transactionManagerWrapper);

        transaction.Commit();

        return result;
    }

    /// <summary>
    /// Converts a <see cref="Extrusion"/> to an array of AutoCAD <see cref="CadSolid3d"/>s.
    /// </summary>
    public CadSolid3d[] ToAutoCadType(Extrusion extrusion, ITransactionManager transactionManager)
    {

        var solids = new List<CadSolid3d>();

        try
        {
            using var curves = new DBObjectCollection();

            var profileCount = extrusion.ProfileCount;

            for (var i = 0; i < profileCount; i++)
            {
                var profile = extrusion.Profile3d(i, 0);

                if (profile == null)
                    continue;

                var cadCurves = this.ToAutoCadType(profile);

                foreach (var cadCurve in cadCurves)
                {
                    curves.Add(cadCurve);
                }
            }

            var regions = Region.CreateFromCurves(curves);

            foreach (Region region in regions)
            {
                var solid = new Solid3d();

                var extrusionLine = extrusion.PathLineCurve();

                var extrusionVector = extrusionLine.PointAtEnd - extrusionLine.PointAtStart;

                var cadExtrusionVector = this.ToAutoCadType(extrusionVector);

                var magnitude = extrusionVector.Length;

                var cadMagnitude = _unitSystemManager.ToAutoCadLength(magnitude);

                var directionVector = cadExtrusionVector.MultiplyBy(cadMagnitude);

                var sweepOptions = new SweepOptions();

                solid.CreateExtrudedSolid(region, directionVector, sweepOptions);

                solids.Add(solid);

                region.Dispose();
            }
        }
        catch (Exception ex)
        {

        }

        return solids.ToArray();

    }

    /// <summary>
    /// Converts a <see cref="CadHatch"/> to a <see cref="RhinoHatch"/>.
    /// </summary>
    public CadHatch ToAutoCadType(RhinoHatch rhinoHatch, ITransactionManager transactionManager)
    {
        var scale = _unitSystemManager.ToAutoCadLength(rhinoHatch.PatternScale);

        var rotation = rhinoHatch.PatternRotation;

        //TODO: Support Hatch pattens/Styles

        var cadHatch = new CadHatch()
        {
            PatternAngle = rotation,
            PatternScale = scale,
            Origin = this.ToAutoCadType2d(rhinoHatch.BasePoint)

        };

        var outerCurves = rhinoHatch.Get3dCurves(true);

        var outerCurve = new PolyCurve();
        foreach (var curve in outerCurves)
        {
            outerCurve.Append(curve);
        }

        var curve2ds = new Curve2dCollection();
        var edgeTypes = new IntegerCollection();

        var curves = this.ToAutoCadType2d(outerCurve);

        foreach (var curve in curves)
        {

            curve2ds.Add(curve);
            edgeTypes.Add(1);
        }

        foreach (var innerCurve in rhinoHatch.Get3dCurves(false))
        {
            var innerLoop = this.ToAutoCadType2d(innerCurve);

            foreach (var curve in innerLoop)
            {
                curve2ds.Add(curve);
                edgeTypes.Add(1);
            }
        }

        cadHatch.AppendLoop(HatchLoopTypes.External, curve2ds, edgeTypes);

        cadHatch.EvaluateHatch(true);

        return cadHatch;
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
