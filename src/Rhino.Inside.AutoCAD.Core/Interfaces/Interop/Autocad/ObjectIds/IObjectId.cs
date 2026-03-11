namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a unique identifier for an object in an AutoCAD database.
/// </summary>
/// <remarks>
/// Wraps an AutoCAD ObjectId to provide platform-independent access to database object
/// references. ObjectIds are session-specific and may change when a drawing is closed
/// and reopened; use handles for persistent identification. Used throughout the Grasshopper
/// component library, including AutocadObjectIdComponent and GetByAutocadIdComponent.
/// </remarks>
/// <seealso cref="IDbObject"/>
/// <seealso cref="IDbObject.Id"/>
public interface IObjectId
{
    /// <summary>
    /// Gets the underlying numeric value of this ObjectId.
    /// </summary>
    /// <remarks>
    /// This value uniquely identifies the object within the current database session.
    /// The value is not persisted across save/load cycles.
    /// </remarks>
    long Value { get; }

    /// <summary>
    /// Gets a value indicating whether this ObjectId references a valid database object.
    /// </summary>
    /// <remarks>
    /// Returns <c>false</c> for null ObjectIds or if the referenced database has been closed.
    /// Always check validity before attempting to open the referenced object.
    /// </remarks>
    bool IsValid { get; }

    /// <summary>
    /// Gets a value indicating whether the referenced object has been erased.
    /// </summary>
    /// <remarks>
    /// Erased objects remain in the database until purged but are not visible in the drawing.
    /// An ObjectId can be valid but reference an erased object.
    /// </remarks>
    bool IsErased { get; }

    /// <summary>
    /// Creates a copy of this ObjectId wrapper.
    /// </summary>
    /// <returns>
    /// A new <see cref="IObjectId"/> instance with the same underlying value.
    /// </returns>
    IObjectId ShallowClone();

    /// <summary>
    /// Determines whether this ObjectId references the same database object as another.
    /// </summary>
    /// <param name="other">
    /// The <see cref="IObjectId"/> to compare with.
    /// </param>
    /// <returns>
    /// <c>true</c> if both ObjectIds reference the same object; otherwise, <c>false</c>.
    /// </returns>
    bool IsEqualTo(IObjectId other);
}