using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadPoint3dCollection = Autodesk.AutoCAD.Geometry.Point3dCollection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPoint3dCollection"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="CadPoint3dCollection"/> without copying it, so
/// <see cref="InteropConverter.Unwrap(IPoint3dCollection)"/> hands back the same native collection.
/// </remarks>
public class AutocadPoint3dCollectionWrapper : AutocadWrapperBase<CadPoint3dCollection>, IPoint3dCollection
{
    /// <inheritdoc/>
    public int Count => _wrappedAutocadObject.Count;

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadPoint3dCollectionWrapper"/>.
    /// </summary>
    /// <param name="collection">
    /// The AutoCAD <see cref="CadPoint3dCollection"/> to wrap.
    /// </param>
    public AutocadPoint3dCollectionWrapper(CadPoint3dCollection collection) : base(collection)
    {
    }
}
