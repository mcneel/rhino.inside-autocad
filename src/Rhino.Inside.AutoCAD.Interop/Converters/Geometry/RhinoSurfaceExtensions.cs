using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;
using CadNurbsSurface = Autodesk.AutoCAD.DatabaseServices.NurbSurface;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;
using CadPolyFaceMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;
using CadPolygonMesh = Autodesk.AutoCAD.DatabaseServices.PolygonMesh;
using CadPolyMeshType = Autodesk.AutoCAD.DatabaseServices.PolyMeshType;
using CadSolid3d = Autodesk.AutoCAD.DatabaseServices.Solid3d;
using CadSubDMesh = Autodesk.AutoCAD.DatabaseServices.SubDMesh;
using CadSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoHatch = Rhino.Geometry.Hatch;
using RhinoMesh = Rhino.Geometry.Mesh;
using RhinoNurbsSurface = Rhino.Geometry.NurbsSurface;
using RhinoSubD = Rhino.Geometry.SubD;
using RhinoSurface = Rhino.Geometry.Surface;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for converting Rhino surface/mesh types to AutoCAD surface/mesh types.
/// </summary>
public static class RhinoSurfaceExtensions
{
    /// <summary>
    /// Converts a Rhino SubD to an AutoCAD SubDMesh, applying unit conversion.
    /// </summary>
    /// <param name="mesh">The Rhino SubD to convert.</param>
    /// <returns>An AutoCAD SubDMesh with vertices scaled to AutoCAD units.</returns>
    public static CadSubDMesh ToAutocadSubDMesh(this RhinoSubD mesh)
    {
        var pointsCollection = new CadPoint3dCollection();
        var vertexMap = new List<SubDVertex>();
        var faceArray = new Int32Collection();

        for (var i = 0; i < mesh.Vertices.Count; i++)
        {
            var vertex = mesh.Vertices.Find(i);
            var cadPoint = vertex.ControlNetPoint.ToAutocadPoint3d();

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
    /// Converts a Rhino Mesh to an AutoCAD SubDMesh, applying unit conversion.
    /// </summary>
    /// <param name="mesh">The Rhino Mesh to convert.</param>
    /// <returns>An AutoCAD SubDMesh with vertices scaled to AutoCAD units.</returns>
    public static CadSubDMesh ToAutocadSubDMesh(this RhinoMesh mesh)
    {
        var pointsCollection = new CadPoint3dCollection();
        var faceArray = new Int32Collection();

        foreach (var point in mesh.Vertices)
            pointsCollection.Add(point.ToAutocadPoint3d());

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
    /// Converts a Rhino NurbsSurface to an AutoCAD PolygonMesh, applying unit conversion.
    /// Only supports surfaces with the same order in U and V.
    /// </summary>
    /// <param name="nurbsSurface">The Rhino NurbsSurface to convert.</param>
    /// <returns>An AutoCAD PolygonMesh with control points scaled to AutoCAD units.</returns>
    /// <exception cref="NotSupportedException">Thrown when the surface has different orders in U and V.</exception>
    public static CadPolygonMesh ToAutocadPolygonMesh(this RhinoNurbsSurface nurbsSurface)
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
            pointCollection.Add(point.Location.ToAutocadPoint3d());
        }

        var polygonMesh = new CadPolygonMesh(polygonMeshType, nurbsSurface.Points.CountU,
            nurbsSurface.Points.CountV, pointCollection, nurbsSurface.IsClosed(1),
            nurbsSurface.IsClosed(0));

        return polygonMesh;
    }

    /// <summary>
    /// Converts a Rhino NurbsSurface to an AutoCAD NurbSurface, applying unit conversion.
    /// </summary>
    /// <param name="surface">The Rhino NurbsSurface to convert.</param>
    /// <returns>An AutoCAD NurbSurface with control points scaled to AutoCAD units.</returns>
    public static CadNurbsSurface ToAutocadNurbSurface(this RhinoNurbsSurface surface)
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
                var convertedPoint = controlPoint.Location.ToAutocadPoint3d();
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
    /// Converts a Rhino Surface to an AutoCAD NurbSurface, applying unit conversion.
    /// </summary>
    /// <param name="surface">The Rhino Surface to convert.</param>
    /// <returns>An AutoCAD NurbSurface with control points scaled to AutoCAD units.</returns>
    public static CadNurbsSurface ToAutocadNurbSurface(this RhinoSurface surface)
    {
        var nurbs = surface.ToNurbsSurface();
        return nurbs.ToAutocadNurbSurface();
    }

    /// <summary>
    /// Converts a Rhino Brep to an array of AutoCAD NurbSurfaces, applying unit conversion.
    /// Each face of the Brep is converted to a separate NurbSurface.
    /// </summary>
    /// <param name="brep">The Rhino Brep to convert.</param>
    /// <returns>An array of AutoCAD NurbSurfaces representing the Brep faces.</returns>
    public static CadNurbsSurface[] ToAutocadNurbSurfaces(this Brep brep)
    {
        var cadFaces = new List<CadNurbsSurface>();

        foreach (var face in brep.Faces)
        {
            var trimmedSurface = face.DuplicateFace(false);
            var singleFace = trimmedSurface.Faces[0];
            var nurbs = singleFace.ToNurbsSurface();
            var cadSurface = nurbs.ToAutocadNurbSurface();

            cadFaces.Add(cadSurface);
        }

        return cadFaces.ToArray();
    }

    /// <summary>
    /// Converts a Rhino Hatch to an AutoCAD Hatch, applying unit conversion.
    /// </summary>
    /// <param name="rhinoHatch">The Rhino Hatch to convert.</param>
    /// <param name="transactionManager">The transaction manager for database operations.</param>
    /// <returns>An AutoCAD Hatch.</returns>
    public static CadHatch ToAutocadHatch(this RhinoHatch rhinoHatch, ITransactionManager transactionManager)
    {
        var scale = UnitConverter.ToAutoCadLength(rhinoHatch.PatternScale);

        var origin = rhinoHatch.BasePoint.ToAutocadPoint2d();

        var cadHatch = new CadHatch()
        {
            PatternScale = scale,
            Origin = origin,
        };

        cadHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");

        var outerCurves = rhinoHatch.Get3dCurves(true);

        var outerCurve = new PolyCurve();
        foreach (var curve in outerCurves)
        {
            outerCurve.Append(curve);
        }

        var transaction = transactionManager.Unwrap();

        var modelSpace = transactionManager.GetModelSpace(true).UnwrapObject() as BlockTableRecord;

        var objectIds = new ObjectIdCollection();

        var outerLoop = outerCurve.ToAutocadCurves();

        foreach (var curve in outerLoop)
        {
            modelSpace!.AppendEntity(curve);

            transaction.AddNewlyCreatedDBObject(curve, true);

            objectIds.Add(curve.ObjectId);
        }

        foreach (var innerCurve in rhinoHatch.Get3dCurves(false))
        {
            var innerLoop = innerCurve.ToAutocadCurves();

            foreach (var curve in innerLoop)
            {
                modelSpace!.AppendEntity(curve);

                transaction.AddNewlyCreatedDBObject(curve, true);

                objectIds.Add(curve.ObjectId);
            }
        }

        cadHatch.AppendLoop(HatchLoopTypes.External, objectIds);

        cadHatch.EvaluateHatch(true);

        foreach (ObjectId objectId in objectIds)
        {
            var dbObject = transaction.GetObject(objectId, OpenMode.ForWrite);
            dbObject.Erase(true);
        }

        return cadHatch;
    }

    /// <summary>
    /// Converts a Rhino Extrusion to an array of AutoCAD Solid3ds, applying unit conversion.
    /// </summary>
    /// <param name="extrusion">The Rhino Extrusion to convert.</param>
    /// <param name="transactionManager">The transaction manager for database operations.</param>
    /// <returns>An array of AutoCAD Solid3d objects.</returns>
    public static CadSolid3d[] ToAutocadSolid3ds(this Extrusion extrusion, ITransactionManager transactionManager)
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

                var cadCurves = profile.ToAutocadCurves();

                foreach (var cadCurve in cadCurves)
                {
                    curves.Add(cadCurve);
                }
            }

            var regions = Region.CreateFromCurves(curves);

            foreach (Region region in regions)
            {
                var solid = new CadSolid3d();

                var extrusionLine = extrusion.PathLineCurve();

                var extrusionVector = extrusionLine.PointAtEnd - extrusionLine.PointAtStart;

                var cadExtrusionVector = extrusionVector.ToAutocadVector3d();

                var magnitude = extrusionVector.Length;

                var cadMagnitude = UnitConverter.ToAutoCadLength(magnitude);

                var directionVector = cadExtrusionVector.MultiplyBy(cadMagnitude);

                var sweepOptions = new SweepOptions();

                solid.CreateExtrudedSolid(region, directionVector, sweepOptions);

                solids.Add(solid);

                region.Dispose();
            }
        }
        catch
        {
            // Swallow exceptions during conversion
        }

        return solids.ToArray();
    }

    /// <summary>
    /// Converts a Rhino Mesh to an AutoCAD PolyFaceMesh, applying unit conversion.
    /// Uses the current active document for transaction management.
    /// </summary>
    /// <param name="mesh">The Rhino Mesh to convert.</param>
    /// <returns>An AutoCAD PolyFaceMesh with vertices scaled to AutoCAD units.</returns>
    public static CadPolyFaceMesh? ToAutocadPolyFaceMesh(this RhinoMesh mesh)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = mesh.ToAutocadPolyFaceMesh(transactionManagerWrapper);

        transaction.Commit();

        return result;
    }

    /// <summary>
    /// Converts a Rhino Mesh to an AutoCAD PolyFaceMesh, applying unit conversion.
    /// AutoCAD mesh faces use 1-based indexing, so indices are adjusted accordingly.
    /// </summary>
    /// <param name="mesh">The Rhino Mesh to convert.</param>
    /// <param name="transactionManager">The transaction manager for database operations.</param>
    /// <returns>An AutoCAD PolyFaceMesh with vertices scaled to AutoCAD units.</returns>
    public static CadPolyFaceMesh? ToAutocadPolyFaceMesh(this RhinoMesh mesh, ITransactionManager transactionManager)
    {
        var polyFaceMesh = new CadPolyFaceMesh();
        var clone = new CadPolyFaceMesh();

        try
        {
            var transaction = transactionManager.Unwrap();

            var blockTable = transaction.GetObject(transactionManager.BlockTableId.Unwrap(), OpenMode.ForRead) as BlockTable;

            var blockTableRecord = transaction.GetObject(blockTable![BlockTableRecord.ModelSpace],
                OpenMode.ForWrite) as BlockTableRecord;

            blockTableRecord!.AppendEntity(polyFaceMesh);

            transaction.AddNewlyCreatedDBObject(polyFaceMesh, true);

            foreach (var point in mesh.Vertices)
            {
                var vertex = new PolyFaceMeshVertex(point.ToAutocadPoint3d());

                polyFaceMesh.AppendVertex(vertex);
                transaction.AddNewlyCreatedDBObject(vertex, true);
            }

            foreach (var face in mesh.Faces)
            {
                if (face.IsQuad)
                {
                    var quadFaceRecord = new FaceRecord((short)(face.A + 1), (short)(face.B + 1), (short)(face.C + 1), (short)(face.D + 1));

                    polyFaceMesh.AppendFaceRecord(quadFaceRecord);

                    transaction.AddNewlyCreatedDBObject(quadFaceRecord, true);

                    continue;
                }

                var faceRecord = new FaceRecord((short)(face.A + 1), (short)(face.B + 1), (short)(face.C + 1), 0);

                polyFaceMesh.AppendFaceRecord(faceRecord);

                transaction.AddNewlyCreatedDBObject(faceRecord, true);
            }

            clone = polyFaceMesh.Clone() as CadPolyFaceMesh;

            polyFaceMesh.Erase(true);
        }
        catch (System.Exception ex)
        {
            LoggerService.Instance?.LogError(ex, "AutoCAD PolyFaceMesh ToAutocadPolyFaceMesh(RhinoMesh mesh)");
        }

        return clone;
    }

    /// <summary>
    /// Converts a Rhino Brep to an AutoCAD BrepProxy representation.
    /// The proxy stores the Brep faces as AutoCAD NurbSurfaces.
    /// </summary>
    /// <param name="rhinoBrep">The Rhino Brep to convert.</param>
    /// <returns>An AutocadBrepProxy containing NurbSurface representations of the Brep faces.</returns>
    public static AutocadBrepProxy? ToAutocadBrepProxy(this RhinoBrep rhinoBrep)
    {
        var faces = new List<CadSurface>();

        foreach (var brepFace in rhinoBrep.Faces)
        {
            var trimmedFace = brepFace.DuplicateFace(false);

            var nurbsSurface = trimmedFace.Faces[0].ToNurbsSurface();

            var cadNurbsSurface = nurbsSurface.ToAutocadNurbSurface();

            faces.Add(cadNurbsSurface);
        }

        return new AutocadBrepProxy(faces);
    }
}
