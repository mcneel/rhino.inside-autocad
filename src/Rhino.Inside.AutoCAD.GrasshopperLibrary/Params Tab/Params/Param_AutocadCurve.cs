using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;
using Rhino.Inside.AutoCAD.Interop.Filters;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD curves.
/// </summary>
public class Param_AutocadCurve : GH_PersistentParam<GH_AutocadCurve>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("54da72a4-8921-4718-83ff-416bcfbc1d8c");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadCurve;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadCurve"/> class.
    /// </summary>
    public Param_AutocadCurve()
        : base("AutoCAD Curve", "Curve",
            "A Curve in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Singular(ref GH_AutocadCurve value)
    {
        var picker = new AutocadObjectPicker();

        var filter = new CurveFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var message = "Select a Curve";

        var entity = picker.PickObject(selectionFilter, message);

        if (entity.Unwrap() is CadCurve cadCurve)
        {
            value = new GH_AutocadCurve(cadCurve);
            return GH_GetterResult.success;
        }

        value = default;
        return GH_GetterResult.cancel;
    }

    /// <inheritdoc />
    protected override GH_GetterResult Prompt_Plural(ref List<GH_AutocadCurve> values)
    {
        var picker = new AutocadObjectPicker();

        var filter = new CurveFilter();

        var selectionFilter = filter.GetSelectionFilter();

        var message = "Select Curves";

        var entities = picker.PickObjects(selectionFilter, message);

        foreach (var entity in entities)
        {
            if (entity is CadCurve cadCurve)
            {
                values.Add(new GH_AutocadCurve(cadCurve));
            }
        }

        return GH_GetterResult.success;
    }
}