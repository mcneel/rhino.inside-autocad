namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The result of a BREP conversion operation.
/// </summary>
public interface IBrepConverterResult
{
    /// <summary>
    /// A collection of converted solids. Only Autocad Solid3d entities are included.
    /// </summary>
    IEntityCollection ConvertedSolids { get; }

    /// <summary>
    /// A boolean indicating whether the conversion was successful.
    /// </summary>
    bool Success { get; }
}
