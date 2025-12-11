using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc />
public class DynamicPropertySet : IDynamicPropertySet
{
    private readonly Dictionary<string, IDynamicBlockReferencePropertyWrapper> _dynamicProperties = new();

    /// <summary>
    /// Constructs a new instance of <see cref="DynamicPropertySet"/>.
    /// </summary>
    public DynamicPropertySet() { }

    public int Count => _dynamicProperties.Count;

    /// <inheritdoc />
    public void Add(IDynamicBlockReferencePropertyWrapper property)
    {
        _dynamicProperties[property.Name] = property;
    }

    /// <inheritdoc />
    public bool TryGet(string name, out IDynamicBlockReferencePropertyWrapper property)
    {
        return _dynamicProperties.TryGetValue(name, out property);
    }

    /// <inheritdoc />
    public IEnumerator<IDynamicBlockReferencePropertyWrapper> GetEnumerator() =>
        _dynamicProperties.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}