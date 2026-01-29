using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;


namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD documents currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class GetAutocadDocumentsComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("ddead218-1ce1-4e15-a89e-85010d6226c3");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadDocumentsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadDocumentsComponent"/> class.
    /// </summary>
    public GetAutocadDocumentsComponent()
        : base("Get AutoCAD Documents", "AC-Docs",
            "Returns the AutoCAD documents current open",
            "AutoCAD", "Document")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {

    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.list), "Documents", "Docs", "AutoCAD Documents",
            GH_ParamAccess.list);

        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Active Document", "ActiveDoc", "The currently active AutoCAD Document",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        var application = RhinoInsideAutoCadExtension.Application;

        var rhinoInsideManger = application.RhinoInsideManager;

        var autocadInstance = rhinoInsideManger.AutoCadInstance;

        var documents = autocadInstance.Documents;

        var activeDocument = autocadInstance.ActiveDocument;

        var gooDocuments = documents
            .Select(doc => new GH_AutocadDocument(doc));

        var gooActiveDocument = activeDocument == null
            ? new GH_AutocadDocument()
            : new GH_AutocadDocument(activeDocument);

        DA.SetDataList(0, gooDocuments);
        DA.SetData(1, gooActiveDocument);
    }
}
