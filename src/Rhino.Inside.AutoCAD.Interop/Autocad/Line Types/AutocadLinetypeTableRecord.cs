using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadLinetypeTableRecord"/>
public class AutocadLinetypeTableRecordWrapper : AutocadDbObjectWrapper, IAutocadLinetypeTableRecord
{
    private readonly LinetypeTableRecord _lineTypeTableRecord;

    private readonly double _patternPointLength = InteropConstants.LinePatternPointLength;

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public double PatternLength { get; }

    /// <inheritdoc/>
    public int NumDashes { get; }

    /// <inheritdoc/>
    public bool IsScaledToFit { get; }

    /// <inheritdoc/>
    public string Comments { get; }

    /// <summary>
    /// Initializes a new instance wrapping the specified <see cref="LinetypeTableRecord"/>.
    /// </summary>
    /// <param name="lineTypeTableRecord">
    /// The AutoCAD line type record to wrap.
    /// </param>
    /// <remarks>
    /// Property values are cached at construction time to minimize database access.
    /// </remarks>
    public AutocadLinetypeTableRecordWrapper(LinetypeTableRecord lineTypeTableRecord) : base(lineTypeTableRecord)
    {
        _lineTypeTableRecord = lineTypeTableRecord;

        this.Name = lineTypeTableRecord.Name;
        this.PatternLength = lineTypeTableRecord.PatternLength;
        this.NumDashes = lineTypeTableRecord.NumDashes;
        this.IsScaledToFit = lineTypeTableRecord.IsScaledToFit;
        this.Comments = lineTypeTableRecord.Comments ?? string.Empty;
    }
    /// <summary>
    /// Creates a single continuous line segment starting from the specified origin point.
    /// </summary>
    /// <param name="originPoint">The starting point of the line in Rhino coordinates.</param>
    /// <param name="patternTotalLength">The total length of the line segment to create.</param>
    /// <returns>
    /// A list containing a single <see cref="LineCurve"/> representing the continuous line segment.
    /// </returns>
    private IList<LineCurve> CreateSingleLine(Point3d originPoint, double patternTotalLength)
    {
        var cadOrigin = originPoint.ToAutocadPoint3d();
        var end = new CadPoint3d(cadOrigin.X + patternTotalLength, cadOrigin.Y, cadOrigin.Z);
        var line = new CadLine(cadOrigin, end).ToRhinoLineCurve();

        return [line];
    }

    /// <summary>
    /// Retrieves the lengths of all dashes in the line type pattern.
    /// </summary>
    /// <param name="dashCount">The number of dashes in the pattern.</param>
    /// <returns>
    /// A list of dash lengths, where positive values represent dashes and negative values represent gaps.
    /// </returns>
    private List<double> GetDashLengths(int dashCount)
    {
        return Enumerable.Range(0, dashCount)
            .Select(i => _lineTypeTableRecord.DashLengthAt(i))
            .ToList();
    }

    /// <summary>
    /// Determines whether the specified length represents a point in the pattern.
    /// </summary>
    /// <param name="absLength">The absolute length to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the length is less than the threshold for a pattern point; otherwise, <c>false</c>.
    /// </returns>
    private bool IsPatternPoint(double absLength) => absLength < _patternPointLength;

    /// <summary>
    /// Determines whether the specified dash length represents a visible dash.
    /// </summary>
    /// <param name="dashLength">The length of the dash to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the dash length is non-negative; otherwise, <c>false</c>.
    /// </returns>
    private bool IsVisibleDash(double dashLength) => Math.Sign(dashLength) >= 0;

    /// <summary>
    /// Creates a dash pattern consisting of multiple line segments based on the specified parameters.
    /// </summary>
    /// <param name="startX">The starting X-coordinate for the pattern.</param>
    /// <param name="patternTotalLength">The total length over which to generate the pattern.</param>
    /// <param name="maxIterations">The maximum number of dash segments to generate.</param>
    /// <param name="dashLengths">The lengths of the dashes and gaps in the pattern.</param>
    /// <returns>
    /// A list of <see cref="LineCurve"/> objects representing the visible dash segments.
    /// </returns>
    private IList<LineCurve> CreateDashPattern(double startX, double patternTotalLength,
        int maxIterations, IList<double> dashLengths)
    {
        var linePattern = new List<LineCurve>();

        var currentX = startX;

        var dashCount = dashLengths.Count;

        for (var i = 0; i < maxIterations && currentX < patternTotalLength; i++)
        {
            var dashLength = dashLengths[i % dashCount];

            var absLength = Math.Abs(dashLength);

            var endX = this.IsPatternPoint(absLength) ? currentX + absLength + _patternPointLength : currentX + absLength;

            if (this.IsVisibleDash(dashLength))
            {
                var line = new CadLine(new CadPoint3d(currentX, 0, 0), new CadPoint3d(endX, 0, 0));
                linePattern.Add(line.ToRhinoLineCurve());
            }

            currentX += absLength;
        }

        return linePattern;
    }

    /// <inheritdoc/>
    public IList<LineCurve> CreateDash(Point3d originPoint, double patternTotalLength, int maxIterations)
    {
        var dashCount = _lineTypeTableRecord.NumDashes;

        if (dashCount <= 1)
            return this.CreateSingleLine(originPoint, patternTotalLength);

        var dashLengths = this.GetDashLengths(dashCount);
        return this.CreateDashPattern(originPoint.X, patternTotalLength, maxIterations, dashLengths);
    }

    /// <summary>
    /// Creates a shallow clone that wraps the same underlying <see cref="LinetypeTableRecord"/>.
    /// </summary>
    /// <returns>
    /// A new <see cref="AutocadLinetypeTableRecordWrapper"/> instance referencing the same AutoCAD object.
    /// </returns>
    /// <remarks>
    /// The cloned wrapper shares the same underlying AutoCAD line type record but is an
    /// independent wrapper instance.
    /// </remarks>
    public override IDbObject ShallowClone()
    {
        return new AutocadLinetypeTableRecordWrapper(_lineTypeTableRecord);
    }

    /// <summary>
    /// Sets a simple alternating dash pattern for the linetype.
    /// </summary>
    private static void SetSimpleDashPattern(LinetypeTableRecord record, double patternLength, int numberOfDashes)
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

    /// <summary>
    /// Creates a new linetype in the active document and returns the <see cref="IAutocadLinetypeTableRecord"/>.
    /// </summary>
    public static IAutocadLinetypeTableRecord Create(IAutocadDocument document,
        string name, double patternLength, int numberOfDashes, bool scaleToFit)
    {
        var transactionManagerWrapper = document.CreateTransactionManager();

        var lineTypeWrapper = transactionManagerWrapper.PerformTask(() =>
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
            SetSimpleDashPattern(linetypeTableRecord, patternLength, numberOfDashes);

            var linetypeTable = (LinetypeTable)transactionManager.GetObject(
                document.AutocadDatabase.LinetypeTableId.Unwrap(), OpenMode.ForWrite);

            linetypeTable.Add(linetypeTableRecord);

            transactionManager.AddNewlyCreatedDBObject(linetypeTableRecord, true);

            return new AutocadLinetypeTableRecordWrapper(linetypeTableRecord);
        });

        document.Regenerate();

        return lineTypeWrapper;
    }
}