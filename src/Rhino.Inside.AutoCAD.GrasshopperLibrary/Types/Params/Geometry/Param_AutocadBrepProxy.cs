using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadSolid = Autodesk.AutoCAD.DatabaseServices.Solid3d;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD 3D solids.
/// </summary>
public class Param_AutocadBrepProxy : Param_AutocadObjectBase<GH_AutocadBrepProxy, AutocadBrepProxy>
{
    private readonly GeometryConverter _converter = GeometryConverter.Instance!;

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("b8f2e7c3-4d91-4a5e-9c1f-3e8b6a2d7f4c");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadSolid;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a 3D Solid";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select 3D Solids";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadBrepProxy"/> class.
    /// </summary>
    public Param_AutocadBrepProxy()
        : base("AutoCAD Solid", "AC-Solid",
            "A 3D Solid in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IFilter CreateSelectionFilter() => new SolidFilter();

    /// <inheritdoc />
    protected override GH_AutocadBrepProxy WrapEntity(AutocadBrepProxy entity) => new GH_AutocadBrepProxy(entity);

    protected override bool ConvertSupportObject(IEntity entity, out GH_AutocadBrepProxy? supportedGoo)
    {
        if (entity is CadSolid cadSolid)
        {
            var proxy = _converter.ToProxyType(cadSolid);
            supportedGoo = new GH_AutocadBrepProxy(proxy);
            return true;
        }

        supportedGoo = null;
        return false;
    }
}