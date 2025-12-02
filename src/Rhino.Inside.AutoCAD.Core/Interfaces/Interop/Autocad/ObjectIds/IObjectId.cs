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
}