using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDictionary"/>
public class AutocadDictionaryWrapper : AutocadDbObjectWrapper, IAutocadDictionary
{
    private readonly DBDictionary _dictionary;
    private readonly List<string> _keys;

    /// <inheritdoc/>
    public IReadOnlyList<string> Keys => _keys;

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <summary>
    /// Constructs a new <see cref="AutocadDictionaryWrapper"/>.
    /// </summary>
    public AutocadDictionaryWrapper(DBDictionary dictionary) : base(dictionary)
    {
        _dictionary = dictionary;
        _keys = this.CacheKeys(dictionary);

        this.Name = this.GetName(dictionary);
    }

    /// <summary>
    /// Gets the name of the dictionary from its owner dictionary if it exists.
    /// </summary>
    private string? GetName(DBDictionary dictionary)
    {
        try
        {
            if (!dictionary.Id.IsValid || dictionary.Id.IsNull || !dictionary.OwnerId.IsValid) return null;

            using var transaction = dictionary.Database?.TransactionManager.StartTransaction();

            var owner = transaction.GetObject(dictionary.OwnerId, OpenMode.ForRead) as DBDictionary;

            string? name = null;
            if (owner != null)
            {
                foreach (var entry in owner)
                {
                    if (entry.Value != dictionary.Id) continue;

                    name = entry.Key;

                }
            }
            transaction.Commit();

            return name;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Caches the keys from the dictionary at construction time for thread safety.
    /// </summary>
    private List<string> CacheKeys(DBDictionary dictionary)
    {
        var keys = new List<string>();

        foreach (var entry in dictionary)
        {
            keys.Add(entry.Key);
        }

        return keys;
    }

    /// <summary>
    /// Wraps an AutoCAD DBObject in the appropriate wrapper type.
    /// </summary>
    private IDbObject? WrapObject(DBObject dbObject)
    {
        return dbObject switch
        {
            DBDictionary nestedDict => new AutocadDictionaryWrapper(nestedDict),
            Xrecord xrecord => new XRecordWrapper(xrecord),
            _ => new AutocadDbObjectWrapper(dbObject)
        };
    }

    /// <inheritdoc/>
    public bool ContainsKey(string key)
    {
        return _dictionary.Contains(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(string key, out IDbObject? value)
    {
        value = null;

        if (!_dictionary.Contains(key))
            return false;

        try
        {
            var objectId = _dictionary.GetAt(key);
            if (!objectId.IsValid || objectId.IsNull)
                return false;

            using var transaction = _dictionary.Database?.TransactionManager.StartTransaction();
            if (transaction == null)
                return false;

            var dbObject = transaction.GetObject(objectId, OpenMode.ForRead);
            value = this.WrapObject(dbObject);
            transaction.Commit();

            return value != null;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<KeyValuePair<string, IDbObject>> GetAllEntries()
    {
        var entries = new List<KeyValuePair<string, IDbObject>>();

        try
        {
            using var transaction = _dictionary.Database?.TransactionManager.StartTransaction();
            if (transaction == null)
                return entries;

            foreach (var entry in _dictionary)
            {
                if (!entry.Value.IsValid || entry.Value.IsNull)
                    continue;

                var dbObject = transaction.GetObject(entry.Value, OpenMode.ForRead);
                var wrappedObject = this.WrapObject(dbObject);

                if (wrappedObject != null)
                {
                    entries.Add(new KeyValuePair<string, IDbObject>(entry.Key, wrappedObject));
                }
            }

            transaction.Commit();
        }
        catch
        {
            // Return whatever entries we've collected
        }

        return entries;
    }

    /// <inheritdoc/>
    public new IAutocadDictionary ShallowClone()
    {
        return new AutocadDictionaryWrapper(_dictionary);
    }
}
