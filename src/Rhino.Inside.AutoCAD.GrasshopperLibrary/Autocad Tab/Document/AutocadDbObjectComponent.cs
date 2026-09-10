using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from an AutoCAD DBObject.
/// </summary>
[ComponentVersion(introduced: "1.0.16", updated: "1.3.2")]
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

        // Entity-specific outputs (null if DBObject is not an Entity)
        pManager.AddTextParameter("LayerName", "Layer",
            "The layer name of the Entity. Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "LayerId", "LayerId",
            "The layer ObjectId of the Entity. Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadColor(GH_ParamAccess.item), "Color", "Col",
            "The AutoCAD color of the Entity (supports ByLayer/ByBlock). Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddTextParameter("MaterialName", "Mat",
            "The material name of the Entity. Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "MaterialId", "MatId",
            "The material ObjectId of the Entity. Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddTextParameter("LinetypeName", "LT",
            "The linetype name of the Entity. Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "LinetypeId", "LTId",
            "The linetype ObjectId of the Entity. Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("LineWeight", "LW",
            "The lineweight of the Entity in 1/100mm. -1=ByLayer, -2=ByBlock, -3=Default. Null if not an Entity.", GH_ParamAccess.item);

        pManager.AddNumberParameter("LinetypeScale", "LTS",
            "The linetype scale of the Entity. Null if not an Entity.", GH_ParamAccess.item);
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

        // Check if DBObject is an Entity and extract entity-specific properties
        if (dbObject.AutocadObject is Entity entity)
        {
            // Layer
            DA.SetData(4, entity.Layer);
            DA.SetData(5, new AutocadObjectIdWrapper(entity.LayerId));

            // Color - output AutoCAD color directly (preserves ByLayer/ByBlock)
            var colorWrapper = new AutocadColorWrapper(entity);
            DA.SetData(6, new GH_AutocadColor(colorWrapper));

            // Material
            DA.SetData(7, entity.Material);
            DA.SetData(8, new AutocadObjectIdWrapper(entity.MaterialId));

            // Linetype
            DA.SetData(9, entity.Linetype);
            DA.SetData(10, new AutocadObjectIdWrapper(entity.LinetypeId));

            // LineWeight (cast enum to int)
            DA.SetData(11, (int)entity.LineWeight);

            // LinetypeScale
            DA.SetData(12, entity.LinetypeScale);
        }
        // If not Entity, outputs 4-12 remain null (default behavior)
    }
}
