namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a reference to an AutoCAD object. This includes an <see cref="IObjectId"/>
/// which uniquely identifies the object within the AutoCAD database but may not guarantee
/// persistence across sessions and the objects Handle which should persist.
/// </summary>
public interface IAutocadReferenceId
{
    /// <summary>
    /// Gets the <see cref="IObjectId"/> associated with the AutoCAD object.
    /// </summary>
    IObjectId ObjectId { get; }

    /// <summary>
    /// Gets a value indicating whether the reference is valid.
    /// </summary>
    /// <value>
    /// <c>true</c> if the reference is valid; otherwise, <c>false</c>.
    /// </value>
    bool IsValid { get; }

    /// <summary>
    /// Serializes the AutoCAD object reference handle to a string representation.
    /// </summary>
    /// <returns>
    /// A string that represents the serialized value of the AutoCAD object reference.
    /// </returns>
    string GetSerializedValue();
}