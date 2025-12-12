using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ILineTypeRepository"/>
public class LineTypeRepository : Disposable, ILineTypeRepository
{
    private readonly IAutocadDocument _document;

    private readonly Dictionary<IObjectId, IAutocadLinetypeTableRecord> _linePatterns;
    private readonly IAutocadLinetypeTableRecord _continuousLinetypeTableRecord;

    /// <summary>
    /// Constructs a new <see cref="LineTypeRepository"/>.
    /// </summary>
    public LineTypeRepository(IAutocadDocument document)
    {
        _document = document;

        _linePatterns =
            new Dictionary<IObjectId, IAutocadLinetypeTableRecord>(
                new ObjectIdEqualityComparer());

        this.Repopulate();

        var continuousLineTypeName = ReservedStringEnumType.Continuous.ToString();

        _continuousLinetypeTableRecord = _linePatterns.Values.First(linePattern =>
            linePattern.Name.Equals(continuousLineTypeName, StringComparison.OrdinalIgnoreCase));

    }

    /// <summary>
    /// Populates this <see cref="ILineTypeRepository"/> with <see cref="IAutocadLinetypeTableRecord"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public void Repopulate()
    {
        _linePatterns.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var lineTypeTable = (LinetypeTable)transactionManager.GetObject(
                _document.Database.LinetypeTableId.Unwrap(), OpenMode.ForRead);

            foreach (var lineTypeRecordId in lineTypeTable)
            {
                var linetypeTableRecord = (LinetypeTableRecord)transactionManager.GetObject(lineTypeRecordId, OpenMode.ForRead);

                var linePattern = new AutocadLinetypeTableRecord(linetypeTableRecord);

                _linePatterns.Add(linePattern.Id, linePattern);
            }

            return true;
        });
    }

    /// <summary>
    /// Creates a new linetype in the active document and returns the <see cref="IAutocadLinetypeTableRecord"/>.
    /// </summary>
    private IAutocadLinetypeTableRecord CreateLineType(string name, double patternLength, int numberOfDashes, bool scaleToFit)
    {
        using var documentLock = _document.Unwrap().LockDocument();

        var lineTypeWrapper = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            var linetypeTableRecord = new LinetypeTableRecord()
            {
                Name = name,
                PatternLength = patternLength,
                NumDashes = numberOfDashes,
                IsScaledToFit = scaleToFit,
            };

            // Set the dash pattern array
            this.SetSimpleDashPattern(linetypeTableRecord, patternLength, numberOfDashes);

            using var linetypeTable = (LinetypeTable)transactionManager.GetObject(
                _document.Database.LinetypeTableId.Unwrap(), OpenMode.ForWrite);

            linetypeTable.Add(linetypeTableRecord);

            transactionManager.AddNewlyCreatedDBObject(linetypeTableRecord, true);

            return new AutocadLinetypeTableRecord(linetypeTableRecord);
        });

        return lineTypeWrapper;
    }

    /// <summary>
    /// Sets a simple alternating dash pattern for the linetype.
    /// </summary>
    private void SetSimpleDashPattern(LinetypeTableRecord record, double patternLength, int numberOfDashes)
    {
        if (numberOfDashes <= 0) return;

        var segmentLength = patternLength / numberOfDashes;

        for (var i = 0; i < numberOfDashes; i++)
        {
            // Alternate: dash (positive), space (negative)
            var dashLength = (i % 2 == 0) ? segmentLength : -segmentLength;
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