using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that adds AutoCAD Block References to a document.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.16")]
public class CreateAutocadBlockReferenceComponent : RhinoInsideAutocad_ComponentBase
{

    /// <inheritdoc />
    public override Guid ComponentGuid => new("c7f3a2e8-9d4b-5c6f-8e1a-2b3c4d5e6f7a");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AddAutocadBlockReferenceComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAutocadBlockReferenceComponent"/> class.
    /// </summary>
    public CreateAutocadBlockReferenceComponent()
        : base("Create AutoCAD Block Reference", "AC-BlkRef",
            "Creates AutoCAD Block Reference(s) to a document at the specified insertion point(s). The Block is created on memory to add to the Autocad Document use the Autocad Bake Component",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddParameter(new Param_AutocadBlockTableRecord(GH_ParamAccess.item), "BlockDefinition",
            "BlockDef", "The Block Definition to insert", GH_ParamAccess.item);

        pManager.AddPointParameter("InsertionPoints", "Points",
            "The insertion point(s) for the Block Reference(s), as Rhino Points", GH_ParamAccess.list);

        pManager.AddNumberParameter("Rotation", "Rot",
            "The rotation angle in radians", GH_ParamAccess.item, 0.0);
        pManager[3].Optional = true;

        pManager.AddParameter(new Param_AutocadScale(GH_ParamAccess.item), "Scale", "Scale", "The Scale of the Block Reference. This will take either one uniform number or three numbers for a non uniform scale",
            GH_ParamAccess.item);
        pManager[4].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadBlockReference(),
            "BlockReferences", "Refs", "The created AutoCAD Block Reference(s)",
            GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        DA.GetData(0, ref autocadDocument);

        if (autocadDocument is null)
        {
            var activeDoc = RhinoInsideAutoCadExtension.Application?.RhinoInsideManager?.AutoCadInstance?.ActiveDocument;
            if (activeDoc is null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
                return;
            }
            autocadDocument = activeDoc as AutocadDocument;
        }

        if (autocadDocument is null)
            return;

        AutocadBlockTableRecordWrapper? blockTableRecord = null;
        if (!DA.GetData(1, ref blockTableRecord) || blockTableRecord is null) return;

        var insertionPoints = new List<Rhino.Geometry.Point3d>();
        if (!DA.GetDataList(2, insertionPoints) || insertionPoints.Count == 0) return;

        var rotation = 0.0;
        var scale = new AutocadScale(1);
        DA.GetData(3, ref rotation);
        DA.GetData(4, ref scale);

        var blockReferences = new List<GH_AutocadBlockReference>();

        using var documentLock = autocadDocument.Unwrap().LockDocument();

        autocadDocument.Transaction(_ =>
        {
            foreach (var rhinoPoint in insertionPoints)
            {

                var insertionPoint = rhinoPoint.ToAutocadPoint3d();

                var blockRef = new BlockReference(
                    insertionPoint,
                    blockTableRecord.Id.Unwrap());
                blockRef.Rotation = rotation;
                blockRef.ScaleFactors = new Scale3d(scale.X, scale.Y, scale.Z);

                blockReferences.Add(
                    new GH_AutocadBlockReference(new AutocadBlockReferenceWrapper(blockRef)));
            }

            return true;
        });

        DA.SetDataList(0, blockReferences);
    }
}