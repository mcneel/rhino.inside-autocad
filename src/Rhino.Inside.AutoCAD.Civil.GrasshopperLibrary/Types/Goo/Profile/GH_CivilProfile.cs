using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;
using CivilProfile = Autodesk.Civil.DatabaseServices.Profile;
using RhinoCurve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for Civil 3D Profiles.
/// </summary>
public class GH_CivilProfile : GH_AutocadGeometricGoo<CivilProfile, RhinoGeometryAdapter<RhinoCurve>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilProfile"/> class with no value.
    /// </summary>
    public GH_CivilProfile()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_CivilProfile"/> class with the
    /// specified Civil 3D Profile.
    /// </summary>
    /// <param name="profile">The Civil 3D Profile to wrap.</param>
    public GH_CivilProfile(CivilProfile profile) : base(profile)
    {
    }

    /// <summary>
    /// A private constructor used to create a reference Goo which is a clone of the
    /// input profile.
    /// </summary>
    private GH_CivilProfile(CivilProfile profile, IAutocadReferenceId referenceId) : base(profile, referenceId)
    {
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CivilProfile, RhinoGeometryAdapter<RhinoCurve>> CreateClonedInstance(CivilProfile entity)
    {
        return new GH_CivilProfile(entity.Clone() as CivilProfile, this.Reference);
    }

    /// <inheritdoc />
    protected override GH_AutocadGeometricGoo<CivilProfile, RhinoGeometryAdapter<RhinoCurve>> CreateInstance(CivilProfile entity)
    {
        return new GH_CivilProfile(entity);
    }

    /// <inheritdoc />
    protected override CivilProfile? Convert(RhinoGeometryAdapter<RhinoCurve> rhinoType)
    {
        // Converting from Rhino Curve back to Civil 3D Profile is not supported
        return null;
    }

    /// <inheritdoc />
    protected override RhinoGeometryAdapter<RhinoCurve>? Convert(CivilProfile wrapperType)
    {
        var database = wrapperType.Database;
        if (database == null)
            return null;

        using var transaction = database.TransactionManager.StartTransaction();
        var transactionManager = new AutocadTransactionManagerWrapper(
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument);

        var curve = wrapperType.ToRhinoCurve(transactionManager);
        transaction.Commit();

        return curve != null ? new RhinoGeometryAdapter<RhinoCurve>(curve) : null;
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryWires(GH_PreviewWireArgs args)
    {
        var curve = this.RhinoGeometry?.Geometry;
        if (curve != null)
        {
            args.Pipeline.DrawCurve(curve, args.Color, args.Thickness);
        }
    }

    /// <inheritdoc />
    protected override void DrawViewportGeometryMeshes(GH_PreviewMeshArgs args)
    {
        // Profiles are drawn as wires only
    }

    /// <inheritdoc />
    public override void AppendInputSignature(IInputSignatureBuilder inputSignatureBuilder)
    {
        var geometry = this.RhinoGeometry?.Geometry;

        if (geometry == null)
        {
            inputSignatureBuilder.Add(this.ToString());
            return;
        }

        inputSignatureBuilder.AddCurve(geometry);
    }

    /// <inheritdoc />
    public override void DrawAutocadPreview(IGrasshopperPreviewData previewData)
    {
        var curve = this.RhinoGeometry?.Geometry;

        if (curve == null) return;

        previewData.Wires.Add(curve);
    }
}
