namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents properties extracted from a Civil 3D Feature Line.
/// </summary>
/// <remarks>
/// Lengths and elevations are reported in drawing units, matching the values shown
/// in the Civil 3D Feature Line properties.
/// </remarks>
public interface ICivilFeatureLineProperties
{
    /// <summary>
    /// Gets the name of the feature line.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the feature line.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the style applied to this feature line as a NamedId.
    /// </summary>
    INamedId Style { get; }

    /// <summary>
    /// Gets the site that owns this feature line as a NamedId. Empty for siteless feature lines.
    /// </summary>
    INamedId Site { get; }

    /// <summary>
    /// Gets the 2D (plan) length of the feature line.
    /// </summary>
    double Length2D { get; }

    /// <summary>
    /// Gets the 3D length of the feature line.
    /// </summary>
    double Length3D { get; }

    /// <summary>
    /// Gets the minimum elevation of the feature line.
    /// </summary>
    double MinElevation { get; }

    /// <summary>
    /// Gets the maximum elevation of the feature line.
    /// </summary>
    double MaxElevation { get; }

    /// <summary>
    /// Gets the minimum grade of the feature line.
    /// </summary>
    double MinGrade { get; }

    /// <summary>
    /// Gets the maximum grade of the feature line.
    /// </summary>
    double MaxGrade { get; }

    /// <summary>
    /// Gets the number of PI points in the feature line.
    /// </summary>
    int PIPointCount { get; }

    /// <summary>
    /// Gets the number of elevation points in the feature line.
    /// </summary>
    int ElevationPointCount { get; }

    /// <summary>
    /// Gets the number of arc segments in the feature line.
    /// </summary>
    int CurveCount { get; }

    /// <summary>
    /// Gets the object id of the feature line.
    /// </summary>
    IObjectId FeatureLineId { get; }

    /// <summary>
    /// Updates the feature line with new properties and returns a new
    /// Feature Line properties object.
    /// </summary>
    /// <param name="transactionManager">The transaction manager to use for the update.</param>
    /// <param name="newName">The new name for the feature line.</param>
    /// <param name="newDescription">The new description for the feature line.</param>
    /// <param name="newStyleName">The new style name for the feature line.</param>
    /// <returns>A new <see cref="ICivilFeatureLineProperties"/> instance with updated values.</returns>
    ICivilFeatureLineProperties Update(IAutocadTransactionManager transactionManager,
        string newName, string newDescription, string newStyleName);
}
