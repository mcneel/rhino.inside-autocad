using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <inheritdoc cref="IAutocadReferenceId"/>
public class AutocadReferenceId : IAutocadReferenceId
{
    /// <summary>
    /// Static constructor for when there is no reference.
    /// </summary>
    public static AutocadReferenceId NoReference => new AutocadReferenceId();

    /// <summary>
    /// The Handle which persists between AutoCAD sessions to identify the
    /// referenced object.
    /// </summary>
    private readonly Handle _objectHandle;

    /// <inheritdoc  />
    public IObjectId ObjectId { get; }

    /// <inheritdoc  />
    public bool IsValid => this.ObjectId.IsValid;

    /// <summary>
    /// Default constructor for when there is no referenced object.
    /// </summary>
    private AutocadReferenceId()
    {
        this.ObjectId = new AutocadObjectId();
        _objectHandle = new Handle();
    }

    /// <summary>
    /// Constructor which references an AutoCAD Object.
    /// </summary>
    public AutocadReferenceId(IDbObject objectToReference)
    {

        this.ObjectId = objectToReference.Id;
        _objectHandle = objectToReference.UnwrapObject().Handle;
    }

    /// <summary>
    /// Constructor which references an AutoCAD Object.
    /// </summary>
    public AutocadReferenceId(DBObject objectToReference)
    {

        this.ObjectId = new AutocadObjectId(objectToReference.Id);
        _objectHandle = objectToReference.Handle;
    }

    /// <inheritdoc  />
    public string GetSerializedValue()
    {
        return _objectHandle.ToString();
    }

    /// <inheritdoc  />
    public override string ToString()
    {
        return this.IsValid ? this.ObjectId.Value.ToString() : "No Database Id";
    }
}