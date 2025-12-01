using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents inputs that can be configured by the user via
/// the UI. Such inputs are imported and exported to JSON and stored local to the
/// <see cref="IDocument"/> in the Resources folder.
/// </summary>
public interface IDataCaptureCache
{
    /// <summary>
    /// Imports a list of <typeparamref name="T"/> objects from the <see cref=
    /// "IDataFileName"/>. The method searches the <see cref="IAutocadDocumentFileInfo
    /// .ResourceDirectory"/> first and if the JSON file is not found, defaults
    /// to the <see cref="IApplicationDirectories.Resources"/> directory to perform
    /// the search. Returns an empty list if no JSON file is found. Only use this
    /// method if the JSON file is stored in the <see cref="IAutocadDocumentFileInfo
    /// .ResourceDirectory"/>, i.e. a project-specific data file, such as a JSON
    /// file.
    /// </summary>
    IList<T> ImportProjectResourceListOf<T>(List<JsonConverter> converters, IDataFileName fileName) where T : class;

    /// <summary>
    /// Exports the inputs to the JSON cache file.
    /// </summary>
    void Export();
}