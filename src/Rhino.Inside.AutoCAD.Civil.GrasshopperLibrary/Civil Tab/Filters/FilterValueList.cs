using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A value list component that provides a dropdown of AutoCAD element filter types.
/// </summary>
public class FilterValueList : GH_ValueList
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("70F9E2D3-3EA2-4DBB-87D3-3F8D5AC2DF19");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_Filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterValueList"/> class.
    /// </summary>
    public FilterValueList()
    {
        this.Name = "Civil3d Filter Type";
        this.NickName = "CVL-Filter";
        this.Description = "Select an Civil 3d element filter type";
        this.Category = "AutoCAD";
        this.SubCategory = "Filter";

        this.ListItems.Clear();

        this.ListItems.Add(new GH_ValueListItem("Alignment", "\"alignment\""));
        this.ListItems.Add(new GH_ValueListItem("Assembly", "\"assembly\""));
        this.ListItems.Add(new GH_ValueListItem("Corridor", "\"corridor\""));
        this.ListItems.Add(new GH_ValueListItem("Feature Line", "\"featureline\""));
        this.ListItems.Add(new GH_ValueListItem("Grading", "\"grading\""));
        this.ListItems.Add(new GH_ValueListItem("Parcel", "\"parcel\""));
        this.ListItems.Add(new GH_ValueListItem("Profile", "\"profile\""));
        this.ListItems.Add(new GH_ValueListItem("Profile View", "\"profileview\""));
        this.ListItems.Add(new GH_ValueListItem("TIN Surface", "\"tinsurface\""));
        this.ListItems.Add(new GH_ValueListItem("TIN Volume Surface", "\"tinvolumesurface\""));

        this.SelectItem(0);
    }
}