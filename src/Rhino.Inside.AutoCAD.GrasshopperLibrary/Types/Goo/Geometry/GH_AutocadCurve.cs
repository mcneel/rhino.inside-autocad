using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutocadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD curves.
/// </summary>
public class GH_AutocadCurve : GH_AutocadGeometricGoo<AutocadCurve, RhinoCurve>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadCurve"/> class with no
    /// value.
    /// </summary>
    public GH_AutocadCurve()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadCurve"/> class with the
    /// specified AutoCAD curve. Internally, the curve is cloned, but the autocad
    /// reference Id is maintained.
    /// </summary>
    /// <param name="curve">The AutoCAD curve to wrap.</param>
    public GH_AutocadCurve(AutocadCurve curve) : base(curve)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is not a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadCurve(AutocadCurve curve, IObjectId referenceId) : base(curve, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadCurve, RhinoCurve> CreateClonedInstance(AutocadCurve entity)
    {
        return new GH_AutocadCurve(entity.Clone() as AutocadCurve, this.AutocadReferenceId);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadCurve, RhinoCurve> CreateInstance(AutocadCurve entity)
    {
        return new GH_AutocadCurve(entity);
    }

    /// <inheritdoc />
    protected override AutocadCurve? Convert(RhinoCurve rhinoType)
    {
        return _geometryConverter.ToAutoCadSingleCurve(rhinoType);
    }

    /// <inheritdoc />
    protected override RhinoCurve? Convert(AutocadCurve wrapperType)
    {
        return _geometryConverter.ToRhinoType(wrapperType);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        args.Pipeline.DrawCurve(this.RhinoGeometry, args.Color, args.Thickness);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        return;
    }
}

