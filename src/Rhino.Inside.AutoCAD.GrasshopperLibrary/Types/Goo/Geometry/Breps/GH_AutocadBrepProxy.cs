using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoBrep = Rhino.Geometry.Brep;
using RhinoMesh = Rhino.Geometry.Mesh;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for Breps using a <see cref="RhinoBrep"/> and
/// <see cref="AutocadBrepProxy"/> a sudo Brep created from AutoCAD Nurbs Surfaces.
/// We cannot directly create Autocad  <see cref="Solid3d"/> objects from Breps
/// synchronously so this proxy object is used instead. Internally (the Value)
/// this Goo is a RhinoBrep, when baking to Autocad the <see cref="IBrepConverterRunner"/>
/// is used to convert the Brep to true Autocad <see cref="Solid3d"/> objects asynchronously.
/// However, for synchronous conversions a proxy object is used instead, This proxy is
/// called <see cref="AutocadBrepProxy "/> and it represents the Brep as a collection of
/// Autocad Nurbs Surfaces and can be used to represent the Brep in Autocad.
/// </summary>
public class GH_AutocadBrepProxy : GH_GeometricGoo<RhinoBrep>, IGH_AutocadReferenceDatabaseObject,
    IGH_PreviewData, IGH_AutocadGeometryPreview, IAutocadBakeable
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private const string _referenceHandleDictionaryName = "AutocadReferenceHandle";

    /// <summary>
    /// Creates a new <see cref="GH_AutocadBrepProxy"/> instance from the specified
    /// Autocad <see cref="Solid3d"/>.
    /// </summary>
    public static GH_AutocadBrepProxy CreateFromSolid(Solid3d solid)
    {
        var geometryConverter = GeometryConverter.Instance!;

        var rhinoBrep = geometryConverter.ToRhinoType(solid).FirstOrDefault();

        return rhinoBrep == null
            ? new GH_AutocadBrepProxy()
            : new GH_AutocadBrepProxy(rhinoBrep, new AutocadReferenceId(solid));
    }

    /// <inheritdoc />
    public IAutocadReferenceId Reference { get; private set; }

    /// <inheritdoc />
    public IDbObject ObjectValue => this.Convert(this.Value);

    /// <inheritdoc />
    public override BoundingBox Boundingbox => this.Value.GetBoundingBox(false);

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
    public override string TypeName => $"AutoCAD Proxy Solid";

    /// <inheritdoc />
    public override string TypeDescription => $"Represents an {this.TypeName}, This is an object which is comparable to a Brep in Rhino and can be converted into a Solid3d in Autocad";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBrepProxy"/> class with no
    /// value.
    /// </summary>
    public GH_AutocadBrepProxy()
    {
        this.Reference = AutocadReferenceId.NoReference;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBrepProxy"/> class with the
    /// specified <see cref="AutocadBrepProxy"/>. Internally, the geometry is cloned,
    /// but the autocad reference ID is maintained.
    /// </summary>
    public GH_AutocadBrepProxy(RhinoBrep brep) : base(brep.DuplicateBrep())
    {
        this.Reference = AutocadReferenceId.NoReference;

    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input curve.
    /// </summary>
    private GH_AutocadBrepProxy(RhinoBrep brep, IAutocadReferenceId referenceId) : base(brep)
    {
        this.Reference = referenceId;
    }

    /// <summary>
    /// Creates a new <see cref="IGH_Goo"/> instance wrapping the specified
    /// </summary>
    private RhinoBrep? Convert(Solid3d solid3d)
    {
        var rhinoType = _geometryConverter.ToRhinoType(solid3d);

        return rhinoType.FirstOrDefault();
    }

    /// <summary>
    /// Creates a new <see cref="IGH_Goo"/> instance wrapping the specified
    /// </summary>
    private AutocadBrepProxy? Convert(RhinoBrep rhinoBrep)
    {
        var rhinoType = _geometryConverter.ToProxyType(rhinoBrep);

        return rhinoType;
    }

    /// <inheritdoc />
    public void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var rhinoGeometry = this.Value;

        if (rhinoGeometry == null) return;

        var meshes = RhinoMesh.CreateFromBrep(rhinoGeometry, MeshingParameters.Default);

        previewData.Meshes.AddRange(meshes);

        var polylines = rhinoGeometry.Curves3D;

        foreach (var polyline in polylines)
        {
            previewData.Wires.Add(polyline);
        }
    }

    /// <inheritdoc />
    /// For true brep baking, we need to convert the brep to AutoCAD solids asynchronously.
    public List<IObjectId> BakeToAutocad(ITransactionManager transactionManager, IBakingComponent bakingComponent, IBakeSettings? settings = null)
    {
        var ids = new List<IObjectId>();

        var rhinoGeometry = this.Value;

        if (rhinoGeometry == null)
            return ids;

        var activeDocument = Application.DocumentManager.MdiActiveDocument;
        IEntityCollection convertedResult = null;

        var request = new BrepConverterRequest(rhinoGeometry, (result) =>
        {
            convertedResult = result.ConvertedSolids;

            if (result.Success == false)
            {
                bakingComponent.AddWarningMessage("Failed to convert a brep to AutoCAD format");
                return false;
            }

            var goo = convertedResult.Select(cadSolid => new GH_AutocadObjectId(cadSolid.Id));

            bakingComponent.AppendDataList(goo);

            return true;
        });

        var application = RhinoInsideAutoCadExtension.Application;

        var brepConverterRunner = application.BrepConverterRunner;

        brepConverterRunner.EnqueueRequest(request);

        activeDocument.SendStringToExecute("RHINO_INSIDE_CONVERT_BREP\n", false, false, false);

        return ids;
    }

    /// <summary>
    /// News up a new <see cref="IGH_Goo"/> instance wrapping the specified <see
    /// cref="IDbObject"/>. The internal entity should be cloned, but the reference ID
    /// must be preserved.
    /// </summary>
    private GH_AutocadBrepProxy CreateClonedInstance(RhinoBrep brep)
    {
        return new GH_AutocadBrepProxy(brep.DuplicateBrep(), this.Reference);
    }

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
        var geometry = this.Value;

        if (geometry == null)
            return this;

        var duplicate = geometry.DuplicateBrep();

        duplicate.Transform(xform);

        return new GH_AutocadBrepProxy(duplicate);
    }

    /// <inheritdoc />
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        var geometry = this.Value;

        if (geometry == null)
            return this;

        var duplicate = geometry.DuplicateBrep();

        xmorph.Morph(duplicate);

        return new GH_AutocadBrepProxy(duplicate);
    }

    /// <inheritdoc />
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
        if (this.Value == null)
            return;

        args.Pipeline.DrawBrepWires(this.Value, args.Color);
    }

    /// <inheritdoc />
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
        if (this.Value == null)
            return;

        args.Pipeline.DrawBrepShaded(this.Value, args.Material);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadBrepProxy goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is RhinoBrep brep)
        {
            this.Value = brep;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(RhinoBrep)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadBrepProxy)))
        {
            target = (Q)(object)new GH_AutocadBrepProxy(this.Value);
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

            this.Value = this.Convert(entity.Unwrap() as Solid3d);
        }
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

        if (referencedObject is Solid3d typeReferencedObject == false)
            return true;

        this.Value = this.Convert(typeReferencedObject!);

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

        return $"{this.TypeName} [Type: Solid3d, Id: {this.Reference} ]";
    }
}