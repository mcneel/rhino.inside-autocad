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
public class GetAutocadBlockTableRecordsComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("feb2beb6-7414-43e5-941a-d50f26a57ab7");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon =>
        Properties.Resources.GetAutocadBlockTableRecordsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadBlockTableRecordsComponent"/> class.
    /// </summary>
    public GetAutocadBlockTableRecordsComponent()
        : base("Get AutoCAD Block Table Records", "AC-BlkRecs",
            "Returns the list of all the AutoCAD Bloc1k Table Records in the document",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadBlockTableRecord(GH_ParamAccess.list),
            "BlockTableRecords", "BlockTableRecords", "The AutoCAD BlockTableRecords",
            GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;

        var blockTableRecordsRepository = autocadDocument.BlockTableRecordRepository;

        var gooBlockTableRecords = blockTableRecordsRepository
            .Select(autocadLayerTableRecord =>
                new GH_AutocadBlockTableRecord(autocadLayerTableRecord))
            .ToList();

        DA.SetDataList(0, gooBlockTableRecords);
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