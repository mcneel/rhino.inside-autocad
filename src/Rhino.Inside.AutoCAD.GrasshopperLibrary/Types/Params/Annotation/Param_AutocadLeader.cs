using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using CadLeader = Autodesk.AutoCAD.DatabaseServices.Leader;
using CadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD Leaders, accepting both the legacy
/// <see cref="CadLeader"/> and <see cref="CadMLeader"/>.
/// </summary>
/// <remarks>
/// The entity type is <see cref="CadEntity"/> because those two AutoCAD types share no
/// closer ancestor. <see cref="LeaderFilter"/> restricts what can actually be picked to
/// LEADER and MLEADER, so the parameter never holds an unrelated entity.
/// </remarks>
public class Param_AutocadLeader : Param_AutocadObjectBase<GH_AutocadLeader, CadEntity>
{
    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("d7f1a4b5-0e6c-4b8d-c2f9-1a5b4c3d6e7f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadLeader;

    /// <inheritdoc />
    protected override string SingularPromptMessage => "Select a Leader";

    /// <inheritdoc />
    protected override string PluralPromptMessage => "Select Leaders";

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLeader"/> class.
    /// </summary>
    public Param_AutocadLeader()
        : base("AutoCAD Leader", "Leader",
            "A Leader in AutoCAD", "Params", "AutoCAD")
    { }

    /// <inheritdoc />
    protected override IObjectFilter CreateSelectionFilter() => new LeaderFilter();

    /// <inheritdoc />
    /// <remarks>
    /// The default branch is unreachable through a prompt, because
    /// <see cref="LeaderFilter"/> only lets LEADER and MLEADER entities be picked.
    /// </remarks>
    protected override GH_AutocadLeader WrapEntity(CadEntity entity)
    {
        return entity switch
        {
            CadMLeader mLeader => new GH_AutocadLeader(mLeader),
            CadLeader leader => new GH_AutocadLeader(leader),
            _ => new GH_AutocadLeader(),
        };
    }
}
