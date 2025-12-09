using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLayout = Autodesk.AutoCAD.DatabaseServices.Layout;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layouts currently in the AutoCAD document.
/// </summary>
public class GetAutocadLayoutsComponent : GH_Component, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("c5f6a8b9-2d3e-4f7a-9b1c-8e5d4a7f9c2e");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLayoutsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayoutsComponent"/> class.
    /// </summary>
    public GetAutocadLayoutsComponent()
        : base("GetAutoCadLayouts", "GetLayouts",
            "Returns the list of all the AutoCAD layouts in the document",
            "AutoCAD", "Layouts")
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
        pManager.AddParameter(new Param_AutocadLayout(GH_ParamAccess.list), "Layouts", "Layouts", "The AutoCAD Layouts",
            GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;

        var layoutsRepository = autocadDocument.LayoutRepository;

        var gooLayouts = layoutsRepository
            .Select(layout => new GH_AutocadLayout(layout))
            .ToList();

        DA.SetDataList(0, gooLayouts);
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
            if (changedObject.UnwrapObject() is CadLayout)
            {
                return true;
            }
        }

        return false;

    }
}
