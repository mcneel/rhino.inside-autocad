using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD block instances.
/// </summary>
public class Param_AutocadBlockReference : GH_Param<GH_AutocadBlockReference>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("b4e8f0a2-5d9c-7e3f-0b2a-8f5c4d7e9a3f");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadBlockReference;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadBlockReference"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadBlockReference(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadBlockReference"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadBlockReference(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadBlockReference"/> class.
    /// </summary>
    public Param_AutocadBlockReference(GH_ParamAccess access)
        : base("AutoCAD Block Instance", "BlockInst",
            "A Block Instance in AutoCAD", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var blockRef in m_data.AllData(true).OfType<GH_AutocadBlockReference>())
        {
            if (change.DoesAffectObject(blockRef.Value.Id))
                return true;
        }

        return false;
    }
}
