using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc />
public class BlockAttributeSet : IBlockAttributeSet
{
    private readonly Dictionary<string, IAttributeWrapper> _attributes = new();

    /// <summary>
    /// Constructs a new instance of <see cref="BlockAttributeSet"/>.
    /// </summary>
    public BlockAttributeSet() { }

    public int Count => _attributes.Count;

    /// <inheritdoc />
    public void Add(IAttributeWrapper property)
    {
        _attributes[property.Tag] = property;
    }

    /// <inheritdoc />
    public bool TryGet(string name, out IAttributeWrapper property)
    {
        return _attributes.TryGetValue(name, out property);
    }

    /// <inheritdoc />
    public IEnumerator<IAttributeWrapper> GetEnumerator() =>
        _attributes.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}