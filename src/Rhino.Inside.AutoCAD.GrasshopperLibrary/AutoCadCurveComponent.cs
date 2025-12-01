using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Params_Tab.ObjectPicker;
using Rhino.Inside.AutoCAD.Interop;
using CadCurve = Autodesk.AutoCAD.DatabaseServices.Curve;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

public class AutoCadCurveComponent : GH_PersistentParam<GH_Curve>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override Guid ComponentGuid => new Guid("54da72a4-8921-4718-83ff-416bcfbc1d8c");
    public AutoCadCurveComponent() : base("AutoCAD Curve", "Curve",
        "A Curve in AutoCad", "Params", "AutoCAD")
    { }

    protected override GH_GetterResult Prompt_Singular(ref GH_Curve value)
    {
        var activeDocument = RhinoInsideAutoCadExtension.Application!.RhinoInsideManager.AutoCadInstance.ActiveDocument;

        var picker = new AutocadObjectPicker(activeDocument);

        var filter = new SelectionFilter(new[]
        {
            new TypedValue(-4, "<OR"),
            new TypedValue(0, "ARC,CIRCLE,ELLIPSE,LEADER,LINE,LWPOLYLINE,RAY,SPLINE,XLINE"),
            new TypedValue(-4, "<AND"),
            new TypedValue(0, "POLYLINE"),
            new TypedValue(-4, "&"),
            new TypedValue(70, 16 | 32 | 64),
            new TypedValue(-4, "AND>"),
            new TypedValue(-4, "OR>")
        });

        var selectionFilter = new SelectionFilterWrapper(filter);

        var message = "Select a Curve";

        var entity = picker.PickObject(selectionFilter, message);

        if (entity.Unwrap() is CadCurve cadCurve)
        {
            var rhinoCurve = _geometryConverter.ToRhinoType(cadCurve!);

            value = new GH_Curve(rhinoCurve);
            return GH_GetterResult.success;

        }

        value = default;
        return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Plural(ref List<GH_Curve> values)
    {
        var activeDocument = RhinoInsideAutoCadExtension.Application!.RhinoInsideManager.AutoCadInstance.ActiveDocument;

        var picker = new AutocadObjectPicker(activeDocument);

        var filter = new SelectionFilter(new[]
        {
            new TypedValue(-4, "<OR"),
            new TypedValue(0, "ARC,CIRCLE,ELLIPSE,LEADER,LINE,LWPOLYLINE,RAY,SPLINE,XLINE"),
            new TypedValue(-4, "<AND"),
            new TypedValue(0, "POLYLINE"),
            new TypedValue(-4, "&"),
            new TypedValue(70, 16 | 32 | 64),
            new TypedValue(-4, "AND>"),
            new TypedValue(-4, "OR>")
        });
        var selectionFilter = new SelectionFilterWrapper(filter);

        var message = "Select Curves";

        var entities = picker.PickObjects(selectionFilter, message);

        foreach (var entity in entities)
        {
            if (entity is CadCurve cadCurve)
            {
                var rhinoCurve = _geometryConverter.ToRhinoType(cadCurve!);

                values.Add(new GH_Curve(rhinoCurve));
            }
        }

        return GH_GetterResult.success;
    }
}

