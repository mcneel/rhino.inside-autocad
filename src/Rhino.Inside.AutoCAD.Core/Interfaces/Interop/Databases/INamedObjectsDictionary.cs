namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A wrapper around the AutoCAD <see cref="IDatabase"/> NamedObjectsDictionary.
/// </summary>
public interface INamedObjectsDictionary : IDisposable
{
    /// <summary>
    /// Attempts to obtain the <see cref="IObjectId"/> using the
    /// <paramref name="key"/> provided. True if the value is found,
    /// otherwise false.
    /// </summary>
    bool TryGetValue(string key, out IObjectId? value);

    /// <summary>
    /// Upgrades this <see cref="INamedObjectsDictionary"/> to write, enabling
    /// modification to its contents.
    /// </summary>
    void UpgradeOpen();
}