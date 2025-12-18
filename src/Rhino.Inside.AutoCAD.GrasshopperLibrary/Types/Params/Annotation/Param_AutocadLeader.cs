using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLeader = Autodesk.AutoCAD.DatabaseServices.Leader;
using CadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD Leaders (MLeader).
/// </summary>
public class Param_AutocadLeader : Param_AutocadObjectBase<GH_AutocadLeader, CadMLeader>
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
    protected override IFilter CreateSelectionFilter() => new LeaderFilter();

    /// <inheritdoc />
    protected override GH_AutocadLeader WrapEntity(CadMLeader entity) => new GH_AutocadLeader(entity);

    /// <inheritdoc />
    protected override bool ConvertSupportObject(IEntity entity, out GH_AutocadLeader supportedGoo)
    {
        if (entity is CadLeader legacyLeader)
        {
            var mleader = ConvertLegacyLeaderToMLeader(legacyLeader);
            supportedGoo = new GH_AutocadLeader(mleader);
            return true;
        }

        supportedGoo = null!;
        return false;
    }

    /// <summary>
    /// Converts a legacy Leader to an MLeader.
    /// </summary>
    private CadMLeader ConvertLegacyLeaderToMLeader(CadLeader legacyLeader)
    {
        var mleader = new CadMLeader();

        var leaderIndex = mleader.AddLeader();
        var lineIndex = mleader.AddLeaderLine(leaderIndex);

        for (var i = 0; i < legacyLeader.NumVertices; i++)
        {
            mleader.AddLastVertex(lineIndex, legacyLeader.VertexAt(i));
        }

        mleader.Layer = legacyLeader.Layer;
        mleader.Color = legacyLeader.Color;

        return mleader;
    }
}
