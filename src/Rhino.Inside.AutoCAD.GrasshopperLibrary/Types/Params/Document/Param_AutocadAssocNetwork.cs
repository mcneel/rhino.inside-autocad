using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD AssocNetworks.
/// </summary>
public class Param_AutocadAssocNetwork : GH_Param<GH_AutocadAssocNetwork>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("7b0efc34-4e0d-45f7-adc8-1f3482c94a35");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadAssocNetwork;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadAssocNetwork"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadAssocNetwork(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadAssocNetwork"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadAssocNetwork(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadAssocNetwork"/> class.
    /// </summary>
    public Param_AutocadAssocNetwork(GH_ParamAccess access)
        : base("AutoCAD AssocNetwork", "AssocNet",
            "An AssocNetwork in AutoCAD", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var assocNetwork in m_data.AllData(true).OfType<GH_AutocadAssocNetwork>())
        {
            if (change.DoesEffectObject(assocNetwork.Value.Id))
                return true;
        }

        return false;
    }
}
