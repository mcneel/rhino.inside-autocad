using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadIntegerCollection = Autodesk.AutoCAD.Geometry.IntegerCollection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IIntegerCollection"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="CadIntegerCollection"/> without copying it, so
/// <see cref="InteropConverter.Unwrap(IIntegerCollection)"/> hands back the same native collection.
/// </remarks>
public class AutocadIntegerCollectionWrapper : AutocadWrapperBase<CadIntegerCollection>, IIntegerCollection
{
    /// <inheritdoc/>
    public int Count => _wrappedAutocadObject.Count;

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadIntegerCollectionWrapper"/>.
    /// </summary>
    /// <param name="collection">
    /// The AutoCAD <see cref="CadIntegerCollection"/> to wrap.
    /// </param>
    public AutocadIntegerCollectionWrapper(CadIntegerCollection collection) : base(collection)
    {
    }
}
