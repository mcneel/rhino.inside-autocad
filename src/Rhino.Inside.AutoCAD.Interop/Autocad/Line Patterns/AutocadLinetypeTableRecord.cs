using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadLine = Autodesk.AutoCAD.DatabaseServices.Line;
using CadPoint3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadLinetypeTableRecord"/>
public class AutocadLinetypeTableRecord : DbObjectWrapper, IAutocadLinetypeTableRecord
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;
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
    /// Constructs a new <see cref="LinetypeTableRecord"/>.
    /// </summary>
    public AutocadLinetypeTableRecord(LinetypeTableRecord lineTypeTableRecord) : base(lineTypeTableRecord)
    {
        _lineTypeTableRecord = lineTypeTableRecord;

        this.Name = lineTypeTableRecord.Name;
        this.PatternLength = lineTypeTableRecord.PatternLength;
        this.NumDashes = lineTypeTableRecord.NumDashes;
        this.IsScaledToFit = lineTypeTableRecord.IsScaledToFit;
        this.Comments = lineTypeTableRecord.Comments ?? string.Empty;
    }

    /// <inheritdoc/>
    public IList<LineCurve> CreateDash(Point3d originPoint, double patternTotalLength,
        int maxIterations)
    {
        var dashNumber = _lineTypeTableRecord.NumDashes;

        var linePattern = new List<LineCurve>();

        var cadOriginPoint = _geometryConverter.ToAutoCadType(originPoint);

        if (dashNumber <= 1)
        {
            var line = new CadLine(cadOriginPoint,
                new CadPoint3d(cadOriginPoint.X + patternTotalLength, cadOriginPoint.Y, cadOriginPoint.Z));

            var rhinoLine = _geometryConverter.ToRhinoType(line);

            linePattern.Add(rhinoLine);

            return linePattern;
        }

        var lengths = new List<double>();
        for (var i = 0; i < dashNumber; i++)
        {
            var dashLength = _lineTypeTableRecord.DashLengthAt(i);

            lengths.Add(dashLength);
        }

        var index = 0;

        var currentPosition = originPoint.X;

        while (index < maxIterations)
        {
            if (currentPosition >= patternTotalLength)
                break;

            var dashLength = lengths[index % dashNumber];

            var lineLength = Math.Abs(dashLength);

            var start = new CadPoint3d(currentPosition, 0, 0);

            currentPosition += lineLength;

            var endXCoordinate = lineLength < _patternPointLength
                ? currentPosition + _patternPointLength
                : currentPosition;

            var end = new CadPoint3d(endXCoordinate, 0, 0);

            // Negative values are gaps.
            if (Math.Sign(dashLength) > -1)
            {
                var line = new CadLine(start, end);

                var rhinoLine = _geometryConverter.ToRhinoType(line);

                linePattern.Add(rhinoLine);
            }

            index++;
        }

        return linePattern;
    }

    /// <summary>
    /// Creates a shallow clone of the <see cref="AutocadLinetypeTableRecord"/>.
    /// </summary>
    public IAutocadLinetypeTableRecord ShallowClone()
    {
        return new AutocadLinetypeTableRecord(_lineTypeTableRecord);
    }
}