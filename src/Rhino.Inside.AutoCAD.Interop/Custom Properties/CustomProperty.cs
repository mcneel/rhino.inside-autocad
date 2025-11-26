using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ICustomProperty"/>
public class CustomProperty : ICustomProperty
{
    ///<inheritdoc/>
    public ICustomPropertyName Name { get; }

    ///<inheritdoc/>
    public ICustomPropertyValue Value { get; private set; }

    ///<inheritdoc/>
    public CustomPropertyType Type => this.IsValid
        ? this.Name.Type : CustomPropertyType.None;

    ///<inheritdoc/>
    public bool IsValid => this.Name.Type == this.Value.Type;

    /// <summary>
    /// Constructs a new <see cref="CustomProperty"/>.
    /// </summary>
    public CustomProperty(ICustomPropertyName name, ICustomPropertyValue value)
    {
        this.Name = name;

        this.Value = value;
    }

    ///<inheritdoc/>
    public void SetValue(ICustomPropertyValue value)
    {
        this.Value = value;
    }
}