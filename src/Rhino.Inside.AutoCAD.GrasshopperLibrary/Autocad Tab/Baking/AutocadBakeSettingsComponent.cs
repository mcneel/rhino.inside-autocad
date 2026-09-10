using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates bake settings for AutoCAD objects.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.3.2")]
public class AutocadBakeSettingsComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("B4C6D8E0-F2A4-4B6C-9D8E-0F2A4B6C8D0E");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadBakeSettingsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadBakeSettingsComponent"/> class.
    /// </summary>
    public AutocadBakeSettingsComponent()
        : base("AutoCAD Bake Settings", "AC-BkSet",
            "Creates bake settings for AutoCAD objects",
            "AutoCAD", "Baking")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.item), "Layer",
            "L", "The layer to assign to baked objects", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddParameter(new Param_AutocadLineType(GH_ParamAccess.item), "LineType",
            "LT", "The line type to assign to baked objects", GH_ParamAccess.item);
        pManager[1].Optional = true;

        pManager.AddParameter(new Param_AutocadColor(GH_ParamAccess.item), "Color", "C",
            "The color to assign to baked objects", GH_ParamAccess.item);
        pManager[2].Optional = true;

        pManager.AddNumberParameter("LinetypeScale", "LTS",
            "The linetype scale to assign to baked objects", GH_ParamAccess.item);
        pManager[3].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_BakeSettings(GH_ParamAccess.item), "Settings",
            "S", "The bake settings", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_AutocadLayer? layerGoo = null;
        DA.GetData(0, ref layerGoo);

        GH_AutocadLineType? lineTypeGoo = null;
        DA.GetData(1, ref lineTypeGoo);

        GH_AutocadColor? colorGoo = null;
        DA.GetData(2, ref colorGoo);

        GH_Number? linetypeScaleGoo = null;
        DA.GetData(3, ref linetypeScaleGoo);

        var layer = layerGoo?.Value;
        var lineType = lineTypeGoo?.Value;
        var color = colorGoo?.Value;
        var linetypeScale = linetypeScaleGoo?.Value;

        var settings = new BakeSettings(layer, lineType, color, linetypeScale);

        var settingsGoo = new GH_BakeSettings(settings);

        DA.SetData(0, settingsGoo);
    }
}