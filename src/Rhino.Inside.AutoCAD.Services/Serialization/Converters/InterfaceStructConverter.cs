using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// JSON converter that deserializes interface-typed properties into concrete struct implementations.
/// </summary>
/// <typeparam name="TStruct">
/// The concrete struct type to instantiate during deserialization. Must implement <typeparamref name="TInterface"/>.
/// </typeparam>
/// <typeparam name="TInterface">
/// The interface type that <typeparamref name="TStruct"/> implements.
/// </typeparam>
/// <remarks>
/// This converter is used by the <see cref="JsonSerializer"/> to deserialize JSON data into value types
/// when the target property is declared as an interface. It is instantiated automatically by
/// <see cref="InterfaceConverterFactory"/> when the concrete type is a struct.
/// Serialization (writing) is not implemented as interface-typed values are typically read-only in configuration scenarios.
/// </remarks>
/// <seealso cref="InterfaceConverterFactory"/>
/// <seealso cref="InterfaceConverter{TClass, TInterface}"/>
public class InterfaceStructConverter<TStruct, TInterface> : JsonConverter<TInterface> where TStruct : struct, TInterface
{
    /// <inheritdoc/>
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<TStruct>(ref reader, options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options) { }
}