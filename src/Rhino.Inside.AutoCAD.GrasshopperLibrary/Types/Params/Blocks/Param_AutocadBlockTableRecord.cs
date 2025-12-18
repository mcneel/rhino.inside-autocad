using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD block definitions.
/// </summary>
public class Param_AutocadBlockTableRecord : GH_Param<GH_AutocadBlockTableRecord>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("a3d7e5f9-4c8b-6d2e-9f1a-7e4b3c8d6a2f");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadBlockTableRecord;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadBlockTableRecord"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadBlockTableRecord(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadBlockTableRecord"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadBlockTableRecord(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadBlockTableRecord"/> class.
    /// </summary>
    public Param_AutocadBlockTableRecord(GH_ParamAccess access)
        : base("AutoCAD Block Definition", "BlockDef",
            "A Block Definition in AutoCAD", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var block in m_data.AllData(true).OfType<GH_AutocadBlockTableRecord>())
        {
            if (change.DoesEffectObject(block.Value.Id))
                return true;
        }

        return false;
    }
}
