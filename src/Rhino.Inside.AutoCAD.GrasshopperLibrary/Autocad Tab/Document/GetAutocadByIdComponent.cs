using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;
using CadLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD documents currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.9")]
public class GetByAutocadIdComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("ab9f48ba-bef6-4646-a3ad-146a92678440");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadByIdComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public GetByAutocadIdComponent()
        : base("Get By Id", "AC-ById",
            "Returns the the AutoCAD object which matches the id",
            "AutoCAD", "Document")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "ObjectId",
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
