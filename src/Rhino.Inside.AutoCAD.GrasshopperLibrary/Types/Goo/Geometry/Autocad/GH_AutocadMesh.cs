using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AutocadMesh = Autodesk.AutoCAD.DatabaseServices.SubDMesh;
using RhinoMesh = Rhino.Geometry.Mesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD meshes.
/// </summary>
public class GH_AutocadMesh : GH_AutocadGeometricGoo<AutocadMesh, RhinoGeometryAdapter<RhinoMesh>>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadMesh"/> class with no value.
    /// </summary>
    public GH_AutocadMesh()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadMesh"/> class with the
    /// specified AutoCAD mesh. Internally, the mesh is cloned, but the autocad
    /// reference Id is maintained.
    /// </summary>
    /// <param name="mesh">The AutoCAD mesh to wrap.</param>
    public GH_AutocadMesh(AutocadMesh mesh) : base(mesh)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadMesh(AutocadMesh curve, IAutocadReferenceId referenceId) : base(curve, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadMesh, RhinoGeometryAdapter<RhinoMesh>> CreateClonedInstance(AutocadMesh entity)
    {
        return new GH_AutocadMesh(entity.Clone() as AutocadMesh, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadMesh, RhinoGeometryAdapter<RhinoMesh>> CreateInstance(AutocadMesh entity)
    {
        return new GH_AutocadMesh(entity);
    }

    /// <inheritdoc />
    protected override AutocadMesh? Convert(RhinoGeometryAdapter<RhinoMesh> rhinoType)
    {
        return rhinoType.Geometry?.ToAutocadSubDMesh();
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoMesh>? Convert(AutocadMesh wrapperType)
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

