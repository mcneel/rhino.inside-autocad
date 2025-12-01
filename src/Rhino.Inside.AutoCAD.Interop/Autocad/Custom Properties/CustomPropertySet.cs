using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc />
public class CustomPropertySet : ICustomPropertySet
{
    private readonly Dictionary<CustomPropertyType, ICustomProperty> _customProperties = new();

    /// <summary>
    /// Constructs a new instance of <see cref="CustomPropertySet"/>.
    /// </summary>
    public CustomPropertySet() { }

    public int Count => _customProperties.Count;

    /// <inheritdoc />
    public void Add(ICustomProperty property)
    {
        if (property.IsValid == false) return;

        _customProperties[property.Type] = property;
    }

    /// <inheritdoc />
    public bool TryGet(CustomPropertyType type, out ICustomProperty property)
    {
        return _customProperties.TryGetValue(type, out property);
    }

    /// <inheritdoc />
    public IEnumerator<ICustomProperty> GetEnumerator() =>
        _customProperties.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}