using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Represents an entry in an AutoCAD dictionary, containing a key and its associated value.
/// </summary>
public class DictionaryEntry : IDictionaryEntry
{
    /// <inheritdoc/>
    public string Key { get; }

    /// <inheritdoc/>
    public IDbObject Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryEntry"/> class.
    /// </summary>
    /// <param name="key">The key of the dictionary entry.</param>
    /// <param name="value">The value of the dictionary entry.</param>
    public DictionaryEntry(string key, IDbObject value)
    {
        this.Key = key;
        this.Value = value;
    }
}