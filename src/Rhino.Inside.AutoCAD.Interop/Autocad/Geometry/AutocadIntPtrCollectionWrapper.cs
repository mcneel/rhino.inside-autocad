using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadIntPtrCollection = Autodesk.AutoCAD.Geometry.IntPtrCollection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IIntPtrCollection"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="CadIntPtrCollection"/> without copying it, so
/// <see cref="InteropConverter.Unwrap(IIntPtrCollection)"/> hands back the same native collection.
/// </remarks>
public class AutocadIntPtrCollectionWrapper : AutocadWrapperBase<CadIntPtrCollection>, IIntPtrCollection
{
    /// <inheritdoc/>
    public int Count => _wrappedAutocadObject.Count;

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadIntPtrCollectionWrapper"/>.
    /// </summary>
    /// <param name="collection">
    /// The AutoCAD <see cref="CadIntPtrCollection"/> to wrap.
    /// </param>
    public AutocadIntPtrCollectionWrapper(CadIntPtrCollection collection) : base(collection)
    {
    }
}
