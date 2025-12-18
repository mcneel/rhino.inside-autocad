namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides the interface for a generic repository pattern.
/// </summary>
public interface IRepository<T> : IDisposable
{
    /// <summary>
    /// Attempts to return an instance of type <typeparamref name="T"/> by name.
    /// Returns true if the instance is found, otherwise returns false.
    /// </summary>
    bool TryGetByName(string name, out T? value);

    /// <summary>
    /// Populates this <see cref="IRepository{T}"/> from the active <see
    /// cref="IAutocadDocument"/>. This will clear the existing items in the repository
    /// and repopulate it.
    /// </summary>
    void Repopulate();
}