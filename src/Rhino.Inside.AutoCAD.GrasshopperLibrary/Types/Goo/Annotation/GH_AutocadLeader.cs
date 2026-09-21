using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AutocadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using AutocadLeader = Autodesk.AutoCAD.DatabaseServices.Leader;
using AutocadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoLeader = Rhino.Geometry.Leader;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD Leaders, holding either a legacy
/// Leader or an MLeader.
/// </summary>
/// <remarks>
/// AutoCAD has two unrelated leader entities: the legacy <c>LEADER</c>
/// (<see cref="AutocadLeader"/>, which derives from Curve) and <c>MLEADER</c>
/// (<see cref="AutocadMLeader"/>, which derives from Entity). Their only common ancestor
/// is <see cref="AutocadEntity"/>, so that is the wrapper type, and the concrete type is
/// resolved at runtime when converting to Rhino - the same approach
/// <see cref="GH_AutocadCurve"/> and <see cref="GH_AutocadDimension"/> take over their own
/// abstract AutoCAD base types. Both types are held as picked and bake back as themselves.
/// Converting the other way always produces an MLeader: Rhino has a single leader type and
/// the legacy entity is not worth synthesising.
/// </remarks>
[GooEntityTypes(typeof(AutocadMLeader), typeof(AutocadLeader))]
public class GH_AutocadLeader : GH_AutocadGeometricGoo<AutocadEntity, RhinoGeometryAdapter<RhinoLeader>>
{
    /// <inheritdoc />
    /// <remarks>
    /// Overridden because the wrapper type is <see cref="AutocadEntity"/>, which the base
    /// would render as "AutoCAD Entity".
    /// </remarks>
    public override string TypeName => "AutoCAD Leader";

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
    /// Initializes a new instance of the <see cref="GH_AutocadLeader"/> class with the
    /// specified legacy AutoCAD Leader. Internally, the leader is cloned, but the AutoCAD
    /// reference ID is maintained.
    /// </summary>
    /// <param name="leader">The legacy AutoCAD Leader to wrap.</param>
    public GH_AutocadLeader(AutocadLeader leader) : base(leader)
    {
    }

    /// <summary>
    /// A private constructor used when the concrete leader type is not known statically,
    /// such as when re-wrapping a value this Goo already holds.
    /// </summary>
    private GH_AutocadLeader(AutocadEntity leader) : base(leader)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input leader.
    /// </summary>
    private GH_AutocadLeader(AutocadEntity leader, IAutocadReferenceId referenceId) : base(leader, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadEntity, RhinoGeometryAdapter<RhinoLeader>> CreateClonedInstance(AutocadEntity entity)
    {
        return new GH_AutocadLeader(entity.Clone() as AutocadEntity, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadEntity, RhinoGeometryAdapter<RhinoLeader>> CreateInstance(AutocadEntity entity)
    {
        return new GH_AutocadLeader(entity);
    }

    /// <inheritdoc />
    /// <remarks>
    /// A Rhino leader always becomes an MLeader, never a legacy Leader.
    /// </remarks>
    protected override AutocadEntity? Convert(RhinoGeometryAdapter<RhinoLeader> rhinoType)
    {
        return rhinoType.Geometry?.ToAutocadMLeader();
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoLeader>? Convert(AutocadEntity wrapperType)
    {
        return new RhinoGeometryAdapter<RhinoLeader>(wrapperType.ToRhinoLeader());
    }

    /// <inheritdoc />
    /// <remarks>
    /// Drawn as an annotation so the arrowheads and the text render, not just the leader
    /// line. The curve is drawn as well because <c>DrawAnnotation</c> ignores
    /// <see cref="GH_PreviewWireArgs.Thickness"/>, which is what thickens a selected wire.
    /// </remarks>
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry == null) return;

        args.Pipeline.DrawAnnotation(geometry, args.Color);

        var curve = geometry.Curve;

        if (curve != null)
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
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
    /// <remarks>
    /// Exploded into its wires and text. The leader's Curve is only the leader line, so on
    /// its own it previews without the arrowheads or the text. Exploding is also what
    /// <see cref="GH_AutocadDimension"/> does, because the AutoCAD preview draws transients
    /// and <see cref="IGrasshopperPreviewData.Leaders"/> has never been used by anything.
    /// </remarks>
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var geometry = this.RhinoGeometry?.Geometry;

        if (geometry == null) return;

        var geometryBases = geometry.Explode();

        if (geometryBases == null) return;

        foreach (var geometryBase in geometryBases)
        {
            if (geometryBase is RhinoCurve curve)
            {
                previewData.Wires.Add(curve);
                continue;
            }

            if (geometryBase is TextEntity textEntity)
            {
                previewData.Texts.Add(textEntity);
            }
        }
    }
}
