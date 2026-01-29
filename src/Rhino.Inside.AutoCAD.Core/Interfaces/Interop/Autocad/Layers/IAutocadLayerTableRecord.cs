namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped AutoCAD LayerTableRecord
/// </summary>
public interface IAutocadLayerTableRecord : IDbObject, IDisposable
{
    /// <summary>
    /// The <see cref="IColor"/> of the
    /// <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    IColor Color { get; }

    /// <summary>
    /// The <see cref="IObjectId"/> of the <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    IObjectId Id { get; }

    /// <summary>
    /// The <see cref="IObjectId"/> of the line pattern for this layer.
    /// </summary>
    IObjectId LinePattenId { get; }

    /// <summary>
    /// The name of the <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns true if the  <see cref="IAutocadLayerTableRecord"/> is
    /// valid, otherwise returns false.
    /// </summary>>
    bool IsValid { get; }

    /// <summary>
    /// Returns true if the <see cref="IAutocadLayerTableRecord"/> is locked, and false otherwise.
    /// </summary>
    /// <remarks>
    /// A locked <see cref="IAutocadLayerTableRecord"/> restricts any modifications to the
    /// <see cref="IEntity"/>s within it, making them as read-only.
    /// </remarks>
    bool IsLocked { get; }

    /// <summary>
    /// Returns the <see cref="IAutocadLinetypeTableRecord"/> assigned to the <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    IAutocadLinetypeTableRecord GetLinePattern(ILineTypeRepository lineTypeRepository);

    /// <summary>
    /// Creates a shallow clone of the <see cref="IAutocadLayerTableRecord"/>.
    /// </summary>
    IAutocadLayerTableRecord ShallowClone();
}