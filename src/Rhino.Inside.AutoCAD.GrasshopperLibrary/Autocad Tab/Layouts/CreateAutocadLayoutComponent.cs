using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates a new AutoCAD layout.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.3.2")]
public class CreateAutocadLayoutComponent : Layout_BaseComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("d6e8f0a3-7b9c-4d2e-8f3a-9c5b4e7d8a1f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateAutocadLayoutComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAutocadLayoutComponent"/> class.
    /// </summary>
    public CreateAutocadLayoutComponent()
        : base("Create AutoCAD Layout", "AC-AddLay",
            "Creates a new AutoCAD Layout",
            "AutoCAD", "Layouts")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddTextParameter("Name", "Name",
            "The name of the new AutoCAD Layout", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayout(GH_ParamAccess.item), "Layout", "Layout",
            "The created AutoCAD Layout", GH_ParamAccess.item);

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

        if (!DA.GetData(1, ref name) || string.IsNullOrEmpty(name)) return;

        // Validation
        if (string.IsNullOrWhiteSpace(name))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Layout name cannot be empty");
            return;
        }

        var transactionManager = autocadDocument.CreateTransactionManager();

        var layout = transactionManager.PerformTask(() =>
        {
            if (this.TryGetByName(transactionManager, name, out var existing))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "A Layout with this name already exists");

                return existing;
            }

            return AutocadLayoutWrapper.Create(autocadDocument, name);
        });

        var layoutGoo = new GH_AutocadLayout(layout);

        DA.SetData(0, layoutGoo);
        DA.SetData(1, layout.Name);
        DA.SetData(2, layout.Id);
        DA.SetData(3, layout.TabOrder);
        DA.SetData(4, layout.BlockTableRecordId);
    }
}
