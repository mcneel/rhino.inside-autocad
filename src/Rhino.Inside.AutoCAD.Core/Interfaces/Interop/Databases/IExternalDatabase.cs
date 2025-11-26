namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Opens an external database with a <see cref="Transaction{T}"/> to perform
/// queries and obtain objects from it.
/// </summary>
public interface IExternalDatabase : IDisposable
{
    /// <summary>
    /// Returns this <see cref="IDatabase"/> instance.
    /// </summary>
    IDatabase Database { get; }

    /// <summary>
    /// Attempts to get a <see cref="IBlockTableRecord"/> from the external database
    /// that matches the given <paramref name="blockName"/>. Returns true if the block
    /// exist, otherwise returns false.
    /// </summary>
    bool TryGetBlockRecord(string blockName, out IBlockTableRecord blockTableRecord);

    /// <summary>
    /// Execute an AutoCAD transaction inside this <see cref="IExternalDatabase"/>
    /// and returns the result <typeparamref name="T"/>. If <paramref name="abort"/>;
    /// is set to true aborts the transaction to roll back any changes - this is
    /// useful when the transaction is being used to read data from the database
    /// that requires a change to obtain it. By default, all transactions are committed.
    /// </summary>
    T Transaction<T>(Func<ITransactionManager, T> function, bool abort = false);
}