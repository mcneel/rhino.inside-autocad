namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the boundary curves that form an outline in <see cref="IHatch"/>.
/// </summary>
public interface IHatchLoop
{
    /// <summary>
    /// The curves that make up the loop.
    /// </summary>
    // IPolyCurve OutlineCurve { get; }

    /// <summary>
    /// The <see cref="OutlineType"/>.
    /// </summary>
    // HatchOutlineType OutlineType { get; }

    /// <summary>
    /// True if the loop forms a boundary of a <see cref="IHatch"/>, otherwise false.
    /// </summary>
    /// <remarks>
    /// Typically, a valid <see cref="IHatchLoop"/> forms a boundary of a <see cref="IHatch"/>,
    /// however there are cases where the loop does not form a boundary, for example if a
    /// <see cref="IBlockReference"/> with a closed curve outline has any interior closed curves
    /// (e.g. an offset of its outer boundary), then all the geometry in the block is added
    /// to the <see cref="IHatch"/>. This can be problematic since the interior closed curves
    /// are not part of the boundary of the <see cref="IHatch"/>, which can cause problems for
    /// a consumer of this type to obtain only the <see cref="IHatchLoop"/>s that form the
    /// boundary edges of the <see cref="IHatch"/>.
    /// </remarks>
    bool IsBoundary { get; }
}