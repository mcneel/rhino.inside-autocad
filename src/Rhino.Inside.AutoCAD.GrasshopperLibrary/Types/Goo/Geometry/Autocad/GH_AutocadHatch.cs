using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadHatch = Autodesk.AutoCAD.DatabaseServices.Hatch;
using RhinoHatch = Rhino.Geometry.Hatch;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD hatches.
/// </summary>
public class GH_AutocadHatch : GH_AutocadGeometricGoo<CadHatch, RhinoHatch>
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
    protected override GH_AutocadGeometricGoo<CadHatch, RhinoHatch> CreateClonedInstance(CadHatch entity)
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
    protected override GH_AutocadGeometricGoo<CadHatch, RhinoHatch> CreateInstance(CadHatch entity)
    {
        return new GH_AutocadHatch(entity);
    }

    /// <inheritdoc />
    protected override CadHatch? Convert(RhinoHatch rhinoType)
    {
        return _geometryConverter.ToAutoCadType(rhinoType, null!);
    }

    /// <inheritdoc />
    protected override RhinoHatch? Convert(CadHatch wrapperType)
    {
        return _geometryConverter.ToRhinoType(wrapperType);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var rhinoGeometry = this.RhinoGeometry;
        if (rhinoGeometry == null)
            return;

        foreach (var curve in rhinoGeometry.Get3dCurves(true))
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }

        foreach (var curve in rhinoGeometry.Get3dCurves(false))
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        var rhinoGeometry = this.RhinoGeometry;
        if (rhinoGeometry == null)
            return;

        args.Pipeline.DrawHatch(rhinoGeometry, args.Material.BackDiffuse, args.Material.BackDiffuse);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var rhinoGeometry = this.RhinoGeometry;
        if (rhinoGeometry == null)
            return;

        foreach (var curve in rhinoGeometry.Get3dCurves(true))
        {
            previewData.Wires.Add(curve);
        }

        foreach (var curve in rhinoGeometry.Get3dCurves(false))
        {
            previewData.Wires.Add(curve);
        }
    }
}
