using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="ILinePattern"/>
public class LinePattern : WrapperDisposableBase<LinetypeTableRecord>, ILinePattern
{
    private readonly double _maxIterations = 50;

    //  private readonly double _patternPointLength = InteropConstants.LinePatternPointLength;
    //  private readonly double _linePatternTotalLength = InteropConstants.LinePatternTotalLength;

    /// <inheritdoc/>
    //  public IList<ILineCurve> Pattern { get; }
    /// <inheritdoc/>
    public IObjectId Id { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <summary>
    /// Constructs a new <see cref="LinetypeTableRecord"/>.
    /// </summary>
    public LinePattern(LinetypeTableRecord lineTypeTableRecord, IUnitSystemManager unitSystemManager) : base(lineTypeTableRecord)
    {
        this.Name = lineTypeTableRecord.Name;

        //    this.Pattern = this.CreateDash(lineTypeTableRecord, unitSystemManager);

        this.Id = new ObjectId(lineTypeTableRecord.Id);
    }

    /*  /// <summary>
      /// Returns the pattern of the <see cref="LinetypeTableRecord"/> as a list
      /// of <see cref="ILineCurve"/>s. The pattern is repeated until it is longer
      /// than the <see cref=" InteropConstants.LinePatternTotalLength"/> or the
      /// number of iterations exceeds the <see cref="_maxIterations"/>.
      /// </summary>
      private IList<ILineCurve> CreateDash(LinetypeTableRecord lineTypeTableRecord, IUnitSystemManager unitSystemManager)
      {
          var dashNumber = lineTypeTableRecord.NumDashes;

          var linePattern = new List<ILineCurve>();

          var originPoint = Point3d.ZeroPoint;

          if (dashNumber <= 1)
          {
              var line = new LineCurve(originPoint, new Point3d(_linePatternTotalLength, 0, 0));

              linePattern.Add(line);

              return linePattern;
          }

          var lengths = new List<double>();
          for (var i = 0; i < dashNumber; i++)
          {
              var dashLength = lineTypeTableRecord.DashLengthAt(i);

              var internalDashLength = unitSystemManager.ToInternalLength(dashLength);

              lengths.Add(internalDashLength);
          }

          var index = 0;

          var currentPosition = originPoint.X;

          while (index < _maxIterations)
          {
              if (currentPosition >= _linePatternTotalLength)
                  break;

              var dashLength = lengths[index % dashNumber];

              var lineLength = Math.Abs(dashLength);

              var start = new Point3d(currentPosition, 0, 0);

              currentPosition += lineLength;

              var endXCoord = lineLength < _patternPointLength ? currentPosition + _patternPointLength : currentPosition;

              var end = new Point3d(endXCoord, 0, 0);

              // Negative values are gaps.
              if (Math.Sign(dashLength) > -1)
              {
                  var line = new LineCurve(start, end);

                  linePattern.Add(line);
              }

              index++;
          }

          return linePattern;
      }*/
}