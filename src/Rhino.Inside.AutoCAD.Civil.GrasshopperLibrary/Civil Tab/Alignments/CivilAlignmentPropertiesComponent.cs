using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts individual values from Civil 3D Alignment Properties.
/// </summary>
[ComponentVersion(introduced: "1.1.19")]
public class CivilAlignmentPropertiesComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("F6A7B8C9-D0E1-2345-F012-567890123DEF");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CivilAlignmentPropertiesComponent;

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <summary>
    /// Initializes a new instance of the <see cref="CivilAlignmentPropertiesComponent"/> class.
    /// </summary>
    public CivilAlignmentPropertiesComponent()
        : base("Civil3d Alignment Properties", "CVL-AlignProps",
            "Extracts individual values from Civil 3D Alignment Properties",
            "Civil3d", "Alignments")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilAlignmentProperties(GH_ParamAccess.item), "Properties",
            "Props", "Alignment properties from a Civil3d Alignment", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "N",
            "The name of the alignment. When set this will update the name of the alignment.", GH_ParamAccess.item);
        pManager[1].Optional = true;

        pManager.AddTextParameter("Description", "Desc",
            "The description of the alignment. When set this will update the description of the alignment", GH_ParamAccess.item);
        pManager[2].Optional = true;

    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "N",
            "The name of the alignment.", GH_ParamAccess.item);

        pManager.AddTextParameter("Description", "Desc",
            "The description of the alignment.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Start Station", "StaSt",
            "The starting station of the alignment.", GH_ParamAccess.item);

        pManager.AddNumberParameter("End Station", "StaEnd",
            "The ending station of the alignment.", GH_ParamAccess.item);

        pManager.AddNumberParameter("Length", "Len",
            "The total length of the alignment.", GH_ParamAccess.item);

        pManager.AddTextParameter("Alignment Type", "Type",
            "The type of alignment (Centerline, Offset, CurbReturn, etc.).", GH_ParamAccess.item);

        pManager.AddParameter(new Param_NamedId(GH_ParamAccess.item), "Site",
            "Site", "The site containing this alignment as a NamedId.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_NamedId(GH_ParamAccess.item), "Style",
            "Style", "The style applied to this alignment as a NamedId.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_NamedId(GH_ParamAccess.item), "Design Check Set",
            "DCS", "The design check set applied to this alignment as a NamedId.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilReferenceStation(GH_ParamAccess.item), "Reference Station",
            "RefSta", "The reference station information.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilDesignSpeeds(GH_ParamAccess.item), "Design Speeds",
            "DesSpd", "The design speed information.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilCANTInfo(GH_ParamAccess.item), "CANT Info",
            "CANT", "The CANT (superelevation) information.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilConnectedAlignmentInfo(GH_ParamAccess.item), "Connected Alignment Info",
            "ConnInfo", "The connected alignment information.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilOffsetAlignmentInfo(GH_ParamAccess.item), "Offset Alignment Info",
            "OfsInfo", "The offset alignment information.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_CivilRailAlignmentInfo(GH_ParamAccess.item), "Rail Alignment Info",
            "RailInfo", "The rail alignment information.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {

        GH_CivilAlignmentProperties? propsGoo = null;

        if (!DA.GetData(0, ref propsGoo) || propsGoo?.Value is null) return;

        ICivilAlignmentProperties alignmentProperties = propsGoo.Value;

        var newName = alignmentProperties.Name;
        var newDescription = alignmentProperties.Description;

        var updateFlag = false;

        if (DA.GetData(1, ref newName) && newName != alignmentProperties.Name) updateFlag = true;
        if (DA.GetData(2, ref newDescription) && newDescription != alignmentProperties.Description) updateFlag = true;

        var document = this.GetDocumentForObjectId(alignmentProperties.AlignmentId);
        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
            return;
        }

        var transactionManager = document.CreateTransactionManager();

        if (updateFlag)
        {
            alignmentProperties = transactionManager.PerformTask(() =>
                alignmentProperties.Update(transactionManager, newName, newDescription));
        }

        var cantInfo = transactionManager.PerformTask(() => alignmentProperties.GetCantInfo(transactionManager), true);

        // Basic properties
        DA.SetData(0, alignmentProperties.Name);
        DA.SetData(1, alignmentProperties.Description);
        DA.SetData(2, alignmentProperties.StartStation);
        DA.SetData(3, alignmentProperties.EndStation);
        DA.SetData(4, alignmentProperties.Length);
        DA.SetData(5, alignmentProperties.CivilAlignmentType.ToString());

        // NamedId properties
        DA.SetData(6, new GH_NamedId(alignmentProperties.Site));
        DA.SetData(7, new GH_NamedId(alignmentProperties.Style));
        DA.SetData(8, new GH_NamedId(alignmentProperties.DesignCheckSet));

        // Extended property types
        DA.SetData(9, new GH_CivilReferenceStation(alignmentProperties.ReferenceStation));
        DA.SetData(10, new GH_CivilDesignSpeeds(alignmentProperties.DesignSpeeds));
        DA.SetData(11, new GH_CivilCANTInfo(cantInfo));
        DA.SetData(12, new GH_CivilConnectedAlignmentInfo(alignmentProperties.ConnectedAlignmentInfo));
        DA.SetData(13, new GH_CivilOffsetAlignmentInfo(alignmentProperties.OffsetAlignmentInfo));
        DA.SetData(14, new GH_CivilRailAlignmentInfo(alignmentProperties.RailAlignmentInfo));
    }
}
