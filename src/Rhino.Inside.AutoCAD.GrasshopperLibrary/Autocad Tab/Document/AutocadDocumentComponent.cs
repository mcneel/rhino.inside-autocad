using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD documents currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class AutocadDocumentComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("c3c7891a-8e29-42d0-8a23-79784877069c");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadDocumentComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadDocumentsComponent"/> class.
    /// </summary>
    public AutocadDocumentComponent()
        : base("AutoCAD Document", "AC-Doc",
            "Gets Information from an autocad Document",
            "AutoCAD", "Document")
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
        pManager.AddTextParameter("File Name", "Name",
            "The name of the AutoCAD file.", GH_ParamAccess.item);

        pManager.AddTextParameter("File Path", "Path",
            "The file path of the AutoCAD file.", GH_ParamAccess.item);

        pManager.AddBooleanParameter("Read Only", "ReadOnly",
            "Boolean value indicating if the AutoCAD file is read only.",
            GH_ParamAccess.item);

        pManager.AddBooleanParameter("Active", "Active",
            "Boolean value indicating if the AutoCAD file is the active document at the time the solution is being solved.",
            GH_ParamAccess.item);

    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;

        var filePath = autocadDocument.FileInfo.FilePath;
        var fileName = autocadDocument.FileInfo.FileName;
        var isActive = autocadDocument.FileInfo.IsActive;
        var isReadOnly = autocadDocument.FileInfo.IsReadOnly;

        DA.SetData(0, fileName);
        DA.SetData(1, filePath);
        DA.SetData(2, isReadOnly);
        DA.SetData(3, isActive);
    }
}
