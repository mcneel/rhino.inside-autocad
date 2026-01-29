using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A value list component that provides a dropdown of AutoCAD element filter types.
/// </summary>
public class FilterValueList : GH_ValueList
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("C5D7E9F1-A3B5-4C7D-8E9F-1A3B5C7D9E0F");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_Filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterValueList"/> class.
    /// </summary>
    public FilterValueList()
    {
        this.Name = "AutoCAD Geometry Filter Type";
        this.NickName = "AC-Geometry Filter";
        this.Description = "Select an AutoCAD element filter type";
        this.Category = "AutoCAD";
        this.SubCategory = "Document";

        this.ListItems.Clear();

        this.ListItems.Add(new GH_ValueListItem("Curve", "\"Curve\""));
        this.ListItems.Add(new GH_ValueListItem("Point", "\"Point\""));
        this.ListItems.Add(new GH_ValueListItem("Mesh", "\"Mesh\""));
        this.ListItems.Add(new GH_ValueListItem("Solid", "\"Solid\""));
        this.ListItems.Add(new GH_ValueListItem("Hatch", "\"Hatch\""));
        this.ListItems.Add(new GH_ValueListItem("Dimension", "\"Dimension\""));
        this.ListItems.Add(new GH_ValueListItem("Text", "\"Text\""));
        this.ListItems.Add(new GH_ValueListItem("Leader", "\"Leader\""));
        this.ListItems.Add(new GH_ValueListItem("Block", "\"Block\""));


        this.SelectItem(0);
    }
}
