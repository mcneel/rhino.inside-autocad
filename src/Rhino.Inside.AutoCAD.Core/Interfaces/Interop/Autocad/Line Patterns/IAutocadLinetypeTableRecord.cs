using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A geometrical representation of a line pattern from AutoCAD used for
/// display purposes.
/// </summary>
public interface IAutocadLinetypeTableRecord : IDisposable
{
    /// <summary>
    /// The <see cref="IObjectId"/> of this <see cref="IAutocadLinetypeTableRecord"/>.
    /// </summary>
    IObjectId Id { get; }

    /// <summary>
    /// The name of this <see cref="IAutocadLinetypeTableRecord"/> and underlying line style
    /// in AutoCAD.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The total length of the linetype pattern.
    /// </summary>
    double PatternLength { get; }

    /// <summary>
    /// The number of dash segments in the pattern.
    /// </summary>
    int NumDashes { get; }

    /// <summary>
    /// Indicates whether the pattern is scaled to fit the geometry.
    /// </summary>
    bool IsScaledToFit { get; }

    /// <summary>
    /// Comments associated with the linetype.
    /// </summary>
    string Comments { get; }

    /// <summary>
    /// Returns the pattern of the <see cref="LinetypeTableRecord"/> as a list
    /// of <see cref="LineCurve"/>s. The pattern is from the  <paramref name=
    /// "originPoint"/> and runs in the X direction The pattern is repeated until
    /// it is longer than the <paramref name="patternTotalLength"/> or the
    /// number of iterations exceeds the  <paramref name="maxIterations"/>.
    /// </summary>
    IList<LineCurve> CreateDash(Point3d originPoint, double patternTotalLength,
        int maxIterations);

    /// <summary>
    /// Creates a shallow clone of the <see cref="IAutocadLinetypeTableRecord"/>.
    /// </summary>
    IAutocadLinetypeTableRecord ShallowClone();
}