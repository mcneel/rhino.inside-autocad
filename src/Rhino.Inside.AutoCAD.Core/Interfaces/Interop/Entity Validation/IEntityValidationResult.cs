namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the result of the validation process for entities, including the ability to track errors.
/// </summary>
public interface IEntityValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation result is valid.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Adds a validation error to the result.
    /// </summary>
    /// <param name="error">The validation error to add.</param>
    void AddError(IEntityValidationError error);

    /// <summary>
    /// Returns a string representation of the validation result, including any errors.
    /// </summary>
    /// <returns>A string describing the validation result.</returns>
    string ToString();
}