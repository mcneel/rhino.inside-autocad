using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD ObjectIds.
/// </summary>
public abstract class GH_AutocadGeometricGoo<TWrapperType, TRhinoType>
    : GH_GeometricGoo<TWrapperType>, IGH_AutocadReferenceObject,
        IGH_PreviewData, IGH_AutocadGeometryPreview, IAutocadBakeable
where TWrapperType : Entity
where TRhinoType : GeometryBase
{
    /// <summary>
    /// The geometry converter instance.s
    /// </summary>
    protected readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private readonly AutocadColorConverter _colorConverter = AutocadColorConverter.Instance!;

    /// <inheritdoc />
    public IObjectId AutocadReferenceId { get; }

    /// <inheritdoc />
    public IDbObject ObjectValue => new AutocadEntityWrapper(this.Value);

    /// <summary>
    /// Gets the Rhino geometry equivalent of the AutoCAD curve.
    /// </summary>
    public TRhinoType? RhinoGeometry =>
        this.Value == null
            ? null
            : this.Convert(this.Value);

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
                return $"No internal {this.TypeName} data";
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => $"AutoCAD {typeof(TWrapperType).Name}";

    /// <inheritdoc />
    public override string TypeDescription => $"Represents an {this.TypeName}";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with no value.
    /// </summary>
    protected GH_AutocadGeometricGoo()
    {
        this.AutocadReferenceId = new AutocadObjectId();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    protected GH_AutocadGeometricGoo(TWrapperType dbObject) : base(dbObject.Clone() as TWrapperType)
    {
        this.AutocadReferenceId = new AutocadObjectId(dbObject.Id);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    /// <param name="referenceId">The AutoCAD ObjectId to bind to this reference.</param>
    protected GH_AutocadGeometricGoo(TWrapperType dbObject, IObjectId referenceId) : base(dbObject)
    {
        this.AutocadReferenceId = referenceId;
    }

    /// <summary>
    /// News up a new <see cref="IGH_Goo"/> instance wrapping the specified <see
    /// cref="IDbObject"/>. The internal entity should be cloned, but the reference ID
    /// must be preserved.
    /// </summary>
    protected abstract GH_AutocadGeometricGoo<TWrapperType, TRhinoType> CreateClonedInstance(TWrapperType entity);

    /// <summary>
    /// News up a new <see cref="IGH_Goo"/> instance wrapping the specified <see
    /// cref="IDbObject"/>. The internal entity is cloned and the reference ID is reset.
    /// </summary>
    protected abstract GH_AutocadGeometricGoo<TWrapperType, TRhinoType> CreateInstance(TWrapperType entity);

    /// <summary>
    /// Creates a new <see cref="IGH_Goo"/> instance wrapping the specified
    /// </summary>
    protected abstract TWrapperType? Convert(TRhinoType rhinoType);

    /// <summary>
    /// Creates a new <see cref="IGH_Goo"/> instance wrapping the specified
    /// </summary>
    protected abstract TRhinoType? Convert(TWrapperType wrapperType);

    /// <summary>
    /// Draws the geometry in the Rhino viewport for wires views.
    /// </summary>
    protected abstract void DrawViewportGeometryWires(GH_PreviewWireArgs args);

    /// <summary>
    /// Draws the geometry in the Rhino viewport for meshes views.
    /// </summary>
    protected abstract void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args);

    /// <inheritdoc />
    public abstract void DrawAutocadPreview(IGrasshopperPreviewData previewData);

    /// <inheritdoc />
    public override IGH_Goo Duplicate() => (IGH_Goo)this.CreateClonedInstance(this.Value);

    /// <inheritdoc />
    public override IGH_GeometricGoo DuplicateGeometry() => (IGH_GeometricGoo)this.CreateClonedInstance(this.Value);

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

        var transformed = this.Convert(rhinoCurve);

        return this.CreateInstance(transformed);
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        if (this.RhinoGeometry == null)
            return this;

        var rhinoCurve = this.RhinoGeometry;

        xmorph.Morph(rhinoCurve);

        var morphed = this.Convert(rhinoCurve);

        return this.CreateInstance(morphed);
    }

    /// <inheritdoc />
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
        if (this.RhinoGeometry == null)
            return;

        this.DrawViewportGeometryWires(args);
    }

    /// <inheritdoc />
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
        if (this.RhinoGeometry == null)
            return;

        this.DrawViewportGeometryMeshes(args);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadGeometricGoo<TWrapperType, TRhinoType> goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is TWrapperType curve)
        {
            this.Value = curve;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(TWrapperType)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadGeometricGoo<TWrapperType, TRhinoType>)))
        {
            target = (Q)(object)this.CreateInstance(this.Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public void GetUpdatedObject()
    {
        var picker = new AutocadObjectPicker();
        if (picker.TryGetUpdatedObject(this.AutocadReferenceId, out var entity))
        {
            this.Value = (TWrapperType?)entity.Unwrap();
        }
    }

    /// <summary>
    /// Applies the given settings to the block reference.
    /// </summary>
    private void ApplySettings(IBakeSettings? settings, Entity entity)
    {
        if (settings is null) return;

        if (settings.Layer != null)
            entity.LayerId = settings.Layer.Id.Unwrap();

        if (settings?.LineType != null)
            entity.LinetypeId = settings.LineType.Id.Unwrap();

        if (settings?.Color != null)
        {
            var color = settings.Color;
            entity.Color = _colorConverter.ToCadColor(color);
        }
    }

    /// <inheritdoc />
    public IObjectId BakeToAutocad(ITransactionManager transactionManager, IBakeSettings? settings = null)
    {
        if (this.Value == null)
            throw new InvalidOperationException("Cannot bake a null block reference");

        var transaction = transactionManager.Unwrap();

        var modelSpace = transactionManager.GetModelSpaceBlockTableRecord(openForWrite: true);

        var modelSpaceRecord = modelSpace.Unwrap();

        var source = this.Value;

        var blockReference = (Entity)source.Clone();

        this.ApplySettings(settings, blockReference);

        var objectId = modelSpaceRecord.AppendEntity(blockReference);

        transaction.AddNewlyCreatedDBObject(blockReference, true);

        return new AutocadObjectId(objectId);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return $"Null {this.TypeName}";

        var idSuffix = this.AutocadReferenceId.IsValid
            ? this.AutocadReferenceId.Value.ToString()
            : "Invalid or Not in Database";

        return $"{this.TypeName} [Type: {this.Value.GetType().Name.ToString()}, Id: {idSuffix} ]";
    }
}