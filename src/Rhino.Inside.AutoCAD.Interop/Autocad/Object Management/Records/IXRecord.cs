using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IXRecord"/>
public class XRecord : IXRecord
{
    private readonly Dictionary<GroupCodeValue, IList<ITypedValue>> _dataTagsDictionary;

    /// <inheritdoc/>
    public string Key { get; }

    /// <summary>
    /// Creates a new <see cref="IXRecord"/>.
    /// </summary>
    public XRecord(string key)
    {
        _dataTagsDictionary = new Dictionary<GroupCodeValue, IList<ITypedValue>>();

        this.Key = key;
    }

    /// <summary>
    /// Adds the <paramref name="typedValue"/> to the <see cref="_dataTagsDictionary"/>.
    /// </summary>
    private void AddToDictionary(ITypedValue typedValue)
    {
        var groupCode = typedValue.GroupCode;

        if (_dataTagsDictionary.ContainsKey(groupCode) == false)
            _dataTagsDictionary.Add(groupCode, new List<ITypedValue>());

        _dataTagsDictionary[groupCode].Add(typedValue);
    }

    /// <inheritdoc/>
    public void Add(GroupCodeValue groupCode, object value)
    {
        var dataTag = new TypedValue(groupCode, value);

        this.AddToDictionary(dataTag);
    }

    /// <inheritdoc/>
    public void Replace(GroupCodeValue groupCode, object value)
    {
        var dataTag = new TypedValue(groupCode, value);

        if (_dataTagsDictionary.ContainsKey(groupCode))
            _dataTagsDictionary.Remove(groupCode);

        this.AddToDictionary(dataTag);
    }

    /// <inheritdoc/>
    public void Clear() => _dataTagsDictionary.Clear();

    /// <inheritdoc/>
    public bool IsEmpty() => _dataTagsDictionary.Any() == false;

    /// <inheritdoc/>
    public bool ContainsKey(GroupCodeValue groupCode)
    {
        return _dataTagsDictionary.ContainsKey(groupCode);
    }

    /// <inheritdoc/>
    public bool ContainsValue(GroupCodeValue groupCode, object value)
    {
        if (this.ContainsKey(groupCode) == false) return false;

        var dataTags = this.GetAt(groupCode);

        var values = dataTags.Select(d => d.Value);

        return values.Contains(value);
    }

    /// <inheritdoc/>
    public IList<ITypedValue> GetAt(GroupCodeValue groupCode)
    {
        var exists = _dataTagsDictionary.TryGetValue(groupCode, out var dataTags);

        return exists ? dataTags! : new List<ITypedValue>();
    }

    /// <inheritdoc/>
    public TValue GetFirstValueAt<TValue>(GroupCodeValue groupCode)
    {
        var tags = this.GetAt(groupCode);

        return tags.Count > 0 ? (TValue)Convert.ChangeType(tags.First().Value, typeof(TValue)) : default!;
    }

    /// <inheritdoc/>
    public IEnumerator<ITypedValue> GetEnumerator()
    {
        foreach (var dataTags in _dataTagsDictionary.Values)
        {
            foreach (var dataTag in dataTags)
            {
                yield return dataTag;
            }
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}