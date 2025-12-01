using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

public class AutoCadCurveComponent : GH_PersistentParam<GH_Curve>
{
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override Guid ComponentGuid => new Guid("54da72a4-8921-4718-83ff-416bcfbc1d8c");
    public AutoCadCurveComponent() : base("AutoCAD Curve", "Curve",
        "A Curve in AutoCad", "Params", "AutoCAD")
    { }

    protected override GH_GetterResult Prompt_Singular(ref GH_Curve value)
    {
        throw new NotImplementedException();
    }

    protected override GH_GetterResult Prompt_Plural(ref List<GH_Curve> values)
    {
        throw new NotImplementedException();
    }
}
