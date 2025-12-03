using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Interop;
using AutocadSolid = Autodesk.AutoCAD.DatabaseServices.Solid3d;
using RhinoBrep = Rhino.Geometry.Brep;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD solids.
/// </summary>
public class GH_AutocadSolid : GH_GeometricGoo<AutocadSolid>, IGH_PreviewData
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Gets the Rhino geometry equivalent of the AutoCAD solid.
    /// </summary>
    public RhinoBrep[]? RhinoGeometry =>
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
                return "No internal AutocadSolid data";
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public override string TypeName => "AutocadSolid";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD solid object";

    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadSolid"/> class with no value.
    /// </summary>
    public GH_AutocadSolid()
    {
        this.Value = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadSolid"/> class with the specified AutoCAD solid.
    /// </summary>
    /// <param name="solid">The AutoCAD solid to wrap.</param>
    public GH_AutocadSolid(AutocadSolid solid)
    {
        this.Value = solid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadSolid"/> class by copying another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadSolid(GH_AutocadSolid other)
    {
        this.Value = other.Value;
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo DuplicateGeometry()
    {
        var clone = this.Value.Clone() as AutocadSolid;

        return new GH_AutocadSolid(clone);
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
        //TODO: This should be updated when we have a proper AutoCAD to Rhino Brep Conversation implementation
        if (this.RhinoGeometry == null || this.RhinoGeometry.Length != 1)
            return this;

        var rhinoSolid = this.RhinoGeometry[0];

        rhinoSolid.Transform(xform);

        var morphedSolids = _geometryConverter.ToAutoCadType(rhinoSolid);

        return new GH_AutocadSolid(morphedSolids[0]);
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        //TODO: This should be updated when we have a proper AutoCAD to Rhino Brep Conversation implementation

        if (this.RhinoGeometry == null || this.RhinoGeometry.Length != 1)
            return this;

        var rhinoSolid = this.RhinoGeometry[0];

        xmorph.Morph(rhinoSolid);

        var morphedSolid = _geometryConverter.ToAutoCadType(rhinoSolid);

        return new GH_AutocadSolid(morphedSolid[0]);
    }

    /// <inheritdoc />
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
        if (this.RhinoGeometry == null)
            return;

        foreach (var brep in this.RhinoGeometry)
        {
            args.Pipeline.DrawBrepWires(brep, args.Color);
        }
    }

    /// <inheritdoc />
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
        if (this.RhinoGeometry == null)
            return;

        foreach (var brep in this.RhinoGeometry)
        {
            args.Pipeline.DrawBrepShaded(brep, args.Material);
        }
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadSolid goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadSolid solid)
        {
            this.Value = solid;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadSolid)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadSolid)))
        {
            target = (Q)(object)new GH_AutocadSolid(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadSolid";

        return $"AutocadSolid [{this.Value.GetType().ToString()}]";
    }
}

