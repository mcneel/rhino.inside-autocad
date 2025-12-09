namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped AutoCAD Layout.
/// </summary>
public interface IAutocadLayout : IDbObject, ITaggedObjectId
{
    /// <summary>
    /// The name of the <see cref="IAutocadLayout"/>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The tab display order of the <see cref="IAutocadLayout"/>.
    /// </summary>
    int TabOrder { get; }

    /// <summary>
    /// The <see cref="IObjectId"/> of the BlockTableRecord associated with this <see cref="IAutocadLayout"/>.
    /// </summary>
    IObjectId BlockTableRecordId { get; }

    /// <summary>
    /// Creates a shallow clone of the <see cref="IAutocadLayout"/>.
    /// </summary>
    IAutocadLayout ShallowClone();
}