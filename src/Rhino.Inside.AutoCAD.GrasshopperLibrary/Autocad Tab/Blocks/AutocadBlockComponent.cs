using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
public class AutocadBlockComponent : GH_Component
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("ac5cdf28-ef75-4c47-9147-15c12a61ab80");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBlockComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadBlockComponent"/> class.
    /// </summary>
    public AutocadBlockComponent()
        : base("AutoCadLayer", "BlockTableRecord",
            "Gets Information from an AutoCAD Block Table Record",
            "AutoCAD", "Block")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadBlockTableRecord(GH_ParamAccess.item), "BlockTableRecord",
            "Block", "An AutoCAD Block Table Record", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddPointParameter("Origin", "Origin",
            "The origin point of the Block Table Record. Note this has been converted to the Rhino Units",
            GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadId(GH_ParamAccess.list), "ObjectIds", "ObjectIds",
            "The Ids of the object inside this Block Table Record", GH_ParamAccess.list);

    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        BlockTableRecordWrapper? blockTableRecordWrapper = null;

        if (!DA.GetData(0, ref blockTableRecordWrapper)
            || blockTableRecordWrapper is null) return;

        var origin = blockTableRecordWrapper.Origin;

        var name = blockTableRecordWrapper.Name;

        var id = blockTableRecordWrapper.Id;

        var objectIds = blockTableRecordWrapper.ObjectIds.Select(objectId => new GH_AutocadObjectId(objectId));



        DA.SetData(0, name);
        DA.SetData(1, id);
        DA.SetData(2, origin);
        DA.SetData(3, objectIds);
    }
}
