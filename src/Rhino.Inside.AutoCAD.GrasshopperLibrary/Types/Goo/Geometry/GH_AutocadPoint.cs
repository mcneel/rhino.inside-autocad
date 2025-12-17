using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using AutocadPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using RhinoPoint = Rhino.Geometry.Point;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD points.
/// </summary>
public class GH_AutocadPoint : GH_AutocadGeometricGoo<AutocadPoint, RhinoPoint>
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
    protected override GH_AutocadGeometricGoo<AutocadPoint, RhinoPoint> CreateClonedInstance(AutocadPoint entity)
    {
        return new GH_AutocadPoint(entity.Clone() as AutocadPoint, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<AutocadPoint, RhinoPoint> CreateInstance(AutocadPoint entity)
    {
        return new GH_AutocadPoint(entity);
    }

    /// <inheritdoc />
    protected override AutocadPoint? Convert(RhinoPoint rhinoType)
    {
        return _geometryConverter.ToAutoCadType(rhinoType);
    }

    /// <inheritdoc />
    protected override RhinoPoint? Convert(AutocadPoint wrapperType)
    {
        return _geometryConverter.ToRhinoType(wrapperType);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        args.Pipeline.DrawPoint(this.RhinoGeometry.Location, args.Color);
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        return;
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var rhinoGeometry = this.RhinoGeometry;

        if (rhinoGeometry == null) return;

        previewData.Points.Add(new RhinoPoint(rhinoGeometry.Location));
    }
}