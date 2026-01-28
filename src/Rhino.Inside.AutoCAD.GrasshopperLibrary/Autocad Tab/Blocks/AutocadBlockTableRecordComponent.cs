using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class AutocadBlockTableRecordComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("b19eb942-ce7b-4bb8-a5e3-3596e7517172");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBlockTableRecordComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadBlockTableRecordComponent"/> class.
    /// </summary>
    public AutocadBlockTableRecordComponent()
        : base("AutoCAD Block Table Record", "AC-BlkRec",
            "Gets Information from an AutoCAD Block Table Record",
            "AutoCAD", "Blocks")
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
            "The name of the AutoCAD Block Table Record.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Block Table Record.", GH_ParamAccess.item);

        pManager.AddPointParameter("Origin", "Origin",
            "The origin point of the Block Table Record. Note this has been converted to the Rhino Units",
            GH_ParamAccess.item);

    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        BlockTableRecordWrapper? blockTableRecordWrapper = null;

        if (!DA.GetData(0, ref blockTableRecordWrapper)
            || blockTableRecordWrapper is null) return;

        var origin = _geometryConverter.ToRhinoType(blockTableRecordWrapper.Unwrap().Origin);

        var name = blockTableRecordWrapper.Name;

        var id = blockTableRecordWrapper.Id;

        DA.SetData(0, name);
        DA.SetData(1, id);
        DA.SetData(2, origin);
    }
}