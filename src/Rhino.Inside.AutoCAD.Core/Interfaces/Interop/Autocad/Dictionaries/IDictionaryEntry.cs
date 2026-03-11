namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an entry in an AutoCAD dictionary, containing a key and its associated value.
/// </summary>
public interface IDictionaryEntry
{
    /// <summary>
    /// Gets the key of the dictionary entry.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the value of the dictionary entry.
    /// </summary>
    IDbObject Value { get; }
}