namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Event Args for when a <see cref="IAutocadDocument"/> is changed.
/// </summary>
public interface IAutocadDocumentChangeEventArgs
{
    /// <summary>
    /// The modeled change. This contains information about the type
    /// of changes and the objects which they changes affect
    /// </summary>
    IAutocadDocumentChange Change { get; }
}