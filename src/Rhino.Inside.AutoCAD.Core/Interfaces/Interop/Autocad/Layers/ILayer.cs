namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped AutoCAD LayerTableRecord
/// </summary>
public interface ILayer : IDisposable
{
    /// <summary>
    /// The <see cref="IColor"/> of the
    /// <see cref="ILayer"/>.
    /// </summary>
    IColor Color { get; }

    /// <summary>
    /// The <see cref="IObjectId"/> of the <see cref="ILayer"/>.
    /// </summary>
    IObjectId Id { get; }

    /// <summary>
    /// The <see cref="ILinePattern"/> assigned to the <see cref="ILayer"/>.
    /// </summary>
    ILinePattern LinePattern { get; }

    /// <summary>
    /// The name of the <see cref="ILayer"/>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns true if the  <see cref="ILayer"/> is
    /// valid, otherwise returns false.
    /// </summary>>
    bool IsValid { get; }

    /// <summary>
    /// Returns true if the <see cref="ILayer"/> is locked, and false otherwise.
    /// </summary>
    /// <remarks>
    /// A locked <see cref="ILayer"/> restricts any modifications to the
    /// <see cref="IEntity"/>s within it, making them as read-only.
    /// </remarks>
    bool IsLocked { get; }
}