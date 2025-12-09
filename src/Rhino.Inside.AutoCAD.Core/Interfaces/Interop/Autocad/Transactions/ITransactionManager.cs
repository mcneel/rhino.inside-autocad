namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Serves as a wrapper around the AutoCAD TransactionManager.
/// </summary>
/// <remarks>
/// The <see cref="ITransactionManager"/>s primary purpose is to provide a
/// bridge between the AutoCAD API and the AWI API. It enables the AWI API
/// to be decoupled from AutoCAD (no AutoCAD types are exposed inRhino.Inside.AutoCAD.Core),
/// and serves as the main entry point to the AutoCAD API when it needs to
/// be called.
/// </remarks>
public interface ITransactionManager : IDisposable
{

    /// <summary>
    /// Returns the <see cref="IBlockTableRecord"/>. Input true to
    /// open the block table record for write purposes.
    /// </summary>
    IBlockTableRecord GetModelSpaceBlockTableRecord(bool openForWrite = false);
}