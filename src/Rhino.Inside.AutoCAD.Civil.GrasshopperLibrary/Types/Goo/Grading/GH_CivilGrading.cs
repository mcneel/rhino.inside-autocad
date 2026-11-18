using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CivilGrading = Autodesk.Civil.DatabaseServices.Grading;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for Civil 3D Gradings.
/// </summary>
/// <remarks>
/// Preview geometry comes from exploding the grading, so it reflects what the grading
/// style displays.
/// </remarks>
public class GH_CivilGrading : GH_CivilOneWayGoo<CivilGrading, RhinoCurveCollectionAdapter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilGrading"/> class with no value.
    /// </summary>
    public GH_CivilGrading()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilGrading"/> class with the
    /// specified Civil 3D Grading.
    /// </summary>
    /// <param name="grading">The Civil 3D Grading to wrap.</param>
    public GH_CivilGrading(CivilGrading grading) : base(grading)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input grading.
    /// </summary>
    private GH_CivilGrading(CivilGrading grading, IAutocadReferenceId referenceId) : base(grading, referenceId)
    {
    }

    /// <inheritdoc />
    public override string TypeName => "Civil3d Grading";

    /// <inheritdoc />
    public override string TypeDescription => "A Civil 3D Grading";

    /// <inheritdoc />
    protected override GH_CivilOneWayGoo<CivilGrading, RhinoCurveCollectionAdapter> CreateClonedInstance(CivilGrading entity)
    {
        return new GH_CivilGrading(entity.Clone() as CivilGrading, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_CivilOneWayGoo<CivilGrading, RhinoCurveCollectionAdapter> CreateInstance(CivilGrading entity)
    {
        return new GH_CivilGrading(entity);
    }

    /// <inheritdoc />
    protected override RhinoCurveCollectionAdapter? ConvertToRhino(CivilGrading wrapperType)
    {
        var curves = wrapperType.ToRhinoCurves();
        return curves.Count > 0 ? new RhinoCurveCollectionAdapter(curves) : null;
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var curves = this.RhinoGeometry?.Curves;
        if (curves == null) return;

        foreach (var curve in curves)
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        // Grading preview is drawn as wires only
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var curves = this.RhinoGeometry?.Curves;
        if (curves == null) return;

        foreach (var curve in curves)
        {
            previewData.Wires.Add(curve);
        }
    }
}
