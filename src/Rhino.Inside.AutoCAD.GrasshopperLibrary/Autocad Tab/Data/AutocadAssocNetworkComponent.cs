using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from an AutoCAD AssocNetwork.
/// </summary>
[ComponentVersion(introduced: "1.0.17")]
public class AutocadAssocNetworkComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("ad4ebf13-aa79-4810-9607-74ca53564705");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadAssocNetworkComponent;

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.hidden;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadAssocNetworkComponent"/> class.
    /// </summary>
    public AutocadAssocNetworkComponent()
        : base("AutoCAD AssocNetwork2", "AC-AssocNet",
            "Gets Information from an AutoCAD AssocNetwork",
            "AutoCAD", "Data")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadAssocNetwork(GH_ParamAccess.item), "AssocNetwork",
            "AssocNet", "An AutoCAD AssocNetwork", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.list), "Actions", "Actions",
            "The ObjectIds of actions in the AssocNetwork.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AssocNetworkWrapper? assocNetwork = null;

        if (!DA.GetData(0, ref assocNetwork)
            || assocNetwork is null) return;

        var actions = assocNetwork.Actions
            .Select(a => new GH_AutocadObjectId(a))
            .ToList();

        DA.SetDataList(0, actions);
    }
}
