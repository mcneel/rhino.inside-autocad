using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines an importer with a method for importing a list of
/// any object type.
/// </summary>
public interface IJsonResourceImporter
{
    /// <summary>
    /// Imports a list of <typeparamref name="T"/> from the named JSON
    /// <paramref name="fileName"/> in the <see cref="IApplicationDirectories.Resources"/>
    /// folder or an empty list if the JSON file doesn't exist or fails to deserialize. 
    /// </summary>
    /// <remarks>
    /// The file name must include its extension.
    /// </remarks>
    IList<T> ImportListOf<T>(List<JsonConverter> converters, IDataFileName fileName) where T : class;

    /// <summary>
    /// Attempts to import an instance of type <typeparamref name="T"/> from the
    /// named JSON <paramref name="fileName"/> in the <see cref="IApplicationDirectories.Resources"/>
    /// folder. Returns true if successful, otherwise false. 
    /// </summary>
    bool TryImportInstance<T>(List<JsonConverter> converters, IDataFileName fileName, out T? instance) where T : class;
}