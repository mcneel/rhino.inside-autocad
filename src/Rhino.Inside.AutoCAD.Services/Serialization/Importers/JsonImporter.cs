using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IJsonResourceImporter"/>
public class JsonResourceImporter : IJsonResourceImporter
{
    private readonly IApplicationDirectories _applicationDirectories;

    /// <summary>
    /// Constructs a new <see cref="JsonImporter"/>.
    /// </summary>
    public JsonResourceImporter(IApplicationDirectories applicationDirectories)
    {
        _applicationDirectories = applicationDirectories;
    }

    /// <inheritdoc/>
    public IList<T> ImportListOf<T>(List<JsonConverter> converters, IDataFileName fileName) where T : class
    {
        var importFilePath = $"{_applicationDirectories.Resources}{fileName.FileName}";

        var emptyList = new List<T>();

        if (File.Exists(importFilePath) == false)
            return emptyList;

        using var jsonFileStream = File.OpenRead(importFilePath);

        var serializerOptions = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        converters.ForEach(serializerOptions.Converters.Add);

        var importedTypes = JsonSerializer.Deserialize<IList<T>>(jsonFileStream, serializerOptions);

        return importedTypes ?? emptyList;
    }

    /// <inheritdoc/>
    public bool TryImportInstance<T>(List<JsonConverter> converters, IDataFileName fileName, out T? instance) where T : class
    {
        var importFilePath = $"{_applicationDirectories.Resources}{fileName.FileName}";

        if (File.Exists(importFilePath) == false)
        {
            instance = default;

            return false;
        }

        using var jsonFileStream = File.OpenRead(importFilePath);

        var serializerOptions = new JsonSerializerOptions();
        converters.ForEach(serializerOptions.Converters.Add);

        instance = JsonSerializer.Deserialize<T>(jsonFileStream, serializerOptions);

        return instance != null;
    }
}