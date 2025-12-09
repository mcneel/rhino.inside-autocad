using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD documents currently open in the AutoCAD session.
/// </summary>
public class GetByAutocadIdComponent : GH_Component, IReferenceComponent
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("ab9f48ba-bef6-4646-a3ad-146a92678440");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLayersComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public GetByAutocadIdComponent()
        : base("GetById", "GetId",
            "Returns the the AutoCAD object which matches the id",
            "AutoCAD", "Document")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadId(GH_ParamAccess.item), "ObjectId",
            "Id", "An AutoCAD ObjectId", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGenericParameter("Object", "Obj", "An Autocad Object",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        AutocadObjectId? id = null;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;
        if (!DA.GetData(1, ref id)
            || id is null) return;

        var dbObject = autocadDocument.GetObjectById(id);

        var gooObject = new GH_AutocadObject(dbObject);

        DA.SetData(0, gooObject);
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
            if (changedObject.UnwrapObject() is CadLayer)
            {
                return true;
            }
        }

        return false;

    }
}
