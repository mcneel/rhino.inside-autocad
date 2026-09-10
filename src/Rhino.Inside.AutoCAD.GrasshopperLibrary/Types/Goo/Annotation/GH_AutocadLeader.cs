using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AutocadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;
using RhinoLeader = Rhino.Geometry.Leader;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD Leaders (MLeader).
/// </summary>
public class GH_AutocadLeader : GH_AutocadGeometricGoo<AutocadMLeader, RhinoGeometryAdapter<RhinoLeader>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLeader"/> class with no value.
    /// </summary>
    public GH_AutocadLeader()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLeader"/> class with the
    /// specified AutoCAD MLeader. Internally, the leader is cloned, but the AutoCAD
    /// reference ID is maintained.
    /// </summary>
    /// <param name="leader">The AutoCAD MLeader to wrap.</param>
    public GH_AutocadLeader(AutocadMLeader leader) : base(leader)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input leader.
    /// </summary>
    private GH_AutocadLeader(AutocadMLeader leader, IAutocadReferenceId referenceId) : base(leader, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadMLeader, RhinoGeometryAdapter<RhinoLeader>> CreateClonedInstance(AutocadMLeader entity)
    {
        return new GH_AutocadLeader(entity.Clone() as AutocadMLeader, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadMLeader, RhinoGeometryAdapter<RhinoLeader>> CreateInstance(AutocadMLeader entity)
    {
        return new GH_AutocadLeader(entity);
    }

    /// <inheritdoc />
    protected override AutocadMLeader? Convert(RhinoGeometryAdapter<RhinoLeader> rhinoType)
    {
        return rhinoType.Geometry?.ToAutocadMLeader();
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoLeader>? Convert(AutocadMLeader wrapperType)
    {
        return new RhinoGeometryAdapter<RhinoLeader>(wrapperType.ToRhinoLeader());
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry == null) return;

        var curve = geometry.Curve;
        if (curve != null)
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        return;
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

        inputSignatureBuilder.AddGeometry(geometry);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var geometry = this.RhinoGeometry?.Geometry;

        if (geometry == null) return;

        var curve = geometry.Curve;

        previewData.Wires.Add(curve);
    }
}
