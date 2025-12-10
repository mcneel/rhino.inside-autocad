using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD ObjectIds.
/// </summary>
public class Param_AutocadId : GH_Param<GH_AutocadObjectId>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("a3d8f7c2-4b1e-4f9a-8c5d-2e7a9b3c6d4f");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadId;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadId"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadId(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadId"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadId(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadId"/> class.
    /// </summary>
    public Param_AutocadId(GH_ParamAccess access)
        : base("AutoCAD ObjectId", "Id",
            "An ObjectId in AutoCAD", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var autocadId in m_data.AllData(true).OfType<GH_AutocadObjectId>())
        {
            if (change.DoesAffectObject(autocadId.Value))
                return true;
        }

        return false;
    }
}
