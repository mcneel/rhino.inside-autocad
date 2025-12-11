using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
public class GetAutocadLayersComponent : GH_Component, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("41c4ed14-3a97-4812-94bc-4950bca8be7d");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLayersComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public GetAutocadLayersComponent()
        : base("Get AutoCAD Layers", "Lyrs",
            "Returns the list of all the AutoCAD layer in the document",
            "AutoCAD", "Layers")
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
        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.list), "Layers", "Layers", "The AutoCAD Layers",
            GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;

        var layersRepository = autocadDocument.LayerRepository;

        var gooLayers = layersRepository
            .Select(layer => new GH_AutocadLayer(layer))
            .ToList();

        DA.SetDataList(0, gooLayers);
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
            if (changedObject.UnwrapObject() is CadLayer)
            {
                return true;
            }
        }

        return false;

    }
}
