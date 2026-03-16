using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDbObject"/>
/// <remarks>
/// Wraps an AutoCAD <see cref="DBObject"/> to provide a managed abstraction layer.
/// This is the base wrapper class for all AutoCAD database objects and is used extensively
/// in the Grasshopper library by components such as AutocadDbObjectComponent and
/// AutocadObjectIdComponent, as well as Goo types including <c>GH_AutocadObject</c>,
/// <c>GH_AutocadLayer</c>, <c>GH_AutocadLayout</c>, and <c>GH_AutocadLineType</c>.
/// </remarks>
/// <seealso cref="IDbObject"/>
/// <seealso cref="IObjectId"/>
/// <seealso cref="AutocadObjectIdWrapper"/>
public class AutocadDbObjectWrapper : AutocadWrapperDisposableBase<DBObject>, IDbObject
{
    /// <inheritdoc/>
    public IObjectId Id => new AutocadObjectIdWrapper(_wrappedAutocadObject.Id);

    /// <inheritdoc/>
    public Type Type { get; }

    /// <inheritdoc/>
    public bool IsValid => this.Validate();

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadDbObjectWrapper"/> class.
    /// </summary>
    /// <param name="autocadObject">
    /// The AutoCAD <see cref="DBObject"/> to wrap.
    /// </param>
    /// <remarks>
    /// Captures the runtime type of the wrapped object for later type checking operations.
    /// The wrapper does not take ownership of the underlying object's lifecycle.
    /// </remarks>
    public AutocadDbObjectWrapper(DBObject autocadObject) : base(autocadObject)
    {
        this.Type = autocadObject.GetType();
    }

    /// <summary>
    /// Validates whether the wrapped <see cref="DBObject"/> is still accessible.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the object's <see cref="ObjectId"/> is not null, valid, and not erased;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Override this method in derived classes to add additional validation logic
    /// specific to the wrapped object type.
    /// </remarks>
    protected virtual bool Validate()
    {
        return _wrappedAutocadObject.Id is { IsNull: false, IsValid: true, IsErased: false };
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Creates a new <see cref="AutocadDbObjectWrapper"/> that references the same
    /// underlying <see cref="DBObject"/>. Both wrappers share the same AutoCAD object,
    /// so modifications through either wrapper affect the same database object.
    /// </remarks>
    public virtual IDbObject ShallowClone()
    {
        return new AutocadDbObjectWrapper(_wrappedAutocadObject);
    }
}
