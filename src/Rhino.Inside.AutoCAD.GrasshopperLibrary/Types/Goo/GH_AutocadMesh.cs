using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Interop;
using AutocadMesh = Autodesk.AutoCAD.DatabaseServices.PolyFaceMesh;
using RhinoMesh = Rhino.Geometry.Mesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD meshs.
/// </summary>
public class GH_AutocadMesh : GH_GeometricGoo<AutocadMesh>, IGH_PreviewData
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Gets the Rhino geometry equivalent of the AutoCAD mesh.
    /// </summary>
    public RhinoMesh? RhinoGeometry =>
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
                return "No internal AutocadMesh data";
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public override string TypeName => "AutocadMesh";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD mesh object";

    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadMesh"/> class with no value.
    /// </summary>
    public GH_AutocadMesh()
    {
        this.Value = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadMesh"/> class with the specified AutoCAD mesh.
    /// </summary>
    /// <param name="mesh">The AutoCAD mesh to wrap.</param>
    public GH_AutocadMesh(AutocadMesh mesh)
    {
        this.Value = mesh;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadMesh"/> class by copying another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadMesh(GH_AutocadMesh other)
    {
        this.Value = other.Value;
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo DuplicateGeometry()
    {
        var clone = this.Value.Clone() as AutocadMesh;

        return new GH_AutocadMesh(clone);
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

        var rhinoMesh = this.RhinoGeometry;

        rhinoMesh.Transform(xform);

        var polyFaceMesh = _geometryConverter.ToAutoCadType(rhinoMesh);

        return new GH_AutocadMesh(polyFaceMesh);
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        if (this.RhinoGeometry == null)
            return this;

        var rhinoMesh = this.RhinoGeometry;

        xmorph.Morph(rhinoMesh);

        var morphedMesh = _geometryConverter.ToAutoCadType(rhinoMesh);

        return new GH_AutocadMesh(morphedMesh);
    }

    /// <inheritdoc />
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
        if (this.RhinoGeometry == null)
            return;

        args.Pipeline.DrawMeshWires(this.RhinoGeometry, args.Color, args.Thickness);
    }

    /// <inheritdoc />
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
        args.Pipeline.DrawMeshShaded(this.RhinoGeometry, args.Material);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadMesh goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadMesh mesh)
        {
            this.Value = mesh;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadMesh)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadMesh)))
        {
            target = (Q)(object)new GH_AutocadMesh(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadMesh";

        return $"AutocadMesh [{this.Value.GetType().ToString()}]";
    }
}

