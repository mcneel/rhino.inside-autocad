using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Interop;
using AutocadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD curves.
/// </summary>
public class GH_AutocadCurve : GH_GeometricGoo<AutocadCurve>, IGH_PreviewData
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Gets the Rhino geometry equivalent of the AutoCAD curve.
    /// </summary>
    public RhinoCurve? RhinoGeometry =>
        this.Value == null
            ? null
            : _geometryConverter.ToRhinoType(this.Value);

    /// <inheritdoc />
    public override BoundingBox Boundingbox
    {
        get
        {
            if (this.Value == null && this.Value.Bounds.HasValue == false)
                return BoundingBox.Empty;

            var bounds = this.Value.Bounds;

            return _geometryConverter.ToRhinoType(bounds!.Value);
        }
    }

    /// <inheritdoc />
    public BoundingBox ClippingBox => this.Boundingbox;

    /// <inheritdoc />
    public override string IsValidWhyNot
    {
        get
        {
            if (this.Value == null)
                return "No internal AutocadCurve data";
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public override string TypeName => "AutocadCurve";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD curve object";

    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadCurve"/> class with no value.
    /// </summary>
    public GH_AutocadCurve()
    {
        this.Value = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadCurve"/> class with the specified AutoCAD curve.
    /// </summary>
    /// <param name="curve">The AutoCAD curve to wrap.</param>
    public GH_AutocadCurve(AutocadCurve curve)
    {
        this.Value = curve;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadCurve"/> class by copying another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadCurve(GH_AutocadCurve other)
    {
        this.Value = other.Value;
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo DuplicateGeometry()
    {
        var clone = this.Value.Clone() as AutocadCurve;

        return new GH_AutocadCurve(clone);
    }

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

        var rhinoCurve = this.RhinoGeometry;

        rhinoCurve.Transform(xform);

        var morphedCurves = _geometryConverter.ToAutoCadSingleCurve(rhinoCurve);

        return new GH_AutocadCurve(morphedCurves);
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        if (this.RhinoGeometry == null)
            return this;

        var rhinoCurve = this.RhinoGeometry;

        xmorph.Morph(rhinoCurve);

        var morphedCurve = _geometryConverter.ToAutoCadSingleCurve(rhinoCurve);

        return new GH_AutocadCurve(morphedCurve);
    }

    /// <inheritdoc />
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
        if (this.RhinoGeometry == null)
            return;

        args.Pipeline.DrawCurve(this.RhinoGeometry, args.Color, args.Thickness);
    }

    /// <inheritdoc />
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
        return;
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadCurve goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadCurve curve)
        {
            this.Value = curve;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadCurve)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadCurve)))
        {
            target = (Q)(object)new GH_AutocadCurve(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadCurve";

        return $"AutocadCurve [{this.Value.GetType().ToString()}]";
    }
}