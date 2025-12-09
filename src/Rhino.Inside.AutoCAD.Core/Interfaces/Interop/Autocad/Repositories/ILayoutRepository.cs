namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A repository storing all <see cref="IAutocadLayout"/>s
/// in the active <see cref="IAutocadDocument"/>.
/// </summary>
public interface ILayoutRepository : IRepository<IAutocadLayout>, IEnumerable<IAutocadLayout>
{
    /// <summary>
    /// Updates the <see cref="ILayoutRepository"/>> with the <see cref="IAutocadLayout"/>s per
    /// the current state of the active <see cref="IAutocadDocument"/>.
    /// </summary>
    void Populate();

    /// <summary>
    /// Tries to add a new <see cref="IAutocadLayout"/> to the repository.
    /// </summary>
    bool TryAddLayout(string name, out IAutocadLayout? layout);
}