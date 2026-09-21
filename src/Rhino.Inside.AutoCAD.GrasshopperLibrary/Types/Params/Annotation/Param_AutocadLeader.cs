using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using AnnotationType = Autodesk.AutoCAD.DatabaseServices.AnnotationType;
using CadLeader = Autodesk.AutoCAD.DatabaseServices.Leader;
using CadMLeader = Autodesk.AutoCAD.DatabaseServices.MLeader;
using CadMText = Autodesk.AutoCAD.DatabaseServices.MText;
using ContentType = Autodesk.AutoCAD.DatabaseServices.ContentType;
using OpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;

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
    protected override IObjectFilter CreateSelectionFilter() => new LeaderFilter();

    /// <inheritdoc />
    protected override GH_AutocadLeader WrapEntity(CadMLeader entity) => new GH_AutocadLeader(entity);

    /// <inheritdoc />
    /// <remarks>
    /// <see cref="LeaderFilter"/> lets the user pick legacy LEADER entities as well as
    /// MLEADERs, so a picked <see cref="CadLeader"/> is converted here rather than being
    /// dropped. The entity is the wrapper around the picked object, so it must be
    /// unwrapped before it is tested against the AutoCAD type.
    /// </remarks>
    protected override bool ConvertSupportObject(IEntity entity, out GH_AutocadLeader supportedGoo)
    {
        supportedGoo = null!;

        if (entity.Unwrap() is not CadLeader legacyLeader) return false;

        var mleader = this.ConvertLegacyLeaderToMLeader(legacyLeader);

        if (mleader == null) return false;

        supportedGoo = new GH_AutocadLeader(mleader);

        return true;
    }

    /// <summary>
    /// Converts a legacy Leader to an MLeader, preserving its leader line, appearance
    /// and MText annotation. The MLeader derives its own normal from those vertices.
    /// </summary>
    /// <param name="legacyLeader">The legacy Leader to convert.</param>
    /// <returns>
    /// The equivalent MLeader, or <see langword="null"/> when the legacy Leader has too
    /// few vertices to describe a leader line.
    /// </returns>
    private CadMLeader? ConvertLegacyLeaderToMLeader(CadLeader legacyLeader)
    {
        if (legacyLeader.NumVertices < 2) return null;

        var mleader = new CadMLeader();

        // Database defaults populate the MLeader style, text style and linetype. They
        // must be applied before anything else is set: assigning properties such as the
        // layer on an MLeader that has no database throws eNoDatabase.
        var database = legacyLeader.Database;

        if (database != null)
            mleader.SetDatabaseDefaults(database);
        else
            mleader.SetDatabaseDefaults();

        mleader.SetPropertiesFrom(legacyLeader);

        // A zero size means the Leader inherits its arrow from the dimension style, in
        // which case the MLeader style's own arrow is the right default.
        if (legacyLeader.Dimasz > 0.0)
            mleader.ArrowSize = legacyLeader.Dimasz;

        // A legacy Leader's geometry is exactly its vertices. The dogleg and landing an
        // MLeader adds by default would introduce a segment the original does not have.
        mleader.EnableDogleg = false;
        mleader.EnableLanding = false;

        var annotation = ReadAnnotationMText(legacyLeader);

        // The content type is set before the leader lines are added: changing it
        // afterwards rebuilds the MLeader's content and discards them.
        mleader.ContentType = annotation == null
            ? ContentType.NoneContent
            : ContentType.MTextContent;

        var leaderIndex = mleader.AddLeader();
        var lineIndex = mleader.AddLeaderLine(leaderIndex);

        // Vertex order matches: index 0 is the arrowhead on both types.
        for (var i = 0; i < legacyLeader.NumVertices; i++)
        {
            mleader.AddLastVertex(lineIndex, legacyLeader.VertexAt(i));
        }

        if (annotation != null)
            mleader.MText = annotation;

        return mleader;
    }

    /// <summary>
    /// Reads a copy of the legacy Leader's MText annotation.
    /// </summary>
    /// <param name="legacyLeader">The legacy Leader supplying the annotation.</param>
    /// <returns>
    /// A copy of the annotation, or <see langword="null"/> when the Leader has no
    /// annotation or is annotated with a block or a feature control frame.
    /// </returns>
    /// <remarks>
    /// The annotation is a separate database object referenced by id, and the picking
    /// transaction has already been committed by the time this runs, so it is read in a
    /// transaction of its own. The copy is taken because the annotation belongs to the
    /// drawing whereas the MLeader takes ownership of the MText it is given.
    /// </remarks>
    private static CadMText? ReadAnnotationMText(CadLeader legacyLeader)
    {
        if (legacyLeader.AnnoType != AnnotationType.MText) return null;

        var annotationId = legacyLeader.Annotation;

        if (annotationId.IsNull || annotationId.IsValid == false) return null;

        var database = legacyLeader.Database;

        if (database == null) return null;

        using var transaction = database.TransactionManager.StartTransaction();

        var annotation = transaction.GetObject(annotationId, OpenMode.ForRead) as CadMText;

        var copy = annotation?.Clone() as CadMText;

        transaction.Commit();

        return copy;
    }
}
