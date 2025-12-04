using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Interop;
using AutocadPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
using RhinoPoint = Rhino.Geometry.Point;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD points.
/// </summary>
public class GH_AutocadPoint : GH_GeometricGoo<AutocadPoint>, IGH_PreviewData
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Gets the Rhino geometry equivalent of the AutoCAD point.
    /// </summary>
    public RhinoPoint RhinoGeometry => _geometryConverter.ToRhinoType(this.Value);

    /// <inheritdoc />
    public override BoundingBox Boundingbox =>
        new BoundingBox(this.RhinoGeometry.Location, this.RhinoGeometry.Location);

    /// <inheritdoc />
    public BoundingBox ClippingBox => this.Boundingbox;

    /// <inheritdoc />
    public override string IsValidWhyNot
    {
        get
        {
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public override string TypeName => "AutocadPoint";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD point object";

    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadPoint"/> class with no value.
    /// </summary>
    public GH_AutocadPoint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadPoint"/> class with the specified AutoCAD point.
    /// </summary>
    /// <param name="point">The AutoCAD point to wrap.</param>
    public GH_AutocadPoint(AutocadPoint point) : base(point.Clone() as AutocadPoint)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadPoint"/> class by copying another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadPoint(GH_AutocadPoint other)
    {
        m_value = other.m_value;
        if (m_value != null)
            m_value = m_value.Clone() as AutocadPoint;
    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate() => (IGH_Goo)this.DuplicatePoint();

    /// <inheritdoc />
    public override IGH_GeometricGoo DuplicateGeometry()
    {
        return (IGH_GeometricGoo)this.DuplicatePoint();
    }

    /// <summary>
    /// Duplicates this <see cref="GH_AutocadPoint"/> instance.
    /// </summary>
    /// <returns></returns>
    public GH_AutocadPoint DuplicatePoint() => new GH_AutocadPoint(this);

    /// <inheritdoc />
    public override BoundingBox GetBoundingBox(Transform xform)
    {
        if (this.Value == null)
            return BoundingBox.Empty;

        var box = this.Boundingbox;
        box.Transform(xform);
        return box;
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Transform(Transform xform)
    {
        if (this.RhinoGeometry == null)
            return this;

        var rhinoPoint = this.RhinoGeometry;

        rhinoPoint.Transform(xform);

        var morphedPoints = _geometryConverter.ToAutoCadType(rhinoPoint);

        return new GH_AutocadPoint(morphedPoints);
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        if (this.RhinoGeometry == null)
            return this;

        var rhinoPoint = this.RhinoGeometry;

        xmorph.Morph(rhinoPoint);

        var morphedPoint = _geometryConverter.ToAutoCadType(rhinoPoint);

        return new GH_AutocadPoint(morphedPoint);
    }

    /// <inheritdoc />
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
        if (this.RhinoGeometry == null)
            return;

        args.Pipeline.DrawPoint(this.RhinoGeometry.Location, args.Color);
    }

    /// <inheritdoc />
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
        return;
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadPoint goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadPoint point)
        {
            this.Value = point;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadPoint)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadPoint)))
        {
            target = (Q)(object)new GH_AutocadPoint(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadPoint";
        var position = this.Value.Position;

        return $"AutocadPoint [{position.X}, {position.Y}, {position.Z}]";
    }
}