using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A JSON converter for enums that provides a fallback to the default value
/// when deserialization fails.
/// </summary>
/// <typeparam name="T">The enum type to be converted.</typeparam>
public class EnumConverterWithFallback<T> : JsonConverter<T> where T : struct, Enum
{
    private readonly JsonConverter<T> _underlying;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumConverterWithFallback{T}"/> class.
    /// </summary>
    /// <param name="underlying">The underlying JSON converter for the enum type.</param>
    public EnumConverterWithFallback(JsonConverter<T> underlying)
        => _underlying = underlying;

    /// <summary>
    /// Reads and converts the JSON to the specified enum type.
    /// Falls back to the default value of the enum if deserialization fails.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader.</param>
    /// <param name="enumType">The type of the enum to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The deserialized enum value, or the default value if deserialization fails.</returns>
    public override T Read(ref Utf8JsonReader reader, Type enumType, JsonSerializerOptions options)
    {
        try
        {
            return _underlying.Read(ref reader, enumType, options);
        }
        catch (JsonException) when (enumType.IsEnum)
        {
            return default;
        }
    }

    /// <summary>
    /// Determines whether the specified type can be converted.
    /// </summary>
    /// <param name="typeToConvert">The type to check for conversion support.</param>
    /// <returns><c>true</c> if the type can be converted; otherwise, <c>false</c>.</returns>
    public override bool CanConvert(Type typeToConvert)
        => _underlying.CanConvert(typeToConvert);

    /// <summary>
    /// Writes the specified enum value as JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer.</param>
    /// <param name="value">The enum value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => _underlying.Write(writer, value, options);

    /// <summary>
    /// Reads and converts the JSON property name to the specified enum type.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader.</param>
    /// <param name="typeToConvert">The type of the enum to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The deserialized enum value.</returns>
    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => _underlying.ReadAsPropertyName(ref reader, typeToConvert, options);

    /// <summary>
    /// Writes the specified enum value as a JSON property name.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer.</param>
    /// <param name="value">The enum value to write as a property name.</param>
    /// <param name="options">The serializer options.</param>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => _underlying.WriteAsPropertyName(writer, value, options);
}