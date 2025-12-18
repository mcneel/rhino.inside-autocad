using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using GH_IO.Serialization;
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
    : GH_GeometricGoo<TWrapperType>, IGH_AutocadReferenceDatabaseObject,
        IGH_PreviewData, IGH_AutocadGeometryPreview, IAutocadBakeable
where TWrapperType : Entity
where TRhinoType : GeometryBase
{
    protected readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private readonly AutocadColorConverter _colorConverter = AutocadColorConverter.Instance!;
    private const string _referenceHandleDictionaryName = "AutocadReferenceHandle";

    /// <inheritdoc />
    public IAutocadReferenceId Reference { get; private set; }

    /// <inheritdoc />
    public IDbObject ObjectValue => new AutocadEntityWrapper(this.Value);

    /// <summary>
    /// Gets the Rhino geometry equivalent of the AutoCAD geometry.
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
        this.Reference = AutocadReferenceId.NoReference;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    protected GH_AutocadGeometricGoo(TWrapperType? dbObject) : base(dbObject?.Clone() as TWrapperType)
    {
        this.Reference = dbObject is not null ? new AutocadReferenceId(dbObject) : AutocadReferenceId.NoReference;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadObject"/> class with the
    /// specified AutoCAD Object.
    /// </summary>
    /// <param name="dbObject">The AutoCAD Object to wrap.</param>
    /// <param name="referenceId">The AutoCAD ObjectId to bind to this reference.</param>
    protected GH_AutocadGeometricGoo(TWrapperType dbObject, IAutocadReferenceId referenceId) : base(dbObject)
    {
        this.Reference = referenceId;
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
        var rhinoGeometry = this.RhinoGeometry;

        if (rhinoGeometry == null)
            return this;

        rhinoGeometry.Transform(xform);

        var transformed = this.Convert(rhinoGeometry);

        return this.CreateInstance(transformed);
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        var rhinoGeometry = this.RhinoGeometry;

        if (rhinoGeometry == null)
            return this;

        xmorph.Morph(rhinoGeometry);

        var morphed = this.Convert(rhinoGeometry);

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

        if (source is TWrapperType wrapperType)
        {
            this.Value = wrapperType;
            return true;
        }

        if (source is TRhinoType rhinoGoo)
        {
            this.Value = this.Convert(rhinoGoo)!;
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
        if (picker.TryGetUpdatedObject(this.Reference.ObjectId, out var entity))
        {
            this.Value = (TWrapperType?)entity.Unwrap();
        }
    }

    /// <summary>
    /// Applies the given settings to the block reference.
    /// </summary>
    protected void ApplySettings(IBakeSettings? settings, Entity entity)
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
    public virtual List<IObjectId> BakeToAutocad(ITransactionManager transactionManager, IBakingComponent bakingComponent, IBakeSettings? settings = null)
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

        return [new AutocadObjectId(objectId)];
    }

    /// <inheritdoc />
    public override bool Read(GH_IReader reader)
    {
        var referenceHandle = string.Empty;

        reader.TryGetString(_referenceHandleDictionaryName, ref referenceHandle);

        if (string.IsNullOrEmpty(referenceHandle))
            return true;

        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var database = activeDocument.Database;

        var handle = new Handle(System.Convert.ToInt64(referenceHandle, 16));

        var transaction = database.TransactionManager.StartTransaction();

        var newId = database.GetObjectId(false, handle, 0);

        if (newId.IsValid == false) return true;

        var referencedObject = transaction.GetObject(newId, OpenMode.ForRead);

        if (referencedObject is TWrapperType typeReferencedObject == false)
            return true;

        this.Value = typeReferencedObject;

        this.Reference = new AutocadReferenceId(typeReferencedObject);

        transaction.Commit();

        return true;
    }

    /// <inheritdoc />
    public override bool Write(GH_IWriter writer)
    {
        if (this.Reference.IsValid && this.Value is not null)
            writer.SetString(_referenceHandleDictionaryName, this.Reference.GetSerializedValue());

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return $"Null {this.TypeName}";

        return $"{this.TypeName} [Type: {this.Value.GetType().Name.ToString()}, Id: {this.Reference} ]";
    }
}