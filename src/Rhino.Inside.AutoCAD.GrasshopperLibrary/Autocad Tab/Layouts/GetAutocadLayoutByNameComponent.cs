using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLayout = Autodesk.AutoCAD.DatabaseServices.Layout;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layout which matches the name.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.9")]
public class GetAutocadLayoutByNameComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("d6e7b9c0-3e4f-5a8b-0c2d-9f6e5b8d0d3f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLayoutsByNameComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayoutByNameComponent"/> class.
    /// </summary>
    public GetAutocadLayoutByNameComponent()
        : base("Get AutoCAD Layout By Name", "AC-Lay",
            "Returns the AutoCAD layout which matches the name",
            "AutoCAD", "Layouts")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddTextParameter("Name", "N", "The name of the AutoCAD Layout to retrieve", GH_ParamAccess.item, string.Empty);

        pManager.AddBooleanParameter("Repopulate", "Repop",
            "There are some modifications to layouts (like renaming outside of grasshopper) which do not correctly trigger the object" +
            " modification event in AutoCAD. If this is set to true, the internal cache of Layouts will be repopulated with" +
            " the latest information from AutoCAD prior to getting the layouts. This guarantees the latest information by is " +
            "potentially slow in complex definitions or files.", GH_ParamAccess.item, false);

        pManager[2].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayout(GH_ParamAccess.item), "Layout", "Layout",
            "The AutoCAD Layout matching the name, or the default layout if no matching layout is found",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        var name = string.Empty;
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

        DA.GetData(1, ref name);
        DA.GetData(2, ref repopulate);

        if (repopulate)
            autocadDocument.LayoutRepository.Repopulate();

        var layoutsRepository = autocadDocument.LayoutRepository;

        if (layoutsRepository.TryGetByName(name, out var layout) == false)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                $"No layout exists with name: {name}");
            return;
        }

        var gooLayout = new GH_AutocadLayout(layout);

        DA.SetData(0, gooLayout);
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
