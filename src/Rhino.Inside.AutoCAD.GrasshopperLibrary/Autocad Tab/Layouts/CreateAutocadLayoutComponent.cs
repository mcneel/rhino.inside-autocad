using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates a new AutoCAD layout.
/// </summary>
public class CreateAutocadLayoutComponent : GH_Component
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("d6e8f0a3-7b9c-4d2e-8f3a-9c5b4e7d8a1f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateAutocadLayoutComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAutocadLayoutComponent"/> class.
    /// </summary>
    public CreateAutocadLayoutComponent()
        : base("CreateAutocadLayout", "CreateLayout",
            "Creates a new AutoCAD Layout",
            "AutoCAD", "Layouts")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "Name",
            "The name of the new AutoCAD Layout", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Layout", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Layout", GH_ParamAccess.item);

        pManager.AddIntegerParameter("TabOrder", "TabOrder",
            "The tab display order", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "BlockTableRecordId", "BlockId",
            "The associated block table record ID", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        var name = string.Empty;

        if (!DA.GetData(0, ref autocadDocument) || autocadDocument is null) return;
        if (!DA.GetData(1, ref name) || string.IsNullOrEmpty(name)) return;

        // Validation
        if (string.IsNullOrWhiteSpace(name))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Layout name cannot be empty");
            return;
        }

        if (!autocadDocument.LayoutRepository.TryAddLayout(name, out var layout) || layout is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Layout already exists or could not be created");
            // layout will be null if it already exists, per the repository implementation
            return;
        }

        DA.SetData(0, layout.Name);
        DA.SetData(1, layout.Id);
        DA.SetData(2, layout.TabOrder);
        DA.SetData(3, layout.BlockTableRecordId);
    }
}
