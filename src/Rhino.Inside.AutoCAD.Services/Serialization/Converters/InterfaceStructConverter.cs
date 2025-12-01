using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A <see cref="JsonConverter"/> which converts an interface type into a concrete type.
/// </summary>
public class InterfaceStructConverter<TStruct, TInterface> : JsonConverter<TInterface> where TStruct : struct, TInterface
{
    /// <summary>
    /// Reads the JSON token and converts the value into the concrete struct type
    /// and returns it as its interface type.
    /// </summary>
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<TStruct>(ref reader, options);
    }

    /// <summary>
    /// Writes a concrete type as an interface type to JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options) { }
}