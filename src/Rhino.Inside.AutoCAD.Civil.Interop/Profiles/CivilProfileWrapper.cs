using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoPolyCurve = Rhino.Geometry.PolyCurve;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <inheritdoc cref="ICivilProfile"/>
public class CivilProfileWrapper : AutocadEntityWrapper, ICivilProfile
{
    private readonly Profile _profile;

    /// <inheritdoc />
    public ICivilProfileProperties Properties { get; }

    /// <summary>
    /// Constructs a new wrapper for Civil Profiles.
    /// </summary>
    public CivilProfileWrapper(Profile profile) : base(profile)
    {
        _profile = profile;
        this.Properties = new CivilProfileProperties(profile);
    }

    /// <summary>
    /// Converts a profile entity to the appropriate wrapper type.
    /// </summary>
    private ICivilProfileEntity? ConvertEntityToWrapper(ProfileEntity entity, int index)
    {
        return entity switch
        {
            ProfileTangent tangent => new CivilProfileTangentWrapper(tangent, index),
            ProfileCircular arc => new CivilProfileCircularArcWrapper(arc, index),
            ProfileParabolaSymmetric parabola => new CivilProfileSymmetricalParabolaWrapper(parabola, index),
            ProfileParabolaAsymmetric asymParabola => new CivilProfileAsymmetricalParabolaWrapper(asymParabola, index),
            _ => new CivilProfileEntityWrapper(entity, index)
        };
    }

    /// <summary>
    /// Extracts all entities from a Civil 3D Profile as wrapper objects.
    /// </summary>
    /// <param name="transactionManager">The transaction manager for database operations.</param>
    /// <returns>A list of profile entity wrappers.</returns>
    public List<ICivilProfileEntity> GetProfileEntities(IAutocadTransactionManager transactionManager)
    {
        var entities = new List<ICivilProfileEntity>();
        var entityCollection = _profile.Entities;

        for (var i = 0; i < entityCollection.Count; i++)
        {
            var entity = entityCollection[i];
            var wrapper = this.ConvertEntityToWrapper(entity, i);

            if (wrapper != null)
            {
                entities.Add(wrapper);
            }
        }

        return entities;
    }

    /// <inheritdoc />
    public bool TryGetParentAlignmentName(IAutocadTransactionManager transactionManager, out ICivilAlignment? alignment)
    {
        alignment = null;
        var alignmentId = _profile.AlignmentId;
        if (alignmentId.IsNull || alignmentId.IsErased)
            return false;

        var cadAlignment = transactionManager.Unwrap()
            .GetObject(alignmentId, OpenMode.ForRead) as Alignment;

        if (cadAlignment is null) return false;

        alignment = new CivilAlignmentWrapper(cadAlignment);
        return true;
    }

    /// <inheritdoc />
    public override IDbObject ShallowClone()
    {
        return new CivilProfileWrapper(_profile);
    }

    /// <inheritdoc />
    public RhinoCurve? ExtractCurve(IAutocadTransactionManager transactionManager)
    {
        var entities = this.GetProfileEntities(transactionManager);

        if (entities.Count == 0)
            return null;

        if (entities.Count == 1)
            return entities[0].ToRhinoCurve();

        // Join multiple entities into a PolyCurve in station order, as the
        // entity collection is ordered by creation, not by station.
        var polyCurve = new RhinoPolyCurve();
        foreach (var entity in entities.OrderBy(e => e.Start.Station))
        {
            var rhinoCurve = entity.ToRhinoCurve();
            if (rhinoCurve != null)
            {
                polyCurve.Append(rhinoCurve);
            }
        }

        return polyCurve;
    }

    /// <inheritdoc />
    public List<ICivilProfileLabelGroup> GetProfileLabelGroups(IAutocadTransactionManager transactionManager)
    {
        var labelGroups = new List<ICivilProfileLabelGroup>();

        try
        {
            // Get profile views that contain this profile through the parent alignment
            var alignmentId = _profile.AlignmentId;
            if (alignmentId.IsNull || alignmentId.IsErased)
                return labelGroups;

            var alignment = transactionManager.Unwrap()
                .GetObject(alignmentId, OpenMode.ForRead) as Alignment;

            if (alignment == null)
                return labelGroups;

            // Get all profile view IDs for this alignment
            var profileViewIds = alignment.GetProfileViewIds();

            foreach (ObjectId profileViewId in profileViewIds)
            {
                if (profileViewId.IsNull || profileViewId.IsErased)
                    continue;

                // Get label group IDs for this profile in this profile view
                var labelGroupClass = RXObject.GetClass(typeof(ProfileLabelGroup));
                var labelGroupIds = ProfileLabelGroup.GetAvailableLabelGroupIds(
                    labelGroupClass, profileViewId, _profile.Id, true);

                foreach (ObjectId labelGroupId in labelGroupIds)
                {
                    if (labelGroupId.IsNull || labelGroupId.IsErased)
                        continue;

                    var labelGroup = transactionManager.Unwrap()
                        .GetObject(labelGroupId, OpenMode.ForRead) as ProfileLabelGroup;

                    if (labelGroup == null)
                        continue;

                    labelGroups.Add(new CivilProfileLabelGroupWrapper(labelGroup));
                }
            }
        }
        catch
        {
            // Return empty list if label extraction fails
        }

        return labelGroups;
    }
}