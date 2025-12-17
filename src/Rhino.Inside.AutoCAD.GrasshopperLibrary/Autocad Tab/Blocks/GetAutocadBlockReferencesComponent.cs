using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD BlockTableRecords currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class GetAutocadBlockReferencesComponent : RhinoInsideAutocad_Component, IReferenceComponent
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("fd15f4a5-37be-4376-a934-80ba6695a88b");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadBlockReferencesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadBlockReferencesComponent"/> class.
    /// </summary>
    public GetAutocadBlockReferencesComponent()
        : base("Get AutoCAD Block References", "AC-BlkRefs",
            "Returns the list of all the AutoCAD Block References in the document of a given Block Table Record",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "BlockTableRecordId",
            "RecordId", "The Block Table Record to get the References of", GH_ParamAccess.item);

        pManager.AddBooleanParameter("DirectOnly", "DirectOnly",
            "If true the search will only look at for Block Reference which are not nested, if false then all nested blocks will be included.",
            GH_ParamAccess.item, false);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadBlockReference(),
            "BlockReferences", "References", "The AutoCAD BlockReferences",
            GH_ParamAccess.list);

        pManager.AddParameter(new Param_AutocadBlockReference(),
            "AnonymousBlockReferences", "AnonReferences", "The AutoCAD Anonymous BlockReferences",
            GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        AutocadObjectId? blockTableRecordId = null;
        var directOnly = false;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;

        if (!DA.GetData(1, ref blockTableRecordId)
            || blockTableRecordId is null) return;
        DA.GetData(2, ref directOnly);

        var blockTableRecordsRepository = autocadDocument.BlockTableRecordRepository;

        if (blockTableRecordsRepository.TryGetById(blockTableRecordId, out var blockTableRecord) == false)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Block Table Found with that Id");
            return;
        }

        var cadBlock = blockTableRecord.Unwrap();

        var normalBlocks = cadBlock.GetBlockReferenceIds(directOnly, false);
        var dynamicBlocks = cadBlock.GetAnonymousBlockIds();

        var normalReferences = new List<GH_AutocadBlockReference>();
        var dynamicReferences = new List<GH_AutocadBlockReference>();

        _ = autocadDocument.Transaction(transactionManagerWrapper =>
        {
            var transaction = transactionManagerWrapper.Unwrap();

            foreach (ObjectId normalId in normalBlocks)
            {
                var block =
                    transaction.GetObject(normalId, OpenMode.ForRead) as BlockReference;

                if (block is not null)
                    normalReferences.Add(
                        new GH_AutocadBlockReference(new BlockReferenceWrapper(block)));
            }

            foreach (ObjectId dynamicId in dynamicBlocks)
            {
                var block =
                    transaction.GetObject(dynamicId, OpenMode.ForRead) as BlockReference;

                if (block is not null)
                    dynamicReferences.Add(
                        new GH_AutocadBlockReference(new BlockReferenceWrapper(block)));
            }

            return true;
        });

        DA.SetDataList(0, normalReferences);
        DA.SetDataList(0, dynamicReferences);
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var ghParam in this.Params.Output.OfType<IReferenceParam>())
        {
            if (ghParam.NeedsToBeExpired(change)) return true;
        }

        foreach (var changedObject in change)
        {
            if (changedObject.UnwrapObject() is BlockTableRecord)
            {
                return true;
            }
        }

        return false;

    }
}
