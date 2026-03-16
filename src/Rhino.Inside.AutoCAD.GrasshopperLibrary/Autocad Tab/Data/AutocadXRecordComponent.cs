using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from an AutoCAD XRecord.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class AutocadXRecordComponent : RhinoInsideAutocad_ComponentBase
{
    private readonly GooConverter _gooConverter;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("e7a8b9c0-1d23-4e4f-c051-6f7a8b9c0d1e");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadXRecordComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadXRecordComponent"/> class.
    /// </summary>
    public AutocadXRecordComponent()
        : base("AutoCAD XRecord", "AC-XRec",
            "Gets Information from an AutoCAD XRecord",
            "AutoCAD", "Data")
    {
        _gooConverter = new GooConverter();
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadXRecord(GH_ParamAccess.item), "XRecord",
            "XRec", "An AutoCAD XRecord", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD XRecord.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("TypeCodes", "Types",
            "The DXF group codes for each value.", GH_ParamAccess.list);

        pManager.AddGenericParameter("Values", "Values",
            "The values stored in the XRecord.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        XRecordWrapper? xrecord = null;

        if (!DA.GetData(0, ref xrecord)
            || xrecord is null) return;

        var id = xrecord.Id;
        var data = xrecord.Data;

        var typeCodes = new List<int>();
        var values = new List<IGH_Goo>();

        foreach (var (typeCode, value) in data)
        {
            typeCodes.Add(typeCode);
            values.Add(_gooConverter.ConvertToGoo(value));
        }

        DA.SetData(0, id);
        DA.SetDataList(1, typeCodes);
        DA.SetDataList(2, values);
    }
}
