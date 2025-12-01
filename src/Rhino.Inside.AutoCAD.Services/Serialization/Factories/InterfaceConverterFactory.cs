using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A <see cref="JsonConverterFactory"/> which is used to inject concrete
/// types into interface declarations for the <see cref="JsonSerializer"/>.
/// </summary>
public class InterfaceConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// The concrete type to inject.
    /// </summary>
    public Type ConcreteType { get; }

    /// <summary>
    /// The interface which the <see cref="ConcreteType"/> implements.
    /// </summary>
    public Type InterfaceType { get; }

    /// <summary>
    /// Creates a new <see cref="InterfaceConverterFactory"/>.
    /// </summary>
    public InterfaceConverterFactory(Type concrete, Type interfaceType)
    {
        this.ConcreteType = concrete;
        this.InterfaceType = interfaceType;
    }

    /// <summary>
    /// Returns true if the <see cref="ConcreteType"/> is a <see cref="InterfaceType"/>.
    /// </summary>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == this.InterfaceType;
    }

    /// <summary>
    /// Instantiates the required <see cref="JsonConverter"/> to deserialize
    /// to an interface type using a concretion. 
    /// </summary>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = this.ConcreteType.IsValueType ?
            typeof(InterfaceStructConverter<,>).MakeGenericType(this.ConcreteType, this.InterfaceType) :
            typeof(InterfaceConverter<,>).MakeGenericType(this.ConcreteType, this.InterfaceType);

        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}