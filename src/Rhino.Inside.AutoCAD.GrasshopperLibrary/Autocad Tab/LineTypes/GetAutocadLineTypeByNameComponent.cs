using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLineType = Autodesk.AutoCAD.DatabaseServices.LinetypeTableRecord;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD linetype which matches the name.
/// </summary>
public class GetAutocadLineTypeByNameComponent : GH_Component, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("f8a9d1e2-5b6c-7a0d-2e4f-1c8d7d0a2f5e");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLineTypesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLineTypeByNameComponent"/> class.
    /// </summary>
    public GetAutocadLineTypeByNameComponent()
        : base("GetAutoCadLineTypeByName", "GetLineType",
            "Returns the AutoCAD linetype which matches the name",
            "AutoCAD", "LineTypes")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);
        pManager.AddTextParameter("Name", "N", "The name of the AutoCAD LineType to retrieve", GH_ParamAccess.item, string.Empty);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLineType(GH_ParamAccess.item), "LineType", "LineType",
            "The AutoCAD LineType matching the name, or the default linetype if no matching linetype is found",
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

        var lineTypesRepository = autocadDocument.LineTypeRepository;

        if (lineTypesRepository.TryGetByName(name, out var lineType) == false)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                $"No line type exists with name: {name}");
            return;
        }

        var gooLineType = new GH_AutocadLineType(lineType);

        DA.SetData(0, gooLineType);
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
