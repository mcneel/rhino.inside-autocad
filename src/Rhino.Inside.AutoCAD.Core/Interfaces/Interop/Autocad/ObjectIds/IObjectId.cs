namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which wraps the AutoCAD <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/>.
/// </summary>
public interface IObjectId
{
    /// <summary>
    /// The <see cref="IObjectId"/> value.
    /// </summary>
    long Value { get; }

    /// <summary>
    /// Returns true if the <see cref="IObjectId"/> is valid, otherwise
    /// returns false.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Returns true if the <see cref="IObjectId"/> is erased, otherwise
    /// returns false.
    /// </summary>
    bool IsErased { get; }

    /// <summary>
    /// Creates a shallow clone of the <see cref="IObjectId"/>.
    /// </summary>
    IObjectId ShallowClone();

    /// <summary>
    /// Determines whether the current <see cref="IObjectId"/> is equal to another <see cref="IObjectId"/>.
    /// </summary>
    bool IsEqualTo(IObjectId other);
}