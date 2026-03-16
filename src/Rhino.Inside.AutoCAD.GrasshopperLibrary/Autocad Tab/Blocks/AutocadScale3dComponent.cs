using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates an AutoCAD Scale 3d.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class AutocadScale3dComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("dfd32da6-30c3-4dae-8414-aa4b6069e146");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadScale3dComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadScale3dComponent"/> class.
    /// </summary>
    public AutocadScale3dComponent()
        : base("Autocad Scale3d", "AC-Scale",
            "Creates a Scale with 3 factors",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddNumberParameter("X", "X", "The X Scale Factor, if non of the other factors are connected this will be used create a uniform scale", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddNumberParameter("Y", "Y", "The Y Scale Factor, if non of the other factors are connected this will be used create a uniform scale", GH_ParamAccess.item);
        pManager[1].Optional = true;

        pManager.AddNumberParameter("Z", "Z", "The Z Scale Factor, if non of the other factors are connected this will be used create a uniform scale", GH_ParamAccess.item);
        pManager[2].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadScale(GH_ParamAccess.item),
            "Scale3d", "Scale", "An AutoCAD 3d scale", GH_ParamAccess.item);
    }

    /// <summary>
    /// Returns a Uniform Scale if only one factor is connected, otherwise a non uniform scale
    /// </summary>
    private AutocadScale GetScale(double xFactor, double yFactor, double zFactor)
    {
        var xIsConnected = this.Params.Input[0].SourceCount > 0;
        var yIsConnected = this.Params.Input[1].SourceCount > 0;
        var zIsConnected = this.Params.Input[2].SourceCount > 0;

        var connectionCount = (xIsConnected ? 1 : 0) + (yIsConnected ? 1 : 0) + (zIsConnected ? 1 : 0);

        if (connectionCount == 1)
        {
            if (xIsConnected)
                return new AutocadScale(xFactor);
            if (yIsConnected)
                return new AutocadScale(xFactor);
            if (zIsConnected)
                return new AutocadScale(xFactor);
        }

        return new AutocadScale(xFactor, yFactor, zFactor);

    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        double xFactor = 1;
        double yFactor = 1;
        double zFactor = 1;

        DA.GetData(0, ref xFactor);
        DA.GetData(1, ref yFactor);
        DA.GetData(2, ref zFactor);

        var scale3d = this.GetScale(xFactor, yFactor, zFactor);

        var goo = new GH_Scale3d(scale3d);

        DA.SetData(0, goo);
    }
}