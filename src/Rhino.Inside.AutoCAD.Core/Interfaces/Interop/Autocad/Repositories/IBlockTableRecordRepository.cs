namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A repository storing all <see cref="IBlockTableRecord"/>s
/// in the active <see cref="IAutocadDocument"/>.
/// </summary>
public interface IBlockTableRecordRepository :
    IExtendableRepository<IBlockTableRecord>, IEnumerable<IBlockTableRecord>
{
    /// <summary>
    /// Event raised when the Block table (A table of all <see cref="IBlockTableRecord"/>s
    /// is modified.
    /// </summary>
    event EventHandler? BlockTableModified;

    /// <summary>
    /// Register a new <see cref="IBlockTableRecord"/> to the repository.
    /// </summary>
    void Register(IBlockTableRecord blockTableRecord);

    /// <summary>
    /// Tries to get a <see cref="IBlockTableRecord"/> by its Id.
    /// </summary>
    bool TryGetById(IObjectId id, out IBlockTableRecord? blockTableRecord);
}