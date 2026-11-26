using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CivilSurface = Autodesk.Civil.DatabaseServices.Surface;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <summary>
/// Sets the elevations of a Civil 3D Feature Line from a surface.
/// </summary>
/// <remarks>
/// Wraps <see cref="FeatureLine.AssignElevationsFromSurface(ObjectId, bool)"/>, the same
/// operation as Civil 3D's "Elevations From Surface" command. The elevations are set once
/// and are not linked to the surface.
/// </remarks>
public static class FeatureLineSurfaceElevations
{
    /// <summary>
    /// Assigns elevations from a surface to every point of a feature line.
    /// </summary>
    /// <param name="transactionManager">The transaction manager for database operations.</param>
    /// <param name="featureLineId">The feature line to update.</param>
    /// <param name="surfaceId">The surface to take elevations from. Must be in the same drawing.</param>
    /// <param name="insertGradeBreaks">True to insert elevation points (grade breaks) where the
    /// feature line crosses the surface's triangle edges.</param>
    /// <returns>The updated feature line, opened for write.</returns>
    public static FeatureLine Assign(
        IAutocadTransactionManager transactionManager,
        IObjectId featureLineId,
        IObjectId surfaceId,
        bool insertGradeBreaks)
    {
        var transaction = transactionManager.Unwrap();

        if (transaction.GetObject(surfaceId.Unwrap(), OpenMode.ForRead) is not CivilSurface)
            throw new ArgumentException("The surface input is not a Civil 3D surface.");

        if (transaction.GetObject(featureLineId.Unwrap(), OpenMode.ForWrite) is not FeatureLine featureLine)
            throw new ArgumentException("The feature line input is not a Civil 3D feature line.");

        featureLine.AssignElevationsFromSurface(surfaceId.Unwrap(), insertGradeBreaks);

        return featureLine;
    }
}
