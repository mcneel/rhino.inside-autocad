using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ICustomPropertyValue"/>
public class CustomPropertyValue : ICustomPropertyValue
{
    ///<inheritdoc />
    public CustomPropertyType Type { get; }

    ///<inheritdoc />
    public object Value { get; }

    /// <summary>
    /// Constructs a new <see cref="CustomPropertyValue"/>.
    /// </summary>
    public CustomPropertyValue(CustomPropertyType type, object value)
    {
        this.Type = type;

        this.Value = value;
    }

    ///<inheritdoc />
    public bool IsEqualTo(ICustomPropertyValue other)
    {
        return this.Type == other.Type &&
               this.Value.Equals(other.Value);
    }
}