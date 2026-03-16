namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped AutoCAD DBDictionary object.
/// </summary>
public interface IAutocadDictionary : IDbObject
{
    /// <summary>
    /// Gets the keys contained in the dictionary.
    /// </summary>
    IReadOnlyList<string> Keys { get; }

    /// <summary>
    /// Gets the number of entries in the dictionary.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the name of the dictionary, if available.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Determines whether the dictionary contains an entry with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <returns>True if the dictionary contains an entry with the specified key; otherwise, false.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Attempts to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, null.</param>
    /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
    bool TryGetValue(string key, out IDbObject? value);

    /// <summary>
    /// Gets all entries in the dictionary as key-value pairs.
    /// </summary>
    /// <returns>A read-only list of key-value pairs representing all dictionary entries.</returns>
    IReadOnlyList<KeyValuePair<string, IDbObject>> GetAllEntries();

    /// <summary>
    /// Creates a shallow clone of the <see cref="IAutocadDictionary"/>.
    /// </summary>
    new IAutocadDictionary ShallowClone();
}