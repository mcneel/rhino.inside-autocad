using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// Factory that creates JSON converters for deserializing interface types into their concrete implementations.
/// </summary>
/// <remarks>
/// This factory enables the <see cref="JsonSerializer"/> to deserialize JSON data directly into interface-typed
/// properties by mapping them to their concrete implementations at runtime. The factory automatically selects
/// <see cref="InterfaceConverter{TClass, TInterface}"/> for reference types or
/// <see cref="InterfaceStructConverter{TStruct, TInterface}"/> for value types based on the concrete type provided.
/// </remarks>
/// <seealso cref="InterfaceConverter{TClass, TInterface}"/>
/// <seealso cref="InterfaceStructConverter{TStruct, TInterface}"/>
public class InterfaceConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// The concrete implementation type used during deserialization.
    /// </summary>
    /// <remarks>
    /// This type must implement <see cref="Interface"/>. When deserializing, JSON data will be
    /// converted to an instance of this type and returned as the interface type.
    /// </remarks>
    public Type Concrete { get; }

    /// <summary>
    /// The interface type that <see cref="Concrete"/> implements.
    /// </summary>
    /// <remarks>
    /// This is the type that the <see cref="JsonSerializer"/> will recognize and delegate to this factory
    /// when encountered during deserialization.
    /// </remarks>
    public Type Interface { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterfaceConverterFactory"/> class.
    /// </summary>
    /// <param name="concrete">
    /// The concrete type to instantiate during deserialization.
    /// </param>
    /// <param name="interfaceType">
    /// The interface type that <paramref name="concrete"/> implements.
    /// </param>
    public InterfaceConverterFactory(Type concrete, Type interfaceType)
    {
        this.Concrete = concrete;
        this.Interface = interfaceType;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == this.Interface;
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = this.Concrete.IsValueType ?
            typeof(InterfaceStructConverter<,>).MakeGenericType(this.Concrete, this.Interface) :
            typeof(InterfaceConverter<,>).MakeGenericType(this.Concrete, this.Interface);

        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}