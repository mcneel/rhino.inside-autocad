namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a validation error for an entity, providing details about the error.
/// </summary>
public interface IEntityValidationError
{
    /// <summary>
    /// Gets or sets the type of the entity that caused the validation error.
    /// </summary>
    string EntityType { get; }

    /// <summary>
    /// Gets or sets the handle of the entity, which uniquely identifies it.
    /// </summary>
    string Handle { get; }

    /// <summary>
    /// Gets or sets the message describing the validation error.
    /// </summary>
    string Message { get; }
}