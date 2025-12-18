namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a collection of the <see cref="ICustomProperty"/>s.
/// </summary>
public interface IDynamicPropertySet : IEnumerable<IDynamicBlockReferencePropertyWrapper>
{
    /// <summary>
    /// Gets the number of <see cref="IDynamicBlockReferencePropertyWrapper"/>s in the <see
    /// cref="IDynamicPropertySet"/>.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Adds the provided <see cref="IDynamicBlockReferencePropertyWrapper"/> in the <see
    /// cref= "IDynamicPropertySet"/>.
    /// </summary>
    void Add(IDynamicBlockReferencePropertyWrapper property);

    /// <summary>
    /// Tries to get the <see cref="IDynamicBlockReferencePropertyWrapper"/> with the
    /// specified name.
    /// </summary>
    bool TryGet(string name, out IDynamicBlockReferencePropertyWrapper property);
}