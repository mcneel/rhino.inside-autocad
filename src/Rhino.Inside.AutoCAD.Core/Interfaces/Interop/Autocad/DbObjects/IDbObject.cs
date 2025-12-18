namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which wraps the Autodesk.AutoCAD.DatabaseServices.DBObject class.
/// </summary>
public interface IDbObject : IDisposable
{
    /// <summary>
    /// The <see cref="IObjectId"/> of the <see cref="IDbObject"/>.
    /// </summary>
    IObjectId Id { get; }

    /// <summary>
    /// The <see cref="Type"/> of the underlying AutoCAD DBObject.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Checks if the <see cref="IDbObject"/> is valid.
    /// </summary>
    /// <remarks>
    /// Returns true if the entity is valid, otherwise
    /// returns false.
    /// </remarks>
    bool IsValid { get; }

    /// <summary>
    /// Creates a shallow clone of the <see cref="IDbObject"/>. The autocad
    /// references are the same, but a new wrapper is created.
    /// </summary>
    IDbObject ShallowClone();
}