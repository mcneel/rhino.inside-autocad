using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutocadMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;
using RhinoMesh = Rhino.Geometry.Mesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD meshes.
/// </summary>
public class GH_AutocadMesh : GH_AutocadGeometricGoo<AutocadMesh, RhinoMesh>
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
    private GH_AutocadMesh(AutocadMesh curve, IObjectId referenceId) : base(curve, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadMesh, RhinoMesh> CreateClonedInstance(AutocadMesh entity)
    {
        return new GH_AutocadMesh(entity.Clone() as AutocadMesh, this.AutocadReferenceId);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadMesh, RhinoMesh> CreateInstance(AutocadMesh entity)
    {
        return new GH_AutocadMesh(entity);
    }

    /// <inheritdoc />
    protected override AutocadMesh? Convert(RhinoMesh rhinoType)
    {
        return _geometryConverter.ToAutoCadType(rhinoType);
    }

    /// <inheritdoc />
    protected override RhinoMesh? Convert(AutocadMesh wrapperType)
    {
        return _geometryConverter.ToRhinoType(wrapperType);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        args.Pipeline.DrawMeshWires(this.RhinoGeometry, args.Color, args.Thickness);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        args.Pipeline.DrawMeshShaded(this.RhinoGeometry, args.Material);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var rhinoGeometry = this.RhinoGeometry;

        if (rhinoGeometry == null) return;

        previewData.Meshes.Add(rhinoGeometry);

        var polylines = rhinoGeometry.GetNakedEdges();

        foreach (var polyline in polylines)
        {
            previewData.Wires.Add(new PolylineCurve(polyline));
        }
    }
}

