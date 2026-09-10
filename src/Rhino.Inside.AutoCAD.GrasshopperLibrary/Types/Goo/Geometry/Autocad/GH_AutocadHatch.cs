using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;
using RhinoHatch = Rhino.Geometry.Hatch;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD hatches.
/// </summary>
public class GH_AutocadHatch : GH_AutocadGeometricGoo<CadHatch, RhinoGeometryAdapter<RhinoHatch>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadHatch"/> class with no
    /// value.
    /// </summary>
    public GH_AutocadHatch()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadHatch"/> class with the
    /// specified AutoCAD hatch. Internally, the hatch is cloned, but the autocad
    /// reference Id is maintained.
    /// </summary>
    /// <param name="hatch">The AutoCAD hatch to wrap.</param>
    public GH_AutocadHatch(CadHatch hatch) : base(hatch)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is not a clone of the
    /// input hatch.
    /// </summary>
    private GH_AutocadHatch(CadHatch hatch, IAutocadReferenceId referenceId) : base(hatch, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CadHatch, RhinoGeometryAdapter<RhinoHatch>> CreateClonedInstance(CadHatch entity)
    {
        if (this.Reference.IsValid)
        {
            var picker = new AutocadObjectPicker();
            if (picker.TryGetUpdatedObject(this.Reference.ObjectId, out var updatedEntity)
                && updatedEntity!.Unwrap() is CadHatch hatch)
            {
                return new GH_AutocadHatch(hatch);
            }
        }

        return new GH_AutocadHatch(this.Value);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CadHatch, RhinoGeometryAdapter<RhinoHatch>> CreateInstance(CadHatch entity)
    {
        return new GH_AutocadHatch(entity);
    }

    /// <inheritdoc />
    protected override CadHatch? Convert(RhinoGeometryAdapter<RhinoHatch> rhinoType)
    {
        return rhinoType.Geometry?.ToAutocadHatch(null!);
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoHatch>? Convert(CadHatch wrapperType)
    {
        return new RhinoGeometryAdapter<RhinoHatch>(wrapperType.ToRhinoHatch());
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry == null)
            return;

        foreach (var curve in geometry.Get3dCurves(true))
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }

        foreach (var curve in geometry.Get3dCurves(false))
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry == null)
            return;

        args.Pipeline.DrawHatch(geometry, args.Material.BackDiffuse, args.Material.BackDiffuse);
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
        if (geometry == null)
            return;

        foreach (var curve in geometry.Get3dCurves(true))
        {
            previewData.Wires.Add(curve);
        }

        foreach (var curve in geometry.Get3dCurves(false))
        {
            previewData.Wires.Add(curve);
        }
    }
}
