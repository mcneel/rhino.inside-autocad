namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An extendable repository which can import instances of its type
/// <typeparamref name="T"/> from an <see cref="IExternalDatabase"/>.
/// </summary>
public interface IExtendableRepository<T> : IRepository<T>
{
    /// <summary>
    /// Attempts to import an instance of type <typeparamref name="T"/> from the
    /// <paramref name="externalDatabase"/> using the <paramref name="lookupName"/>.
    /// Returns true if the instance was successfully cloned, otherwise returns false.
    /// </summary>
    bool TryImportByName(IExternalDatabase externalDatabase, string lookupName, out T? blockTableRecord);
}