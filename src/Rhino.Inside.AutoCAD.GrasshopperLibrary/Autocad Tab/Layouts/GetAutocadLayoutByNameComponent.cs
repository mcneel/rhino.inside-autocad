using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;
using CadLayout = Autodesk.AutoCAD.DatabaseServices.Layout;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layout which matches the name.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class GetAutocadLayoutByNameComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("d6e7b9c0-3e4f-5a8b-0c2d-9f6e5b8d0d3f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLayoutsComponent;

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
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);
        pManager.AddTextParameter("Name", "N", "The name of the AutoCAD Layout to retrieve", GH_ParamAccess.item, string.Empty);
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

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;
        DA.GetData(1, ref name);

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
