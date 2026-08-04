using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;
using CivilVolumeSurface = Autodesk.Civil.DatabaseServices.TinVolumeSurface;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for Civil 3D TIN Volume Surfaces.
/// </summary>
public class GH_CivilTinVolumeSurface : GH_AutocadGeometricGoo<CivilVolumeSurface, VolumeSurfaceAdapter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilTinVolumeSurface"/> class with no value.
    /// </summary>
    public GH_CivilTinVolumeSurface()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilTinVolumeSurface"/> class with the
    /// specified Civil 3D TIN Volume Surface. Internally, the surface is cloned, but the autocad
    /// reference Id is maintained.
    /// </summary>
    /// <param name="volumeSurface">The Civil 3D TIN Volume Surface to wrap.</param>
    public GH_CivilTinVolumeSurface(CivilVolumeSurface volumeSurface) : base(volumeSurface)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input volume surface.
    /// </summary>
    private GH_CivilTinVolumeSurface(CivilVolumeSurface volumeSurface, IAutocadReferenceId referenceId)
        : base(volumeSurface, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CivilVolumeSurface, VolumeSurfaceAdapter> CreateClonedInstance(
        CivilVolumeSurface entity)
    {
        return new GH_CivilTinVolumeSurface(entity.Clone() as CivilVolumeSurface, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CivilVolumeSurface, VolumeSurfaceAdapter> CreateInstance(
        CivilVolumeSurface entity)
    {
        return new GH_CivilTinVolumeSurface(entity);
    }

    /// <inheritdoc />
    protected override CivilVolumeSurface? Convert(VolumeSurfaceAdapter rhinoType)
    {
        return rhinoType.ToCivilTinVolumeSurface();
    }

    /// <inheritdoc />
    protected override VolumeSurfaceAdapter? Convert(CivilVolumeSurface wrapperType)
    {
        return wrapperType.ToVolumeSurfaceAdapter();
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var combined = this.RhinoGeometry?.CombinedMesh;
        if (combined != null)
            args.Pipeline.DrawMeshWires(combined, args.Color, args.Thickness);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        var combined = this.RhinoGeometry?.CombinedMesh;
        if (combined != null)
            args.Pipeline.DrawMeshShaded(combined, args.Material);
    }

    /// <inheritdoc />
    public override void AppendInputSignature(IInputSignatureBuilder inputSignatureBuilder)
    {
        var adapter = this.RhinoGeometry;

        if (adapter == null)
        {
            inputSignatureBuilder.Add(this.ToString());
            return;
        }

        inputSignatureBuilder.AddMesh(adapter.BaseMesh);
        inputSignatureBuilder.AddMesh(adapter.ComparisonMesh);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var adapter = this.RhinoGeometry;
        if (adapter?.CombinedMesh == null) return;

        previewData.Meshes.Add(adapter.CombinedMesh);

        var polylines = adapter.CombinedMesh.GetNakedEdges();

        foreach (var polyline in polylines)
        {
            previewData.Wires.Add(new PolylineCurve(polyline));
        }
    }
}
