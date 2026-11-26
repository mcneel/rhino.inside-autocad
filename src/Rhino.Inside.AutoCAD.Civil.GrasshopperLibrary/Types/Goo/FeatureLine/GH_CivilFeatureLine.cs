using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CivilFeatureLine = Autodesk.Civil.DatabaseServices.FeatureLine;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for standalone Civil 3D Feature Lines.
/// </summary>
/// <remarks>
/// Not to be confused with <see cref="GH_CivilCorridorFeatureLine"/>, which wraps a
/// feature line extracted from a corridor baseline.
/// </remarks>
public class GH_CivilFeatureLine : GH_CivilOneWayGoo<CivilFeatureLine, RhinoGeometryAdapter<RhinoCurve>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilFeatureLine"/> class with no value.
    /// </summary>
    public GH_CivilFeatureLine()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilFeatureLine"/> class with the
    /// specified Civil 3D Feature Line.
    /// </summary>
    /// <param name="featureLine">The Civil 3D Feature Line to wrap.</param>
    public GH_CivilFeatureLine(CivilFeatureLine featureLine) : base(featureLine)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input feature line.
    /// </summary>
    private GH_CivilFeatureLine(CivilFeatureLine featureLine, IAutocadReferenceId referenceId)
        : base(featureLine, referenceId)
    {
    }

    /// <inheritdoc />
    public override string TypeName => "Civil3d Feature Line";

    /// <inheritdoc />
    public override string TypeDescription => "A standalone Civil 3D Feature Line";

    /// <inheritdoc />
    protected override GH_CivilOneWayGoo<CivilFeatureLine, RhinoGeometryAdapter<RhinoCurve>> CreateClonedInstance(CivilFeatureLine entity)
    {
        return new GH_CivilFeatureLine(entity.Clone() as CivilFeatureLine, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_CivilOneWayGoo<CivilFeatureLine, RhinoGeometryAdapter<RhinoCurve>> CreateInstance(CivilFeatureLine entity)
    {
        return new GH_CivilFeatureLine(entity);
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoCurve>? ConvertToRhino(CivilFeatureLine wrapperType)
    {
        var curve = wrapperType.ToRhinoCurve();
        return curve != null ? new RhinoGeometryAdapter<RhinoCurve>(curve) : null;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (base.CastTo(ref target))
            return true;

        var curve = this.RhinoGeometry?.Geometry;

        if (curve == null)
            return false;

        if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
        {
            target = (Q)(object)new GH_Curve(curve.DuplicateCurve());
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(RhinoCurve)))
        {
            target = (Q)(object)curve.DuplicateCurve();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var curve = this.RhinoGeometry?.Geometry;
        if (curve != null)
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        // Feature lines are curves, drawn as wires only
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var curve = this.RhinoGeometry?.Geometry;

        if (curve == null) return;

        previewData.Wires.Add(curve);
    }
}
