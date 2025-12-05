using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDbObject"/>
public class DbObjectWrapper : WrapperDisposableBase<DBObject>, IDbObject
{
    ///<inheritdoc />
    public IObjectId Id => this.GetObjectId();

    /// <inheritdoc/>
    public bool IsValid => this.Validate();

    /// <summary>
    /// Constructs a new <see cref="DbObjectWrapper"/>.
    /// </summary>
    public DbObjectWrapper(DBObject value) : base(value) { }

    /// <summary>
    /// Returns the current <see cref="IObjectId"/> of the <see cref="IDbObject"/>. In 
    /// case the <see cref="DBObject"/> were appended to the database, the <see cref=
    /// "IObjectId"/> will be updated.
    /// </summary>
    private IObjectId GetObjectId()
    {
        return new AutocadObjectId(_wrappedValue.Id);
    }

    /// <summary>
    /// Validates the <see cref="IDbObject"/>. The <see cref="IDbObject"/> is valid if 
    /// the <see cref="AutocadObjectId"/> is not null, valid and not erased.
    /// </summary>
    protected virtual bool Validate()
    {
        return _wrappedValue.Id is { IsNull: false, IsValid: true, IsErased: false };
    }

    /// <summary>
    /// Creates a shallow clone of the <see cref="DbObjectWrapper"/>.
    /// </summary>
    public IDbObject ShallowClone()
    {
        return new DbObjectWrapper(_wrappedValue);
    }
}