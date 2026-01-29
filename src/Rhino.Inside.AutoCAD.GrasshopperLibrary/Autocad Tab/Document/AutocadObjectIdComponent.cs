using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;

using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns information about an AutoCAD ObjectId.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.13")]
public class AutocadObjectIdComponent : RhinoInsideAutocad_ComponentBase
{

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("c4d6e8f9-5a3b-4c7d-9e2f-1a8b3c5d7e9f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadObjectIdComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadObjectIdComponent"/> class.
    /// </summary>
    public AutocadObjectIdComponent()
        : base("AutoCAD Object Id", "AC-ObjId",
            "Gets Information from an AutoCAD ObjectId",
            "AutoCAD", "Document")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "ObjectId",
            "Id", "An AutoCAD ObjectId", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddIntegerParameter("Value", "Value",
            "The value of the AutoCAD ObjectId.", GH_ParamAccess.item);

        pManager.AddBooleanParameter("Is Valid", "Valid",
            "Boolean value indicating if the AutoCAD ObjectId is valid",
            GH_ParamAccess.item);

        pManager.AddBooleanParameter("Is Erased", "Erased",
            "Boolean value indicating if the AutoCAD ObjectId is erased",
            GH_ParamAccess.item);

        pManager.AddIntegerParameter("Object Handle", "Handle",
            "The internal Handle which AutoCAD uses to store the object",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadObjectId? objectId = null;

        if (!DA.GetData(0, ref objectId)
            || objectId is null) return;

        var value = objectId.Value;
        var isValid = objectId.IsValid;
        var isErased = objectId.IsErased;

        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        using var documentLock = activeDocument.LockDocument();

        var database = activeDocument.Database;

        using var transaction = database.TransactionManager.StartTransaction();

        var cadObject =
            transaction.GetObject(objectId.Unwrap(), OpenMode.ForRead,
                false) as DBObject;

        var handle = cadObject.Handle.Value;

        transaction.Commit();

        DA.SetData(0, value);
        DA.SetData(1, isValid);
        DA.SetData(2, isErased);
        DA.SetData(3, handle);
    }
}
