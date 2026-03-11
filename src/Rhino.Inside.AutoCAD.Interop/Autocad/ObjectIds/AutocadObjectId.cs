using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectId"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="CadObjectId"/> to provide a stable handle-based identifier.
/// The <see cref="Value"/> property stores the handle value rather than the runtime ObjectId,
/// ensuring consistency across sessions. Used extensively throughout the Grasshopper library
/// in components like <c>AutocadObjectIdComponent</c> and <c>GetByAutocadIdComponent</c>.
/// </remarks>
/// <seealso cref="IObjectIdCollection"/>
public class AutocadObjectIdWrapper : AutocadWrapperBase<CadObjectId>, IObjectId
{
    private readonly CadObjectId _nullId = CadObjectId.Null;

    /// <summary>
    /// Gets a default instance representing a null ObjectId.
    /// </summary>
    public static IObjectId DefaultId => new AutocadObjectIdWrapper(CadObjectId.Null);

    /// <inheritdoc/>
    public long Value { get; }

    /// <inheritdoc/>
    public bool IsValid => _wrappedAutocadObject.IsNull == false && _wrappedAutocadObject.Equals(_nullId) == false;

    /// <inheritdoc/>
    public bool IsErased => _wrappedAutocadObject.IsErased;

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadObjectIdWrapper"/>.
    /// </summary>
    /// <param name="id">
    /// The AutoCAD <see cref="CadObjectId"/> to wrap.
    /// </param>
    public AutocadObjectIdWrapper(CadObjectId id) : base(id)
    {
        this.Value = id.Handle.Value;
    }

    /// <inheritdoc/>
    public IObjectId ShallowClone()
    {
        return new AutocadObjectIdWrapper(_wrappedAutocadObject);
    }

    /// <inheritdoc/>
    public bool IsEqualTo(IObjectId other)
    {
        return this.Value == other.Value;
    }

    /// <summary>
    /// Returns the handle value formatted as a parenthesized string.
    /// </summary>
    /// <returns>
    /// A string in the format "(handle_value)".
    /// </returns>
    public override string ToString()
    {
        return $"({this.Value})";
    }
}
