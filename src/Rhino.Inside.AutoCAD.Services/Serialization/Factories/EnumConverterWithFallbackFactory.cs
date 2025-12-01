using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A factory for creating JSON converters that provide fallback support
/// for enums when deserialization fails.
/// </summary>
public class EnumConverterWithFallbackFactory : JsonConverterFactory
{
    private readonly JsonStringEnumConverter _underlying = new();

    /// <summary>
    /// Determines whether the specified type can be converted by this factory.
    /// </summary>
    /// <param name="enumType">The type to check for conversion support.</param>
    /// <returns><c>true</c> if the type can be converted; otherwise, <c>false</c>.</returns>
    public sealed override bool CanConvert(Type enumType)
        => _underlying.CanConvert(enumType);

    /// <summary>
    /// Creates a JSON converter for the specified enum type.
    /// </summary>
    /// <param name="enumType">The type of the enum to create a converter for.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>A <see cref="JsonConverter"/> instance for the specified enum type.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="enumType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the converter cannot be created for the specified type.
    /// </exception>
    public sealed override JsonConverter CreateConverter(Type enumType, JsonSerializerOptions options)
    {
        var underlyingConverter = _underlying.CreateConverter(enumType, options);
        var converterType = typeof(EnumConverterWithFallback<>).MakeGenericType(enumType);
        return (JsonConverter)Activator.CreateInstance(converterType, underlyingConverter);
    }
}