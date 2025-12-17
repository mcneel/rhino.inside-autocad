using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoBrep = Rhino.Geometry.Brep;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for Breps using a <see cref="AutocadBrepProxy"/>
/// a sudo Brep created from AutoCAD Nurbs Surfaces.
/// </summary>
public class GH_AutocadBrepProxy : GH_AutocadGeometricGoo<AutocadBrepProxy, RhinoBrep>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBrepProxy"/> class with no
    /// value.
    /// </summary>
    public GH_AutocadBrepProxy()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBrepProxy"/> class with the
    /// specified <see cref="AutocadBrepProxy"/>. Internally, the geometry is cloned,
    /// but the autocad reference ID is maintained.
    /// </summary>
    /// <param name="solid">The <see cref="AutocadBrepProxy"/> to wrap.</param>
    public GH_AutocadBrepProxy(AutocadBrepProxy solid) : base(solid)
    {

    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadBrepProxy(AutocadBrepProxy proxy, IAutocadReferenceId referenceId) : base(proxy, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadBrepProxy, RhinoBrep> CreateClonedInstance(AutocadBrepProxy entity)
    {
        return new GH_AutocadBrepProxy(entity.Clone() as AutocadBrepProxy, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadBrepProxy, RhinoBrep> CreateInstance(AutocadBrepProxy entity)
    {
        return new GH_AutocadBrepProxy(entity);
    }

    /// <inheritdoc />
    protected override AutocadBrepProxy? Convert(RhinoBrep rhinoType)
    {
        return _geometryConverter.ToProxyType(rhinoType);
    }

    /// <inheritdoc />
    protected override RhinoBrep? Convert(AutocadBrepProxy wrapperType)
    {
        return _geometryConverter.FromProxyType(wrapperType);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        args.Pipeline.DrawBrepWires(this.RhinoGeometry, args.Color);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        args.Pipeline.DrawBrepShaded(this.RhinoGeometry, args.Material);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var rhinoGeometry = this.RhinoGeometry;

        if (rhinoGeometry == null) return;

        var meshes = Mesh.CreateFromBrep(rhinoGeometry, MeshingParameters.Default);

        previewData.Meshes.AddRange(meshes);

        var polylines = rhinoGeometry.Curves3D;

        foreach (var polyline in polylines)
        {
            previewData.Wires.Add(polyline);
        }
    }

    /// <inheritdoc />
    public override List<IObjectId> BakeToAutocad(ITransactionManager transactionManager, IBakeSettings? settings = null)
    {
        if (this.Value == null)
            throw new InvalidOperationException("Cannot bake a null block reference");

        var transaction = transactionManager.Unwrap();

        var modelSpace = transactionManager.GetModelSpaceBlockTableRecord(openForWrite: true);

        var modelSpaceRecord = modelSpace.Unwrap();

        var ids = new List<IObjectId>();

        foreach (var face in this.Value.Faces)
        {
            var entity = (Entity)face.Clone();

            this.ApplySettings(settings, entity);

            var objectId = modelSpaceRecord.AppendEntity(entity);

            transaction.AddNewlyCreatedDBObject(entity, true);

            var id = new AutocadObjectId(objectId);

            ids.Add(id);
        }

        return ids;
    }
}
