using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD XRecords.
/// </summary>
public class Param_AutocadXRecord : GH_Param<GH_AutocadXRecord>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("b9d4f2e8-6e03-4c7b-af95-8d204c3e6b09");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadXRecord;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadXRecord"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadXRecord(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadXRecord"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadXRecord(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadXRecord"/> class.
    /// </summary>
    public Param_AutocadXRecord(GH_ParamAccess access)
        : base("AutoCAD XRecord", "XRec",
            "An XRecord in AutoCAD", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var xrecord in m_data.AllData(true).OfType<GH_AutocadXRecord>())
        {
            if (change.DoesEffectObject(xrecord.Value.Id))
                return true;
        }

        return false;
    }
}
