using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectIdTagDatabase"/>
public class ObjectIdTagDatabase : IObjectIdTagDatabase
{
    private readonly Dictionary<string, IObjectIdTagRecord> _tagRecords;

    /// <inheritdoc/>
    public string Key { get; }

    /// <summary>
    /// Constructs a new <see cref="ObjectIdTagDatabase"/>.
    /// </summary>
    public ObjectIdTagDatabase(string key)
    {
        _tagRecords = new Dictionary<string, IObjectIdTagRecord>();

        this.Key = key;
    }
    
    /// <inheritdoc/>
    public IObjectIdTagRecord GetTagRecord(string name)
    {
        if (_tagRecords.TryGetValue(name, out var tagRecord) == false)
        {
            tagRecord = new ObjectIdTagRecord(name);

            _tagRecords[name] = tagRecord;
        }

        return tagRecord;
    }

    /// <inheritdoc/>
    public IEnumerator<IObjectIdTagRecord> GetEnumerator()
    {
        foreach (var tagCollection in _tagRecords.Values)
        {
            yield return tagCollection;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}