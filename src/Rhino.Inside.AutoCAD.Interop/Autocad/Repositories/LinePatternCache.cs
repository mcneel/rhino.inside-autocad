using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autofac.Util;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ILinePatternCache"/>
public class LinePatternCache : Disposable, ILinePatternCache
{
    private bool _disposed;

    private readonly Dictionary<IObjectId, IAutocadLinePattern> _linePatterns;
    private readonly IAutocadLinePattern _continuousLinePattern;

    /// <summary>
    /// Constructs a new <see cref="LinePatternCache"/>.
    /// </summary>
    public LinePatternCache(Document document)
    {
        _linePatterns = this.CreateCache(document);

        var continuousLineTypeName = ReservedStringEnumType.Continuous.ToString();

        _continuousLinePattern = _linePatterns.Values.First(linePattern =>
            linePattern.Name.Equals(continuousLineTypeName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Populates this <see cref="ILinePatternCache"/> with <see cref="IAutocadLinePattern"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    private Dictionary<IObjectId, IAutocadLinePattern> CreateCache(Document document)
    {
        using var database = document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        var lineTypeTableId = database.LinetypeTableId;

        using var lineTypeTable = (LinetypeTable)transactionManager.GetObject(lineTypeTableId, OpenMode.ForRead)!;

        var linePatterns = new Dictionary<IObjectId, IAutocadLinePattern>(new ObjectIdEqualityComparer());
        foreach (var lineTypeRecordId in lineTypeTable)
        {
            using var linetypeTableRecord = (LinetypeTableRecord)transactionManager.GetObject(lineTypeRecordId, OpenMode.ForRead)!;

            var linePattern = new AutocadLinePattern(linetypeTableRecord);

            linePatterns.Add(linePattern.Id, linePattern);
        }

        transaction.Commit();

        return linePatterns;
    }

    /// <inheritdoc/>
    public IAutocadLinePattern GetById(IObjectId linePatternId)
    {
        if (_linePatterns.TryGetValue(linePatternId, out var linePattern))
            return linePattern;

        return _continuousLinePattern;
    }

    /// <summary>
    /// Disposes of the <see cref="LinePatternCache"/> and its resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _continuousLinePattern.Dispose();

            foreach (var linePattern in _linePatterns.Values)
            {
                linePattern.Dispose();
            }

            _disposed = true;
        }
    }
}