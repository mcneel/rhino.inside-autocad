namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a unique identifier for an AutoCAD document, stored in model space XData.
/// This object encapsulates the logic for retrieving or creating a persistent GUID
/// associated with the document, ensuring that it remains consistent across sessions
/// and can be used to track the document. It handles the registration of the
/// application name in the RegAppTable and manages the storage and retrieval of the
/// GUID in the model space XData.
/// </summary>
public interface IAutocadDocumentId
{
    /// <summary>
    /// The unique identifier for the AutoCAD document, stored in model space XData.
    /// </summary>
    Guid Id { get; }
}