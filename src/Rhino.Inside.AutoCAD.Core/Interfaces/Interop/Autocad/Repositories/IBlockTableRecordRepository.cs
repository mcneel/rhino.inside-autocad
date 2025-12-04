namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A repository storing all <see cref="IBlockTableRecord"/>s
/// in the active <see cref="IDocument"/>.
/// </summary>
public interface IBlockTableRecordRepository : IExtendableRepository<IBlockTableRecord>, IEnumerable<IBlockTableRecord>
{
    /// <summary>
    /// Register a new <see cref="IBlockTableRecord"/> to the repository.
    /// </summary>
    void Register(IBlockTableRecord blockTableRecord);
}