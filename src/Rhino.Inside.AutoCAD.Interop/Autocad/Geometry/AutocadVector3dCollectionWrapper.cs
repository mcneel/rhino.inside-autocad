using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadVector3dCollection = Autodesk.AutoCAD.Geometry.Vector3dCollection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IVector3dCollection"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="CadVector3dCollection"/> without copying it, so
/// <see cref="InteropConverter.Unwrap(IVector3dCollection)"/> hands back the same native collection.
/// </remarks>
public class AutocadVector3dCollectionWrapper : AutocadWrapperBase<CadVector3dCollection>, IVector3dCollection
{
    /// <inheritdoc/>
    public int Count => _wrappedAutocadObject.Count;

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadVector3dCollectionWrapper"/>.
    /// </summary>
    /// <param name="collection">
    /// The AutoCAD <see cref="CadVector3dCollection"/> to wrap.
    /// </param>
    public AutocadVector3dCollectionWrapper(CadVector3dCollection collection) : base(collection)
    {
    }
}
