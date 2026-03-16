using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an AutoCAD line type definition (LinetypeTableRecord) with its dash/gap pattern.
/// </summary>
/// <remarks>
/// Wraps an AutoCAD LinetypeTableRecord to expose line pattern properties and provide
/// geometry generation for previews. Line types define patterns such as "Continuous",
/// "Dashed", "Center", and "Hidden". Used by Grasshopper components including
/// AutocadLineTypeComponent, GetAutocadLineTypesComponent, and CreateAutocadLineTypeComponent.
/// </remarks>
/// <seealso cref="ILineTypeRegister"/>
/// <seealso cref="INamedDbObject"/>
public interface IAutocadLinetypeTableRecord : INamedDbObject
{
    /// <summary>
    /// Gets the total length of one complete pattern repetition in drawing units.
    /// </summary>
    /// <remarks>
    /// For continuous line types, this value is 0. For patterned line types, this is the
    /// sum of all dash and gap lengths in the pattern definition.
    /// </remarks>
    double PatternLength { get; }

    /// <summary>
    /// Gets the number of dash and gap segments in the pattern.
    /// </summary>
    /// <remarks>
    /// A value of 0 or 1 indicates a continuous line type. Higher values indicate
    /// a patterned line type where positive values are dashes and negative values are gaps.
    /// </remarks>
    int NumDashes { get; }

    /// <summary>
    /// Gets a value indicating whether the pattern is scaled to fit the geometry length.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, AutoCAD adjusts the pattern scale so that the line starts and
    /// ends with a complete dash segment.
    /// </remarks>
    bool IsScaledToFit { get; }

    /// <summary>
    /// Gets the descriptive comments associated with this line type.
    /// </summary>
    /// <remarks>
    /// Returns an empty string if no comments are defined.
    /// </remarks>
    string Comments { get; }

    /// <summary>
    /// Generates the dash pattern as a list of <see cref="LineCurve"/> segments for preview display.
    /// </summary>
    /// <param name="originPoint">
    /// The starting point for the pattern in Rhino coordinates.
    /// </param>
    /// <param name="patternTotalLength">
    /// The total length over which to generate the dash pattern.
    /// </param>
    /// <param name="maxIterations">
    /// Maximum number of dash segments to generate, preventing infinite loops.
    /// </param>
    /// <returns>
    /// A list of <see cref="LineCurve"/> objects representing the visible dash segments.
    /// Gaps are omitted from the output.
    /// </returns>
    /// <remarks>
    /// The pattern is generated along the positive X direction from the origin point.
    /// Generation stops when the pattern exceeds <paramref name="patternTotalLength"/>
    /// or the iteration count reaches <paramref name="maxIterations"/>.
    /// </remarks>
    IList<LineCurve> CreateDash(Point3d originPoint, double patternTotalLength,
        int maxIterations);
}