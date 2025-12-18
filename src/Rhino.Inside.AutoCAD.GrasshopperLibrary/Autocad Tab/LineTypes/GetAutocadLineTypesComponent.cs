using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;
using CadLineType = Autodesk.AutoCAD.DatabaseServices.LinetypeTableRecord;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD linetypes currently in the AutoCAD document.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class GetAutocadLineTypesComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("e7a8c0d1-4f5b-6a9c-1d3e-0d7f6c9a1e4f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLineTypesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLineTypesComponent"/> class.
    /// </summary>
    public GetAutocadLineTypesComponent()
        : base("Get AutoCAD Line Types", "AC-LTypes",
            "Returns the list of all the AutoCAD linetypes in the document",
            "AutoCAD", "LineTypes")
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
        pManager.AddParameter(new Param_AutocadLineType(GH_ParamAccess.list), "LineTypes", "LineTypes", "The AutoCAD LineTypes",
            GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;

        var lineTypesRepository = autocadDocument.LineTypeRepository;

        var gooLineTypes = lineTypesRepository
            .Select(lineType => new GH_AutocadLineType(lineType))
            .ToList();

        DA.SetDataList(0, gooLineTypes);
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
            if (changedObject.UnwrapObject() is CadLineType)
            {
                return true;
            }
        }

        return false;

    }
}
