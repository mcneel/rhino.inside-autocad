using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which wraps the AutoCAD
/// <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/>.
/// </summary>
public class ObjectId : WrapperBase<CadObjectId>, IObjectId
{
    private readonly CadObjectId _nullId = CadObjectId.Null;

    /// <summary>
    /// The <see cref="ObjectId"/> value.
    /// </summary>
    /// <remarks>
    /// This value corresponds to the <see cref="Handle.Value"/> of the
    /// encapsulated <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/>.
    /// This value is persistent even if the <see cref="IAutoCadDocument"/> is closed.
    /// </remarks>
    public long Value { get; }

    /// <summary>
    /// Returns true if the <see cref="ObjectId"/> is valid, otherwise
    /// returns false.
    /// </summary>
    public bool IsValid => _wrappedValue.IsNull == false && _wrappedValue.Equals(_nullId) == false;

    /// <summary>
    /// Constructs a default/invalid <see cref="ObjectId"/>.
    /// </summary>
    public ObjectId() : base(CadObjectId.Null)
    {
        this.Value = _nullId.Handle.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="ObjectId"/>.
    /// </summary>
    public ObjectId(CadObjectId id) : base(id)
    {
        this.Value = id.Handle.Value;
    }
}