using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using CadIntegerCollection = Autodesk.AutoCAD.Geometry.IntegerCollection;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewDrawableBuilder"/>
/// <remarks>
/// Curves, points, meshes, Breps, extrusions, surfaces and SubDs are turned straight into
/// primitive buffers and create no AutoCAD entities. Everything else (text, dimensions,
/// leaders, hatches and anything unrecognised) goes through its existing
/// <see cref="IRhinoConvertible"/> conversion and is drawn as fallback entities.
/// </remarks>
public class PreviewDrawableBuilder : IPreviewDrawableBuilder
{
    private const double _chordToleranceRatio = GeometryConstants.PreviewChordToleranceRatio;
    private const double _angleTolerance = GeometryConstants.PreviewAngleTolerance;
    private const double _minimumSegmentLength = GeometryConstants.PreviewMinimumSegmentLength;
    private const double _maximumSegmentLength = GeometryConstants.PreviewMaximumSegmentLength;
    private const double _minimumChordTolerance = GeometryConstants.ZeroTolerance;
    private const int _subDDisplayDensity = GeometryConstants.PreviewSubDDisplayDensity;
    private const double _meshDensity = GeometryConstants.PreviewMeshDensity;

    private readonly IAutoCadInstance _autoCadInstance;
    private readonly IEntityValidator _entityValidator;

    /// <summary>
    /// Constructs a new <see cref="PreviewDrawableBuilder"/>.
    /// </summary>
    public PreviewDrawableBuilder(IAutoCadInstance autoCadInstance)
    {
        _autoCadInstance = autoCadInstance;
        _entityValidator = new EntityValidator();
    }

    /// <summary>
    /// Tries to get the active AutoCAD document.
    /// </summary>
    private bool TryGetActiveDocument(out IAutocadDocument? activeDocument)
    {
        activeDocument = _autoCadInstance.ActiveDocument;

        return activeDocument != null;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Primitive geometry is built first and the fallback geometry after it, so only the
    /// fallback conversion opens a transaction, and only when there is fallback geometry.
    /// Each item which fails to build is logged and skipped. The item limit is checked
    /// before every item, so a single item may overshoot it by its edge count, but fallback
    /// entities beyond the limit are disposed.
    /// </remarks>
    public IPreviewDrawable Build(IRhinoConvertibleSet rhinoConvertibleSet, IGeometryPreviewSettings unselectedSettings,
        IGeometryPreviewSettings selectedSettings, bool isSelected, int maxItems)
    {
        var buffers = new PreviewGeometryBuffer();

        try
        {
            if (this.TryGetActiveDocument(out var activeDocument))
            {
                var fallbackConvertibles = this.AddPrimitives(rhinoConvertibleSet, buffers, maxItems);

                if (fallbackConvertibles.Count > 0)
                {
                    var settings = isSelected ? selectedSettings : unselectedSettings;

                    this.AddFallbackEntities(activeDocument!, fallbackConvertibles, settings, buffers, maxItems);
                }
            }

            var drawable = new PreviewDrawable(buffers, unselectedSettings, selectedSettings, isSelected);

            return drawable;
        }
        catch
        {
            buffers.Dispose();

            throw;
        }
    }

    /// <summary>
    /// Adds every convertible which can be drawn as primitives to the buffers, and returns
    /// the ones which need the fallback conversion.
    /// </summary>
    private List<IRhinoConvertible> AddPrimitives(IRhinoConvertibleSet rhinoConvertibleSet,
        IPreviewGeometryBuffer buffers, int maxItems)
    {
        var fallbackConvertibles = new List<IRhinoConvertible>();

        using var meshingParameters = new MeshingParameters(_meshDensity);

        foreach (var convertible in rhinoConvertibleSet)
        {
            if (buffers.ItemCount >= maxItems) break;

            try
            {
                if (this.TryAddPrimitive(convertible, buffers, meshingParameters, maxItems) == false)
                {
                    fallbackConvertibles.Add(convertible);
                }
            }
            catch (Exception exception)
            {
                LoggerService.Instance.LogError(exception,
                    $"Unable to build the preview of a {convertible.GetType().Name}; it is not previewed.");
            }
        }

        return fallbackConvertibles;
    }

    /// <summary>
    /// Adds the convertible's geometry to the buffers when it is a type drawn as primitives.
    /// Returns false when it needs the fallback conversion instead.
    /// </summary>
    private bool TryAddPrimitive(IRhinoConvertible convertible, IPreviewGeometryBuffer buffers,
        MeshingParameters meshingParameters, int maxItems)
    {
        switch (convertible)
        {
            case IRhinoConvertibleTyped<Curve> curveConvertible:
                this.AddCurve(curveConvertible.RhinoGeometry, buffers);
                return true;

            case IRhinoConvertibleTyped<Rhino.Geometry.Point> pointConvertible:
                var location = pointConvertible.RhinoGeometry.Location;

                buffers.AddPoint(location);
                return true;

            case IRhinoConvertibleTyped<Mesh> meshConvertible:
                var meshes = new[] { meshConvertible.RhinoGeometry };

                this.AddShell(meshes, buffers);
                return true;

            case IRhinoConvertibleTyped<Brep> brepConvertible:
                this.AddBrep(brepConvertible.RhinoGeometry, buffers, meshingParameters, maxItems);
                return true;

            case IRhinoConvertibleTyped<Extrusion> extrusionConvertible:
                this.AddExtrusion(extrusionConvertible.RhinoGeometry, buffers, meshingParameters, maxItems);
                return true;

            case IRhinoConvertibleTyped<Surface> surfaceConvertible:
                this.AddSurface(surfaceConvertible.RhinoGeometry, buffers, meshingParameters, maxItems);
                return true;

            case IRhinoConvertibleTyped<SubD> subDConvertible:
                this.AddSubD(subDConvertible.RhinoGeometry, buffers, maxItems);
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Adds a Rhino polyline to the buffers as one AutoCAD polyline.
    /// </summary>
    private void AddPolyline(Polyline polyline, IPreviewGeometryBuffer buffers)
    {
        if (polyline.Count < 2) return;

        var cadPoints = new CadPoint3dCollection();

        foreach (var point in polyline)
        {
            var cadPoint = point.ToAutocadPoint3d();

            cadPoints.Add(cadPoint);
        }

        var points = new AutocadPoint3dCollectionWrapper(cadPoints);

        buffers.Polylines.Add(points);
    }

    /// <summary>
    /// Adds a curve to the buffers as one polyline, sampling it when it is not already
    /// polyline-shaped.
    /// </summary>
    /// <remarks>
    /// The chord tolerance is relative to the curve's size, floored at a minimum, so it
    /// samples consistently whatever the model units.
    /// </remarks>
    private void AddCurve(Curve curve, IPreviewGeometryBuffer buffers)
    {
        if (curve.TryGetPolyline(out var polyline))
        {
            this.AddPolyline(polyline, buffers);

            return;
        }

        var boundingBox = curve.GetBoundingBox(false);

        var chordTolerance = Math.Max(boundingBox.Diagonal.Length * _chordToleranceRatio, _minimumChordTolerance);

        using var polylineCurve = curve.ToPolyline(chordTolerance, _angleTolerance,
            _minimumSegmentLength, _maximumSegmentLength);

        if (polylineCurve == null) return;

        if (polylineCurve.TryGetPolyline(out var sampledPolyline))
        {
            this.AddPolyline(sampledPolyline, buffers);
        }
    }

    /// <summary>
    /// Adds curves to the buffers one polyline each, until <paramref name="maxItems"/> is
    /// reached.
    /// </summary>
    private void AddCurves(IEnumerable<Curve> curves, IPreviewGeometryBuffer buffers, int maxItems)
    {
        foreach (var curve in curves)
        {
            if (buffers.ItemCount >= maxItems) break;

            this.AddCurve(curve, buffers);
        }
    }

    /// <summary>
    /// Adds the given meshes to the buffers as a single shell, in AutoCAD units.
    /// </summary>
    /// <remarks>
    /// Rhino faces are triangles or quads, matching AutoCAD's shell face list format of a
    /// vertex count followed by the vertex indices. Merging the meshes keeps a meshed Brep
    /// to one shell however many faces it has.
    /// </remarks>
    private void AddShell(IEnumerable<Mesh> meshes, IPreviewGeometryBuffer buffers)
    {
        var vertices = new CadPoint3dCollection();
        var faces = new CadIntegerCollection();

        foreach (var mesh in meshes)
        {
            var indexOffset = vertices.Count;

            foreach (var vertex in mesh.Vertices)
            {
                var cadVertex = vertex.ToAutocadPoint3d();

                vertices.Add(cadVertex);
            }

            foreach (var face in mesh.Faces)
            {
                faces.Add(face.IsQuad ? 4 : 3);
                faces.Add(face.A + indexOffset);
                faces.Add(face.B + indexOffset);
                faces.Add(face.C + indexOffset);

                if (face.IsQuad)
                {
                    faces.Add(face.D + indexOffset);
                }
            }
        }

        if (vertices.Count == 0 || faces.Count == 0) return;

        var vertexCollection = new AutocadPoint3dCollectionWrapper(vertices);
        var faceCollection = new AutocadIntegerCollectionWrapper(faces);

        var shell = new PreviewShell(vertexCollection, faceCollection);

        buffers.Shells.Add(shell);
    }

    /// <summary>
    /// Adds a Brep's faces to the buffers as a shell, using the faces' cached render meshes
    /// when every face has one and meshing the Brep otherwise.
    /// </summary>
    private void AddBrepShell(Brep brep, IPreviewGeometryBuffer buffers, MeshingParameters meshingParameters)
    {
        var cachedMeshes = new List<Mesh>();

        foreach (var face in brep.Faces)
        {
            var cachedMesh = face.GetMesh(MeshType.Render);

            if (cachedMesh == null)
            {
                cachedMeshes.Clear();

                break;
            }

            cachedMeshes.Add(cachedMesh);
        }

        if (cachedMeshes.Count > 0)
        {
            this.AddShell(cachedMeshes, buffers);

            return;
        }

        var createdMeshes = Mesh.CreateFromBrep(brep, meshingParameters);

        if (createdMeshes == null) return;

        try
        {
            this.AddShell(createdMeshes, buffers);
        }
        finally
        {
            foreach (var createdMesh in createdMeshes)
            {
                createdMesh?.Dispose();
            }
        }
    }

    /// <summary>
    /// Adds a Brep's edges to the buffers as polylines, so it still shows in wireframe.
    /// </summary>
    private void AddBrepEdges(Brep brep, IPreviewGeometryBuffer buffers, int maxItems)
    {
        this.AddCurves(brep.Edges, buffers, maxItems);
    }

    /// <summary>
    /// Adds a Brep to the buffers as a shell plus its edges.
    /// </summary>
    private void AddBrep(Brep brep, IPreviewGeometryBuffer buffers, MeshingParameters meshingParameters,
        int maxItems)
    {
        this.AddBrepShell(brep, buffers, meshingParameters);

        this.AddBrepEdges(brep, buffers, maxItems);
    }

    /// <summary>
    /// Adds an extrusion to the buffers as a shell plus its edges, using its cached render
    /// mesh when it has one.
    /// </summary>
    private void AddExtrusion(Extrusion extrusion, IPreviewGeometryBuffer buffers,
        MeshingParameters meshingParameters, int maxItems)
    {
        using var brep = extrusion.ToBrep(false);

        if (brep == null) return;

        var cachedMesh = extrusion.GetMesh(MeshType.Render);

        if (cachedMesh != null)
        {
            var cachedMeshes = new[] { cachedMesh };

            this.AddShell(cachedMeshes, buffers);
        }
        else
        {
            this.AddBrepShell(brep, buffers, meshingParameters);
        }

        this.AddBrepEdges(brep, buffers, maxItems);
    }

    /// <summary>
    /// Adds a surface to the buffers as a shell plus its edges.
    /// </summary>
    private void AddSurface(Surface surface, IPreviewGeometryBuffer buffers, MeshingParameters meshingParameters,
        int maxItems)
    {
        using var brep = surface.ToBrep();

        if (brep == null) return;

        this.AddBrep(brep, buffers, meshingParameters, maxItems);
    }

    /// <summary>
    /// Adds a SubD to the buffers as a shell of its limit surface plus its edges.
    /// </summary>
    /// <remarks>
    /// The edges are taken one at a time from <see cref="SubDEdge.ToNurbsCurve"/> because
    /// <c>SubD.DuplicateEdgeCurves</c> is missing from the RhinoCommon the net48 leg compiles
    /// against.
    /// </remarks>
    private void AddSubD(SubD subD, IPreviewGeometryBuffer buffers, int maxItems)
    {
        using var mesh = Mesh.CreateFromSubD(subD, _subDDisplayDensity);

        if (mesh != null)
        {
            var meshes = new[] { mesh };

            this.AddShell(meshes, buffers);
        }

        foreach (var edge in subD.Edges)
        {
            if (buffers.ItemCount >= maxItems) break;

            using var edgeCurve = edge.ToNurbsCurve(true);

            if (edgeCurve == null) continue;

            this.AddCurve(edgeCurve, buffers);
        }
    }

    /// <summary>
    /// Converts the fallback convertibles to AutoCAD entities within a single transaction on
    /// the active document, and adds the valid ones to the buffers until
    /// <paramref name="maxItems"/> is reached.
    /// </summary>
    /// <remarks>
    /// The transaction takes a document lock because the conversion is not purely a read:
    /// converting a hatch appends its boundary curves to model space and erases them again.
    /// Entities rejected by the validator, or beyond the limit, are disposed.
    /// </remarks>
    private void AddFallbackEntities(IAutocadDocument activeDocument, List<IRhinoConvertible> fallbackConvertibles,
        IGeometryPreviewSettings settings, IPreviewGeometryBuffer buffers, int maxItems)
    {
        var transactionManagerWrapper = activeDocument.CreateTransactionManager();

        var silent = true;

#if DEBUG
        silent = false;
#endif

        transactionManagerWrapper.PerformTask(() =>
        {
            foreach (var convertible in fallbackConvertibles)
            {
                if (buffers.ItemCount >= maxItems) break;

                var convertedEntities = new List<IEntity>();

                var keptEntities = new HashSet<CadEntity>();

                try
                {
                    convertedEntities = convertible.Convert(transactionManagerWrapper, settings);

                    var validEntities = _entityValidator.ValidateEntitiesForTransientManager(convertedEntities, silent);

                    foreach (var validEntity in validEntities)
                    {
                        if (buffers.ItemCount >= maxItems) break;

                        buffers.FallbackEntities.Add(validEntity);

                        var cadEntity = validEntity.Unwrap();

                        keptEntities.Add(cadEntity);
                    }
                }
                catch (Exception exception)
                {
                    LoggerService.Instance.LogError(exception,
                        $"Unable to build the preview of a {convertible.GetType().Name}; it is not previewed.");
                }
                finally
                {
                    this.DisposeUnused(convertedEntities, keptEntities);
                }
            }

            return true;
        });
    }

    /// <summary>
    /// Disposes the converted entities which were not kept as fallback entities, whether
    /// rejected, beyond the limit or abandoned by a failure.
    /// </summary>
    private void DisposeUnused(List<IEntity> convertedEntities, HashSet<CadEntity> keptEntities)
    {
        foreach (var convertedEntity in convertedEntities)
        {
            var cadEntity = convertedEntity?.Unwrap();

            if (cadEntity == null || cadEntity.IsDisposed || keptEntities.Contains(cadEntity)) continue;

            cadEntity.Dispose();
        }
    }
}
