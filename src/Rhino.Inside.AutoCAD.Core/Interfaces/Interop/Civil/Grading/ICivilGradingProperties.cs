namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents properties extracted from a Civil 3D Grading.
/// </summary>
/// <remarks>
/// The Civil 3D .NET API exposes no grading-specific members: the grading group, criteria,
/// footprint feature line and target are only available through the COM API. Only the
/// properties common to every Civil 3D entity are available here.
/// </remarks>
public interface ICivilGradingProperties
{
    /// <summary>
    /// Gets the name of the grading.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the grading.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the style applied to this grading as a NamedId.
    /// </summary>
    INamedId Style { get; }

    /// <summary>
    /// Gets the object id of the grading.
    /// </summary>
    IObjectId GradingId { get; }

    /// <summary>
    /// Updates the grading with new properties and returns a new Grading properties object.
    /// </summary>
    /// <param name="transactionManager">The transaction manager to use for the update.</param>
    /// <param name="newName">The new name for the grading.</param>
    /// <param name="newDescription">The new description for the grading.</param>
    /// <returns>A new <see cref="ICivilGradingProperties"/> instance with updated values.</returns>
    ICivilGradingProperties Update(IAutocadTransactionManager transactionManager,
        string newName, string newDescription);
}
