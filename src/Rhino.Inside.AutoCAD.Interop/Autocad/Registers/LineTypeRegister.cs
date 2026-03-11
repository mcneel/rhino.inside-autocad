using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ILineTypeRegister"/>
public class LineTypeRegister : RegisterBase<IAutocadLinetypeTableRecord>, ILineTypeRegister
{
    private readonly IAutocadLinetypeTableRecord _continuousLinetypeTableRecord;

    /// <summary>
    /// Constructs a new <see cref="LineTypeRegister"/>.
    /// </summary>
    public LineTypeRegister(IAutocadDocument document) : base(document)
    {
        var continuousLineTypeName = ReservedStringEnumType.Continuous.ToString();

        _continuousLinetypeTableRecord = _objects.Values.First(linePattern =>
            linePattern.Name.Equals(continuousLineTypeName, StringComparison.OrdinalIgnoreCase));

    }

    /// <summary>
    /// Populates this <see cref="ILineTypeRegister"/> with <see cref="IAutocadLinetypeTableRecord"/>s
    /// from the active <see cref="IAutocadDocument"/>.
    /// </summary>
    public override void Repopulate()
    {
        _objects.Clear();

        _ = _document.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var lineTypeTable = (LinetypeTable)transactionManager.GetObject(
                _document.Database.LinetypeTableId.Unwrap(), OpenMode.ForRead);

            foreach (var lineTypeRecordId in lineTypeTable)
            {
                var linetypeTableRecord = (LinetypeTableRecord)transactionManager.GetObject(lineTypeRecordId, OpenMode.ForRead);

                var linePattern = new AutocadLinetypeTableRecordWrapper(linetypeTableRecord);

                _objects.Add(linePattern.Id, linePattern);
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

            return new AutocadLinetypeTableRecordWrapper(linetypeTableRecord);
        });

        _document.Regenerate();

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
    public bool TryAddLineType(string name, double patternLength, int numberOfDashes, bool scaleToFit, out IAutocadLinetypeTableRecord lineType)
    {
        if (this.TryGetByName(name, out var existing) && existing != null)
        {
            lineType = existing;
            return false;
        }

        lineType = this.CreateLineType(name, patternLength, numberOfDashes, scaleToFit);

        _objects[lineType.Id] = lineType;

        return true;
    }

    /// <inheritdoc/>
    public IAutocadLinetypeTableRecord GetDefault() => _continuousLinetypeTableRecord;
}