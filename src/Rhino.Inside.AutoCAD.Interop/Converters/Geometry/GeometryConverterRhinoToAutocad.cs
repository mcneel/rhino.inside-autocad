using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Arc = Autodesk.AutoCAD.DatabaseServices.Arc;
using CadCircularArc3d = Autodesk.AutoCAD.Geometry.CircularArc3d;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;
using Curve = Autodesk.AutoCAD.DatabaseServices.Curve;
using Ellipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using Line = Autodesk.AutoCAD.DatabaseServices.Line;
using RhinoArc = Rhino.Geometry.Arc;
using RhinoArcCurve = Rhino.Geometry.ArcCurve;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoCircle = Rhino.Geometry.Circle;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoEllipse = Rhino.Geometry.Ellipse;
using RhinoLineCurve = Rhino.Geometry.LineCurve;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoNurbsCurve = Rhino.Geometry.NurbsCurve;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;
using RhinoPolyLineCurve = Rhino.Geometry.PolylineCurve;
using RhinoSubD = Rhino.Geometry.SubD;

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
    /// <see cref="Curve"/>s. Typically, the list contains one <see cref="Curve"/>
    /// if successful, however it can contain more if the <see cref="RhinoPolyCurve"/>
    /// is converted. It is the <see cref="RhinoPolyCurve"/> conversion which drives
    /// the need to return a list as AutoCAD does not have a direct equivalent.
    /// </summary>
    public IList<Curve> ToAutoCadType(RhinoCurve curve)
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
    /// Converts a <see cref="RhinoLineCurve"/> to a <see cref="Line"/>.
    /// </summary>
    public Line ToAutoCadType(RhinoLineCurve line)
    {
        var startPoint = this.ToAutoCadType(line.PointAtStart);

        var endPoint = this.ToAutoCadType(line.PointAtEnd);

        return new Line(startPoint, endPoint);
    }

    /// <summary>
    /// Converts a <see cref="RhinoNurbsCurve"/> to a <see cref="Spline"/>.
    /// </summary>
    public Spline ToAutoCadType(RhinoNurbsCurve nurbsCurve)
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

        var spline = new Spline(nurbsCurve.Degree, nurbsCurve.IsRational,
            nurbsCurve.IsClosed, nurbsCurve.IsPeriodic, cadControlPoints,
            knotCollection, weightCollection, _fitTolerance, _fitTolerance);

        spline.UpdateFitData();

        return spline;
    }

    /// <summary>
    /// Converts a <see cref="RhinoEllipse"/> to a <see cref="Ellipse"/>.
    /// </summary>  
    public Ellipse ToAutoCadType(RhinoEllipse ellipse)
    {
        var plane = ellipse.Plane;

        var centrePoint = this.ToAutoCadType(ellipse.Center);

        var normal = this.ToAutoCadType(plane.Normal);

        var majorAxis = this.ToAutoCadType(plane.XAxis);

        var radiusRatio = _unitSystemManager.ToAutoCadLength(ellipse.Radius1) /
                          _unitSystemManager.ToAutoCadLength(ellipse.Radius2);

        var cadEllipse = new Ellipse(centrePoint, normal, majorAxis, radiusRatio, 0,
            2 * Math.PI);

        return cadEllipse;
    }

    /// <summary>
    /// Converts a <see cref="RhinoArc"/> to a <see cref="Arc"/>.
    /// </summary>
    public Arc ToAutoCadType(RhinoArc arc)
    {
        var center = this.ToAutoCadType(arc.Center);

        var plane = this.ToAutoCadType(arc.Plane);

        var normal = plane.Normal;

        var radius = _unitSystemManager.ToAutoCadLength(arc.Radius);

        var startAngle = arc.StartAngle;

        var endAngle = arc.EndAngle;

        var cadArc = new Arc(center, normal, radius, startAngle, endAngle);

        return cadArc;
    }

    /// <summary>
    /// Converts a <see cref="RhinoArcCurve"/> to a <see cref="Arc"/>.
    /// </summary>
    public Arc ToAutoCadType(RhinoArcCurve arcCurve)
    {
        var midPoint = arcCurve.PointAtNormalizedLength(_midPointParam);

        var startPoint = this.ToAutoCadType(arcCurve.PointAtStart);
        var endPoint = this.ToAutoCadType(arcCurve.PointAtEnd);
        var pointOnArc = this.ToAutoCadType(midPoint);

        var circularArc = new CadCircularArc3d(startPoint, pointOnArc, endPoint);

        var cadArc = new Arc();
        cadArc.SetFromGeCurve(circularArc);

        return cadArc;
    }

    /// <summary>
    /// Converts a <see cref="RhinoCircle"/> to a <see cref="Autodesk.AutoCAD.DatabaseServices.Circle"/>.
    /// </summary>
    public Autodesk.AutoCAD.DatabaseServices.Circle ToAutoCadType(RhinoCircle circle)
    {
        var center = this.ToAutoCadType(circle.Center);

        var normal = this.ToAutoCadType(circle.Normal);

        var radius = _unitSystemManager.ToRhinoLength(circle.Radius);

        var cadCircle =
            new Autodesk.AutoCAD.DatabaseServices.Circle(center, normal, radius);

        return cadCircle;
    }

    /// <summary>
    /// Converts a <see cref="RhinoPolyCurve"/> to a <see cref="Autodesk.AutoCAD.DatabaseServices.Circle"/>.
    /// </summary>
    public Polyline3d ToAutoCadType(RhinoPolyLineCurve polyLineCurve)
    {
        var pointCount = polyLineCurve.PointCount;

        var pointCollection = new CadPoint3dCollection();
        for (var j = 0; j < pointCount; j++)
        {
            var rhinoPoint = polyLineCurve.Point(j);

            var cadPoint = this.ToAutoCadType(rhinoPoint);

            pointCollection.Add(cadPoint);
        }

        return new Polyline3d(Poly3dType.SimplePoly, pointCollection,
            polyLineCurve.IsClosed);

    }

    /// <summary>
    /// Converts a <see cref="RhinoPolyCurve"/> to a <see cref="Autodesk.AutoCAD.DatabaseServices.Circle"/>.
    /// </summary>
    public IList<Curve> ToAutoCadType(RhinoPolyCurve polyCurve)
    {
        var segmentCount = polyCurve.SegmentCount;

        var curves = new List<Curve>();
        for (var i = 0; i < segmentCount; i++)
        {
            var curve = polyCurve.SegmentCurve(i);

            var cadCurve = this.ToAutoCadType(curve);

            curves.AddRange(cadCurve);
        }

        return curves;
    }

    /// <summary>
    /// Converts a Rhino mesh into an AutoCAD SubDMesh object.
    /// </summary>
    public SubDMesh ToAutoCadType(RhinoSubD mesh)
    {
        var pointsCollection = new Autodesk.AutoCAD.Geometry.Point3dCollection();

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

        var subDMesh = new SubDMesh();

        subDMesh.SetDatabaseDefaults();

        subDMesh.SetSubDMesh(pointsCollection, faceArray, 0);

        return subDMesh;
    }

    /// <summary>
    /// Converts a Rhino mesh into an AutoCAD SubDMesh object.
    /// </summary>
    public SubDMesh ToAutoCadType(RhinoMesh mesh)
    {
        var pointsCollection = new Autodesk.AutoCAD.Geometry.Point3dCollection();

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

        var subDMesh = new SubDMesh();

        subDMesh.SetDatabaseDefaults();

        subDMesh.SetSubDMesh(pointsCollection, faceArray, 0);

        return subDMesh;
    }

    /// <summary>
    /// Converts a <see cref="RhinoBrep"/> to an array of AutoCAD <see cref="Solid3d"/>s.
    /// Typically, this is just a single solid representing the Brep, but depending on
    /// the import processing it could be multiple solids.
    /// </summary>
    /// <remarks>
    /// We need to use a AutoCAD document to import the Rhino brep temporarily. It is
    /// deleted after the import so which document we use is not important.
    /// </remarks>
    public Solid3d[] ToAutoCadType(RhinoBrep brep)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var database = activeDocument.Database;
        var editor = activeDocument.Editor;

        var addedObjects = new List<Solid3d>();

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
    /// Converts a <see cref="BoundingBox"/> to a <see cref="Extents3d"/>.
    /// </summary>
    public BoundingBox ToRhinoType(Extents3d extents)
    {
        var min = this.ToRhinoType(extents.MinPoint);

        var max = this.ToRhinoType(extents.MaxPoint);

        return new BoundingBox(min, max);
    }
}