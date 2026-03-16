using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD Block Reference currently open in the AutoCAD session.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.0.16")]
public class AutocadBlockReferenceComponent : RhinoInsideAutocad_ComponentBase
{
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
            "Gets Information from an AutoCAD Block Reference",
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

        pManager.AddNumberParameter("Rotation", "Rotation", "The Rotation of the Block Reference.",
            GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadScale(GH_ParamAccess.item), "Scale", "Scale", "The Scale of the Block Reference. This will take either one uniform number or three numbers for a non uniform scale",
            GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "BlockTableRecordId",
            "BlockTableRecordId", "The Object Id  of the AutoCAD Block Table Record that this BlockReference is a reference of.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_DynamicBlockReferenceProperty(GH_ParamAccess.list),
            "Properties", "P", "The Dynamic Block Reference Properties", GH_ParamAccess.list);

        pManager.AddParameter(new Param_BlockAttributeReference(GH_ParamAccess.list),
            "Attributes", "Attr", "The Block Reference Attributes", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadBlockReferenceWrapper? blockReferenceWrapper = null;

        if (!DA.GetData(0, ref blockReferenceWrapper)
            || blockReferenceWrapper is null) return;

        var document = RhinoInsideAutoCadExtension.Application.RhinoInsideManager
            .AutoCadInstance.ActiveDocument;

        var gooProperties = document.Transaction((transactionManager) =>
        {
            var dynamicProperties =
                blockReferenceWrapper.GetDynamicProperties(transactionManager);

            return dynamicProperties.Select(property =>
            new GH_DynamicBlockReferenceProperty(property));
        });

        var gooAttributes = document.Transaction((transactionManager) =>
        {
            var attributesSet =
                blockReferenceWrapper.GetAttributes(transactionManager);

            return attributesSet.Select(property =>
                new GH_BlockAttributeReference(property));
        });

        var blockTableRecordIdGoo =
            new GH_AutocadObjectId(blockReferenceWrapper.BlockTableRecordId);

        DA.SetData(0, blockReferenceWrapper.Name);
        DA.SetData(1, blockReferenceWrapper.Id);
        DA.SetData(2, blockReferenceWrapper.Position);
        DA.SetData(3, blockReferenceWrapper.Rotation);
        DA.SetData(4, blockReferenceWrapper.Scale);
        DA.SetData(5, blockTableRecordIdGoo);
        DA.SetDataList(6, gooProperties);
        DA.SetDataList(7, gooAttributes);
    }
}