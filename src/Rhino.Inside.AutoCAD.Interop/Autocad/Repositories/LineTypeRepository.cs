using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autofac.Util;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ILineTypeRepository"/>
public class LineTypeRepository : Disposable, ILineTypeRepository
{
    private readonly Document _document;
    private bool _disposed;

    private readonly Dictionary<IObjectId, IAutocadLinetypeTableRecord> _linePatterns;
    private readonly IAutocadLinetypeTableRecord _continuousLinetypeTableRecord;

    /// <summary>
    /// Constructs a new <see cref="LineTypeRepository"/>.
    /// </summary>
    public LineTypeRepository(Document document)
    {
        _document = document;

        _linePatterns =
            new Dictionary<IObjectId, IAutocadLinetypeTableRecord>(
                new ObjectIdEqualityComparer());

        this.Populate();

        var continuousLineTypeName = ReservedStringEnumType.Continuous.ToString();

        _continuousLinetypeTableRecord = _linePatterns.Values.First(linePattern =>
            linePattern.Name.Equals(continuousLineTypeName, StringComparison.OrdinalIgnoreCase));

        this.SubscribeToModifyEvent();
    }

    /// <summary>
    /// Handles the LayerTable Modified event.
    /// </summary>
    private void LineTypeTable_Modified(object sender, EventArgs e)
    {
        this.Populate();
    }

    private void SubscribeToModifyEvent()
    {
        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForRead);

        layerTable.Modified += this.LineTypeTable_Modified;

        transaction.Commit();
    }

    private void UnsubscribeToModifyEvent()
    {
        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var layerTable = (LayerTable)transactionManager.GetObject(database.LayerTableId, OpenMode.ForRead);

        layerTable.Modified -= this.LineTypeTable_Modified;

        transaction.Commit();
    }

    /// <summary>
    /// Populates this <see cref="ILineTypeRepository"/> with <see cref="IAutocadLinetypeTableRecord"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    private void Populate()
    {
        _linePatterns.Clear();

        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        using var lineTypeTable = (LinetypeTable)transactionManager.GetObject(database.LinetypeTableId, OpenMode.ForRead);

        foreach (var lineTypeRecordId in lineTypeTable)
        {
            var linetypeTableRecord = (LinetypeTableRecord)transactionManager.GetObject(lineTypeRecordId, OpenMode.ForRead);

            var linePattern = new AutocadLinetypeTableRecord(linetypeTableRecord);

            _linePatterns.Add(linePattern.Id, linePattern);
        }

        transaction.Commit();
    }

    /// <summary>
    /// Creates a new linetype in the active document and returns the <see cref="IAutocadLinetypeTableRecord"/>.
    /// </summary>
    private IAutocadLinetypeTableRecord CreateLineType(string name, double patternLength, int numberOfDashes, bool scaleToFit)
    {
        using var documentLock = _document.LockDocument();

        this.UnsubscribeToModifyEvent();

        var database = _document.Database;

        using var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        var linetypeTableRecord = new LinetypeTableRecord()
        {
            Name = name,
            PatternLength = patternLength,
            NumDashes = numberOfDashes,
            IsScaledToFit = scaleToFit,
        };

        // Set the dash pattern array
        this.SetSimpleDashPattern(linetypeTableRecord, patternLength, numberOfDashes);

        using var linetypeTable = (LinetypeTable)transactionManager.GetObject(database.LinetypeTableId, OpenMode.ForWrite);

        linetypeTable.Add(linetypeTableRecord);

        transactionManager.AddNewlyCreatedDBObject(linetypeTableRecord, true);

        var lineTypeWrapper = new AutocadLinetypeTableRecord(linetypeTableRecord);

        transaction.Commit();

        this.SubscribeToModifyEvent();

        return lineTypeWrapper;
    }

    /// <summary>
    /// Sets a simple alternating dash pattern for the linetype.
    /// </summary>
    private void SetSimpleDashPattern(LinetypeTableRecord record, double patternLength, int numberOfDashes)
    {
        if (numberOfDashes <= 0) return;

        double segmentLength = patternLength / numberOfDashes;

        for (int i = 0; i < numberOfDashes; i++)
        {
            // Alternate: dash (positive), space (negative)
            double dashLength = (i % 2 == 0) ? segmentLength : -segmentLength;
            record.SetDashLengthAt(i, dashLength);
        }
    }

    /// <inheritdoc/>
    public IAutocadLinetypeTableRecord GetById(IObjectId linePatternId)
    {
        if (_linePatterns.TryGetValue(linePatternId, out var linePattern))
            return linePattern;

        return _continuousLinetypeTableRecord;
    }

    /// <inheritdoc/>
    public bool TryAddLineType(string name, double patternLength, int numberOfDashes, bool scaleToFit, out IAutocadLinetypeTableRecord lineType)
    {
        if (this.TryGetByName(name, out var existing) && existing != null)
        {
            lineType = existing;
            return false;
        }

        lineType = this.CreateLineType(name, patternLength, numberOfDashes, scaleToFit);

        _linePatterns[lineType.Id] = lineType;

        return true;
    }

    /// <summary>
    /// Disposes of the <see cref="LineTypeRepository"/> and its resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _continuousLinetypeTableRecord.Dispose();
            this.UnsubscribeToModifyEvent();

            foreach (var linePattern in _linePatterns.Values)
            {
                linePattern.Dispose();
            }

            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public bool TryGetByName(string name, out IAutocadLinetypeTableRecord? value)
    {
        value = _linePatterns.Values.FirstOrDefault(lineType => lineType.Name == name);
        return value != null;
    }

    /// <inheritdoc/>
    public IEnumerator<IAutocadLinetypeTableRecord> GetEnumerator() =>
        _linePatterns.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}