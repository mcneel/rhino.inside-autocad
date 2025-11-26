namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents any object that can be tagged with an <see cref="IObjectIdTag"/>.
/// </summary>
public interface ITaggedObjectId
{
    /// <summary>
    /// Returns the tag of the object.
    /// </summary>
    IObjectIdTag GetTag();
}