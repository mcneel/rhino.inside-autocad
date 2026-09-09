using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDbObject"/>
/// <remarks>
/// Records what a database object was - its <see cref="Id"/> and its <see cref="Type"/> -
/// without holding on to the object itself.
/// <para>
/// AutoCAD hands a <see cref="DBObject"/> to a database reactor for the duration of the
/// notification only; it closes the object as soon as the handler returns, and touching the
/// managed wrapper afterwards reads memory AutoCAD has reclaimed. Document changes are
/// accumulated across a whole command and only read once it ends, which is long after that,
/// so <see cref="AutocadDocument"/> records changes as these instead of as
/// <see cref="AutocadDbObjectWrapper"/>. Both properties are captured while the reactor is
/// still on the stack, and nothing here dereferences the object again.
/// </para>
/// </remarks>
public class DetachedDbObject : IDbObject
{
    /// <inheritdoc/>
    public IObjectId Id { get; }

    /// <inheritdoc/>
    public Type Type { get; }

    /// <inheritdoc/>
    public bool IsValid => this.Id is { IsValid: true, IsErased: false };

    /// <summary>
    /// Constructs a new <see cref="DetachedDbObject"/> from a database object, reading
    /// everything needed from it up front.
    /// </summary>
    /// <param name="dbObject">
    /// The object to record. Only read during this call, so it is safe to pass one handed
    /// out by a reactor.
    /// </param>
    public DetachedDbObject(DBObject dbObject)
    {
        this.Id = new AutocadObjectIdWrapper(dbObject.Id);

        this.Type = dbObject.GetType();
    }

    /// <summary>
    /// Constructs a new <see cref="DetachedDbObject"/> from an already recorded id and type.
    /// </summary>
    private DetachedDbObject(IObjectId id, Type type)
    {
        this.Id = id;

        this.Type = type;
    }

    /// <inheritdoc/>
    public IDbObject ShallowClone()
    {
        return new DetachedDbObject(this.Id.ShallowClone(), this.Type);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Nothing to release: this holds no AutoCAD object. Implemented only because
    /// <see cref="IDbObject"/> is <see cref="IDisposable"/>.
    /// </remarks>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
