namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped AutoCAD AssocNetwork object.
/// </summary>
public interface IAssocNetwork : IDbObject
{
    /// <summary>
    /// Gets the ObjectIds of all actions in the AssocNetwork.
    /// </summary>
    IReadOnlyList<IObjectId> Actions { get; }

    /// <summary>
    /// Creates a shallow clone of the <see cref="IAssocNetwork"/>.
    /// </summary>
    new IAssocNetwork ShallowClone();
}