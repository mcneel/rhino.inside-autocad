using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns information about an AutoCAD ObjectId.
/// </summary>
public class AutocadObjectIdComponent : GH_Component
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
        : base("AutoCAD Object Id", "ObjId",
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

        pManager.AddBooleanParameter("IsValid", "IsValid",
            "Boolean value indicating if the AutoCAD ObjectId is valid",
            GH_ParamAccess.item);

        pManager.AddBooleanParameter("IsErased", "IsErased",
            "Boolean value indicating if the AutoCAD ObjectId is erased",
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

        DA.SetData(0, value);
        DA.SetData(1, isValid);
        DA.SetData(2, isErased);
    }
}
