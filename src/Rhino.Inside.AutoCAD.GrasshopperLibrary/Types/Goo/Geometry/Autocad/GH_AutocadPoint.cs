using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AutocadPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using RhinoPoint = Rhino.Geometry.Point;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD points.
/// </summary>
public class GH_AutocadPoint : GH_AutocadGeometricGoo<AutocadPoint, RhinoGeometryAdapter<RhinoPoint>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadPoint"/> class with no
    /// value.
    /// </summary>
    public GH_AutocadPoint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadPoint"/> class with the
    /// specified AutoCAD point. Internally, the curve is cloned, but the autocad
    /// reference ID is maintained.
    /// </summary>
    /// <param name="point">The AutoCAD point to wrap.</param>
    public GH_AutocadPoint(AutocadPoint point) : base(point)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadPoint(AutocadPoint curve, IAutocadReferenceId referenceId) : base(curve, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadPoint, RhinoGeometryAdapter<RhinoPoint>> CreateClonedInstance(AutocadPoint entity)
    {
        return new GH_AutocadPoint(entity.Clone() as AutocadPoint, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadPoint, RhinoGeometryAdapter<RhinoPoint>> CreateInstance(AutocadPoint entity)
    {
        return new GH_AutocadPoint(entity);
    }

    /// <inheritdoc />
    protected override AutocadPoint? Convert(RhinoGeometryAdapter<RhinoPoint> rhinoType)
    {
        return rhinoType.Geometry?.ToAutocadDBPoint();
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoPoint>? Convert(AutocadPoint wrapperType)
    {
        return new RhinoGeometryAdapter<RhinoPoint>(wrapperType.ToRhinoPoint());
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var geometry = this.RhinoGeometry?.Geometry;
        if (geometry != null)
            args.Pipeline.DrawPoint(geometry.Location, args.Color);
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

        previewData.Points.Add(new RhinoPoint(geometry.Location));
    }
}