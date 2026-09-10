using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CivilSurface = Autodesk.Civil.DatabaseServices.TinSurface;
using RhinoMesh = Rhino.Geometry.Mesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD meshes.
/// </summary>
public class GH_CivilTinSurface : GH_AutocadGeometricGoo<CivilSurface, RhinoGeometryAdapter<RhinoMesh>>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilTinSurface"/> class with no value.
    /// </summary>
    public GH_CivilTinSurface()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilTinSurface"/> class with the
    /// specified AutoCAD mesh. Internally, the mesh is cloned, but the autocad
    /// reference Id is maintained.
    /// </summary>
    /// <param name="mesh">The AutoCAD mesh to wrap.</param>
    public GH_CivilTinSurface(CivilSurface mesh) : base(mesh)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_CivilTinSurface(CivilSurface curve, IAutocadReferenceId referenceId) : base(curve, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CivilSurface, RhinoGeometryAdapter<RhinoMesh>> CreateClonedInstance(CivilSurface entity)
    {
        return new GH_CivilTinSurface(entity.Clone() as CivilSurface, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CivilSurface, RhinoGeometryAdapter<RhinoMesh>> CreateInstance(CivilSurface entity)
    {
        return new GH_CivilTinSurface(entity);
    }

    /// <inheritdoc />
    protected override CivilSurface? Convert(RhinoGeometryAdapter<RhinoMesh> rhinoType)
    {
        return rhinoType.Geometry?.ToTinSurface();
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoMesh>? Convert(CivilSurface wrapperType)
    {
        return new RhinoGeometryAdapter<RhinoMesh>(wrapperType.ToRhinoMesh());
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        args.Pipeline.DrawMeshWires(this.RhinoGeometry?.Geometry, args.Color, args.Thickness);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        args.Pipeline.DrawMeshShaded(this.RhinoGeometry?.Geometry, args.Material);
    }

    /// <inheritdoc />
    public override void AppendInputSignature(IInputSignatureBuilder inputSignatureBuilder)
    {
        var geometry = this.RhinoGeometry?.Geometry;

        if (geometry == null)
        {
            inputSignatureBuilder.Add(this.ToString());
            return;
        }

        inputSignatureBuilder.AddMesh(geometry);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var geometry = this.RhinoGeometry?.Geometry;

        if (geometry == null) return;

        previewData.Meshes.Add(geometry);

        var polylines = geometry.GetNakedEdges();

        foreach (var polyline in polylines)
        {
            previewData.Wires.Add(new PolylineCurve(polyline));
        }
    }
}

