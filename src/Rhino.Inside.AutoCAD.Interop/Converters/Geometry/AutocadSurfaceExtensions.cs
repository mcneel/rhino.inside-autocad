using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadExtents3d = Autodesk.AutoCAD.DatabaseServices.Extents3d;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;
using CadNurbsSurface = Autodesk.AutoCAD.DatabaseServices.NurbSurface;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using CadObjectIdCollection = Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;
using CadPolyFaceMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;
using CadPolygonMesh = Autodesk.AutoCAD.DatabaseServices.PolygonMesh;
using CadPolygonMeshVertex = Autodesk.AutoCAD.DatabaseServices.PolygonMeshVertex;
using CadPolyMeshType = Autodesk.AutoCAD.DatabaseServices.PolyMeshType;
using CadSolid3d = Autodesk.AutoCAD.DatabaseServices.Solid3d;
using CadSubDMesh = Autodesk.AutoCAD.DatabaseServices.SubDMesh;
using CadVertex3dType = Autodesk.AutoCAD.DatabaseServices.Vertex3dType;
using RhinoBoundingBox = Rhino.Geometry.BoundingBox;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoControlPoint = Rhino.Geometry.ControlPoint;
using RhinoHatch = Rhino.Geometry.Hatch;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoNurbsSurface = Rhino.Geometry.NurbsSurface;
using RhinoPoint3d = Rhino.Geometry.Point3d;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting AutoCAD surface/mesh types to Rhino surface/mesh types.
/// </summary>
public static class AutocadSurfaceExtensions
{
    /// <summary>
    /// Converts an AutoCAD Extents3d to a Rhino BoundingBox, applying unit conversion.
    /// </summary>
    /// <param name="extents">The AutoCAD extents to convert.</param>
    /// <returns>A Rhino BoundingBox with coordinates scaled to Rhino units.</returns>
    public static RhinoBoundingBox ToRhinoBoundingBox(this CadExtents3d extents)
    {
        var min = extents.MinPoint.ToRhinoPoint3d();
        var max = extents.MaxPoint.ToRhinoPoint3d();
        return new RhinoBoundingBox(min, max);
    }

    /// <summary>
    /// Converts an AutoCAD SubDMesh to a Rhino Mesh, applying unit conversion.
    /// </summary>
    /// <param name="mesh">The AutoCAD SubDMesh to convert.</param>
    /// <returns>A Rhino Mesh with vertices scaled to Rhino units.</returns>
    public static RhinoMesh ToRhinoMesh(this CadSubDMesh mesh)
    {
        var rhinoMesh = new RhinoMesh();

        foreach (CadPoint3d point in mesh.Vertices)
            rhinoMesh.Vertices.Add(point.ToRhinoPoint3d());

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
    /// Converts an AutoCAD PolygonMesh to a Rhino NurbsSurface, applying unit conversion.
    /// </summary>
    /// <param name="mesh">The AutoCAD PolygonMesh to convert.</param>
    /// <returns>A Rhino NurbsSurface with control points scaled to Rhino units.</returns>
    public static RhinoNurbsSurface ToRhinoNurbsSurface(this CadPolygonMesh mesh)
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
            if (meshItem is not CadPolygonMeshVertex vertex || vertex.VertexType != CadVertex3dType.ControlVertex)
                continue;

            var convertedPoint = vertex.Position.ToRhinoPoint3d();
            points.Add(convertedPoint);
        }

        var rhinoSurface = RhinoNurbsSurface.CreateFromPoints(points, controlPointsU,
            controlPointsV, degree, degree);

        return rhinoSurface;
    }

    /// <summary>
    /// Converts an AutoCAD NurbSurface to a Rhino NurbsSurface, applying unit conversion.
    /// </summary>
    /// <param name="cadNurbsSurface">The AutoCAD NurbSurface to convert.</param>
    /// <returns>A Rhino NurbsSurface with control points scaled to Rhino units.</returns>
    public static RhinoNurbsSurface ToRhinoNurbsSurface(this CadNurbsSurface cadNurbsSurface)
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
                var convertedPoint = controlPoint.ToRhinoPoint3d();
                var weight = cadNurbsSurface.GetWeight(u, v);

                var rhinoControlPoint = new RhinoControlPoint(convertedPoint, weight);
                rhinoSurface.Points.SetControlPoint(u, v, rhinoControlPoint);
            }
        }

        return rhinoSurface;
    }

    /// <summary>
    /// Converts an AutoCAD PolyFaceMesh to a Rhino Mesh, applying unit conversion.
    /// Uses the current active document for transaction management.
    /// </summary>
    /// <param name="mesh">The AutoCAD PolyFaceMesh to convert.</param>
    /// <returns>A Rhino Mesh with vertices scaled to Rhino units.</returns>
    public static RhinoMesh ToRhinoMesh(this CadPolyFaceMesh mesh)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = mesh.ToRhinoMesh(transactionManagerWrapper);

        transaction.Commit();

        return result;
    }

    /// <summary>
    /// Converts an AutoCAD PolyFaceMesh to a Rhino Mesh, applying unit conversion.
    /// </summary>
    /// <param name="mesh">The AutoCAD PolyFaceMesh to convert.</param>
    /// <param name="transactionManager">The transaction manager for database operations.</param>
    /// <returns>A Rhino Mesh with vertices scaled to Rhino units.</returns>
    public static RhinoMesh ToRhinoMesh(this CadPolyFaceMesh mesh, ITransactionManager transactionManager)
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
                            var rhinoVertex = polyFaceMeshVertex.Position.ToRhinoPoint3d();

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
        catch (System.Exception)
        {
            // Swallow exceptions during conversion
        }

        return rhinoMesh;
    }

    /// <summary>
    /// Converts an AutoCAD Hatch to a Rhino Hatch, applying unit conversion.
    /// </summary>
    /// <param name="cadHatch">The AutoCAD Hatch to convert.</param>
    /// <returns>A Rhino Hatch with geometry scaled to Rhino units.</returns>
    public static RhinoHatch ToRhinoHatch(this CadHatch cadHatch)
    {
        var cadPlane = cadHatch.GetPlane();

        var scale = UnitConverter.ToRhinoLength(cadHatch.PatternScale);

        var rotation = cadHatch.PatternAngle;

        // TODO: Support Hatch patterns
        var patternIndex = 1;

        var hatchPlane = cadPlane.ToRhinoPlane();

        var rhinoLoops = new List<RhinoPolyCurve>();
        var externalType = HatchLoopTypes.External;
        var outermostType = HatchLoopTypes.Outermost;

        for (var i = 0; i < cadHatch.NumberOfLoops; i++)
        {
            var hatchLoop = cadHatch.GetLoopAt(i);

            var loopType = hatchLoop.LoopType;

            if ((loopType & externalType) != externalType &&
                (loopType & outermostType) != outermostType) continue;

            var loop = hatchLoop.ToRhinoPolyCurve();

            rhinoLoops.Add(loop);
        }

        return RhinoHatch.Create(hatchPlane, rhinoLoops.FirstOrDefault(), rhinoLoops.Skip(1),
              patternIndex, rotation, scale);
    }

    /// <summary>
    /// Converts an AutoCAD HatchLoop to a Rhino PolyCurve, applying unit conversion.
    /// </summary>
    /// <param name="cadHatchLoop">The AutoCAD HatchLoop to convert.</param>
    /// <returns>A Rhino PolyCurve representing the hatch loop boundary.</returns>
    public static RhinoPolyCurve ToRhinoPolyCurve(this HatchLoop cadHatchLoop)
    {
        var loopCurves = cadHatchLoop.Curves;

        var isPolyLine = cadHatchLoop.IsPolyline;

        return isPolyLine
            ? cadHatchLoop.Polyline.ToRhinoPolyCurve()
            : loopCurves.ToRhinoPolyCurve();
    }

    /// <summary>
    /// Converts a Solid3d to an array of Rhino Breps by exporting to DXF and importing.
    /// </summary>
    /// <param name="solid">The AutoCAD Solid3d to convert.</param>
    /// <returns>An array of Rhino Breps representing the solid.</returns>
    public static RhinoBrep[] ToRhinoBreps(this CadSolid3d solid)
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

                var clonedSolid = solid.Clone() as Entity;

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
        catch (System.Exception)
        {
            // Swallow exceptions during conversion
        }

        return addedObjects.ToArray();
    }
}
