using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ICustomPropertyName"/>
public class CustomPropertyName : ICustomPropertyName
{
    ///<inheritdoc />
    public CustomPropertyType Type { get; }

    ///<inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Constructs a new <see cref="CustomPropertyName"/>.
    /// </summary>
    public CustomPropertyName(CustomPropertyType type, string name)
    {
        this.Type = type;

        this.Name = name;
    }
}