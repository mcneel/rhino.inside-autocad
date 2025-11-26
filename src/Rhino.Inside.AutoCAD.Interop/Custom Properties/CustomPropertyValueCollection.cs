using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="ICustomPropertyValueCollection"/>
public class CustomPropertyValueCollection : ICustomPropertyValueCollection
{
    private readonly Dictionary<CustomPropertyType, ICustomPropertyValue> _customPropertyValues = new();
    private readonly CustomPropertyType _noneType = CustomPropertyType.None;

    ///<inheritdoc />
    public ICustomPropertyValue this[CustomPropertyType type] =>
        _customPropertyValues.TryGetValue(type, out var value)
            ? value : new CustomPropertyValue(_noneType, string.Empty);

    /// <summary>
    /// Constructs a new instance of <see cref="CustomPropertyValueCollection"/>.
    /// </summary>
    public CustomPropertyValueCollection() { }

    ///<inheritdoc />
    public void Add(ICustomPropertyValue value)
    {
        _customPropertyValues[value.Type] = value;
    }

    ///<inheritdoc />
    public void Add(IList<ICustomPropertyValue> values)
    {
        foreach (var value in values)
        {
            this.Add(value);
        }
    }

    ///<inheritdoc />
    public IEnumerator<ICustomPropertyValue> GetEnumerator() => _customPropertyValues.Values.GetEnumerator();

    ///<inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}