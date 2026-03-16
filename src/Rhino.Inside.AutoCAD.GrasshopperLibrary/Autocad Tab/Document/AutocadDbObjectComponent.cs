using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from an AutoCAD DBObject.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class AutocadDbObjectComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("710fa8cf-d48c-452b-a6fb-cc482a7e8c53");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadDbObjectComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadDbObjectComponent"/> class.
    /// </summary>
    public AutocadDbObjectComponent()
        : base("AutoCAD DBObject", "AC-DbObj",
            "Gets Information from an AutoCAD DBObject",
            "AutoCAD", "Document")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObject(GH_ParamAccess.item), "DBObject",
            "Obj", "An AutoCAD DBObject", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The ObjectId of the AutoCAD DBObject.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("Handle", "Handle",
            "The Handle value of the AutoCAD DBObject.", GH_ParamAccess.item);

        pManager.AddTextParameter("Type", "Type",
            "The Type name of the AutoCAD DBObject.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "ExtensionDictionaryId", "ExtDictId",
            "The ExtensionDictionary Id of the AutoCAD DBObject.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDbObjectWrapper? dbObject = null;

        if (!DA.GetData(0, ref dbObject) || dbObject is null) return;

        // Id
        var id = dbObject.Id;

        // Handle (as long)
        var handle = dbObject.Id.Value;

        // Type name
        var typeName = dbObject.Type.Name;

        // ExtensionDictionary
        var extDictId = new AutocadObjectIdWrapper(dbObject.AutocadObject.ExtensionDictionary);

        DA.SetData(0, id);
        DA.SetData(1, handle);
        DA.SetData(2, typeName);
        DA.SetData(3, extDictId);
    }
}
