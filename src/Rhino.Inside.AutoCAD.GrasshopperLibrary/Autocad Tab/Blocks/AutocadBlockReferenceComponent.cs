using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD layers currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.11")]
public class AutocadBlockReferenceComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("ac5cdf28-ef75-4c47-9147-15c12a61ab80");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBlockReferenceComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadBlockReferenceComponent"/> class.
    /// </summary>
    public AutocadBlockReferenceComponent()
        : base("AutoCAD Block Reference", "AC-BlkRef",
            "Gets Information from an AutoCAD Block Table Record",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadBlockReference(), "BlockReference",
            "BlockReference", "An AutoCAD Block Reference", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Block Reference.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Block Reference.", GH_ParamAccess.item);

        pManager.AddPointParameter("Origin", "Origin",
            "The origin point of the Block Reference. Note this has been converted to the Rhino Units",
            GH_ParamAccess.item);

        pManager.AddNumberParameter("Rotation", "Rotation",
            "The Rotation of the Block Reference.",
            GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "BlockTableRecordId",
            "BlockTableRecordId", "The Object Id  of the AutoCAD Block Table Record that this BlockReference is a reference of.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_DynamicBlockReferenceProperty(GH_ParamAccess.list),
            "Properties", "P", "The Dynamic Block Reference Properties", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        BlockReferenceWrapper? blockReferenceWrapper = null;

        if (!DA.GetData(0, ref blockReferenceWrapper)
            || blockReferenceWrapper is null) return;

        var origin = _geometryConverter.ToRhinoType(blockReferenceWrapper.Unwrap().Position);

        var rotation = blockReferenceWrapper.Rotation;

        var name = blockReferenceWrapper.Name;

        var id = blockReferenceWrapper.Id;

        var properties = blockReferenceWrapper.DynamicProperties.Select(property =>
            new GH_DynamicBlockReferenceProperty(property));

        var blockTableRecordIdGoo =
            new GH_AutocadObjectId(blockReferenceWrapper.BlockTableRecordId);

        DA.SetData(0, name);
        DA.SetData(1, id);
        DA.SetData(2, origin);
        DA.SetData(3, rotation);
        DA.SetData(4, blockTableRecordIdGoo);
        DA.SetDataList(5, properties);
    }
}