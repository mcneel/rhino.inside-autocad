using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IDimensionStyleTableRecordRepository"/>
public class DimensionStyleTableRecordRepository : Disposable, IDimensionStyleTableRecordRepository
{
    private readonly Dictionary<string, IDimensionStyleTableRecord> _dimStyleTableRecords = new();
    private readonly IAutocadDocument _document;

    /// <summary>
    /// Constructs a new <see cref="DimensionStyleTableRecordRepository"/>.
    /// </summary>
    public DimensionStyleTableRecordRepository(IAutocadDocument document)
    {
        _document = document;

        this.Repopulate();
    }

    ///<inheritdoc />
    public IDimensionStyleTableRecord GetDefault() =>
        _dimStyleTableRecords.Values.First();

    /// <summary>
    /// Updates this repository.
    /// </summary>
    public void Repopulate()
    {
        _dimStyleTableRecords.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var database = _document.Database.Unwrap();

            using var dimStyleTable = (DimStyleTable)transactionManager.GetObject(database.DimStyleTableId, OpenMode.ForRead);

            foreach (var dimStyleId in dimStyleTable)
            {
                var dimStyle = (DimStyleTableRecord)transactionManager.GetObject(dimStyleId, OpenMode.ForRead);

                var dimStyleTableRecordWrapper = new DimensionStyleTableRecord(dimStyle);

                var dimStyleName = dimStyle.Name;

                if (_dimStyleTableRecords.ContainsKey(dimStyleName) == false)
                    _dimStyleTableRecords[dimStyleName] = dimStyleTableRecordWrapper;
            }

            return true;
        });
    }

    ///<inheritdoc />
    public bool TryGetByName(string name, out IDimensionStyleTableRecord? dimStyleTableRecord) =>
        _dimStyleTableRecords.TryGetValue(name, out dimStyleTableRecord);

    /// <summary>
    /// Disposes the <see cref="DimensionStyleTableRecordRepository"/> and all its
    /// <see cref="IDimensionStyleTableRecord"/>s.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            foreach (var dimensionStyleTableRecord in _dimStyleTableRecords.Values)
                dimensionStyleTableRecord.Dispose();

            _disposed = true;
        }
    }
}