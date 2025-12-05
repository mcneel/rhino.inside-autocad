using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which wraps the AutoCAD
/// <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/>.
/// </summary>
public class AutocadObjectId : WrapperBase<CadObjectId>, IObjectId
{
    private readonly CadObjectId _nullId = CadObjectId.Null;

    /// <summary>
    /// The <see cref="AutocadObjectId"/> value.
    /// </summary>
    /// <remarks>
    /// This value corresponds to the <see cref="Handle.Value"/> of the
    /// encapsulated <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/>.
    /// This value is persistent even if the <see cref="IAutocadDocument"/> is closed.
    /// </remarks>
    public long Value { get; }

    /// <inheritdoc/>
    public bool IsValid => _wrappedValue.IsNull == false && _wrappedValue.Equals(_nullId) == false;

    /// <inheritdoc/>
    public bool IsErased => _wrappedValue.IsErased;

    /// <summary>
    /// Constructs a default/invalid <see cref="AutocadObjectId"/>.
    /// </summary>
    public AutocadObjectId() : base(CadObjectId.Null)
    {
        this.Value = _nullId.Handle.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="AutocadObjectId"/>.
    /// </summary>
    public AutocadObjectId(CadObjectId id) : base(id)
    {
        this.Value = id.Handle.Value;

    }

    /// <summary>
    /// Creates a shallow clone of the <see cref="AutocadObjectId"/>.
    /// </summary>
    public IObjectId ShallowClone()
    {
        return new AutocadObjectId(_wrappedValue);
    }

    /// <summary>
    /// Overrides the default <see cref="Object.ToString"/> method with
    /// the <see cref="IObjectId"/> value.
    /// </summary>
    public override string ToString()
    {
        return $"({this.Value})";
    }
}