using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLayout = Autodesk.AutoCAD.DatabaseServices.Layout;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layouts currently in the AutoCAD document.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.9")]
public class GetAutocadLayoutsComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("c5f6a8b9-2d3e-4f7a-9b1c-8e5d4a7f9c2e");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLayoutsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayoutsComponent"/> class.
    /// </summary>
    public GetAutocadLayoutsComponent()
        : base("Get AutoCAD Layouts", "AC-Lays",
            "Returns the list of all the AutoCAD layouts in the document",
            "AutoCAD", "Layouts")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddBooleanParameter("Repopulate", "Repop",
            "There are some modifications to layouts (like renaming outside of grasshopper) which do not correctly trigger the object" +
            " modification event in AutoCAD. If this is set to true, the internal cache of Layouts will be repopulated with" +
            " the latest information from AutoCAD prior to getting the layouts. This guarantees the latest information by is " +
            "potentially slow in complex definitions or files.", GH_ParamAccess.item, false);

        pManager[1].Optional = true;
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
        var repopulate = false;

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

        DA.GetData(1, ref repopulate);

        if (repopulate)
            autocadDocument.LayoutRepository.Repopulate();

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
