using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadSolid = Autodesk.AutoCAD.DatabaseServices.Solid3d;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD 3D solids.
/// </summary>
public class Param_AutocadSolid : Param_AutocadObjectBase<GH_AutocadBrepProxy, CadSolid>
{
    private readonly GeometryConverter _converter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.hidden;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("0cafbb12-a9fd-4e8d-be7c-a3296575fb8f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadSolid;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a 3D Solid";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select 3D Solids";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadSolid"/> class.
    /// </summary>
    public Param_AutocadSolid()
        : base("AutoCAD Solid", "AC-Solid",
            "A 3D Solid in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new SolidFilter();

    /// <inheritdoc />
    protected override GH_AutocadBrepProxy WrapEntity(CadSolid entity)
    {
        var proxy = _converter.ToProxyType(entity);

        return new GH_AutocadBrepProxy(proxy);
    }
}
