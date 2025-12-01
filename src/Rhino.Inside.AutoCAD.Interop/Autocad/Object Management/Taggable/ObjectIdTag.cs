using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectIdTag"/>
public class ObjectIdTag : IObjectIdTag
{
    private readonly GroupCodeValue _tagGroupCode = GroupCodeValue._160;

    /// <inheritdoc/>
    public GroupCodeValue GroupCode { get; }

    /// <inheritdoc/>
    public long Id { get; }

    /// <summary>
    /// Constructs a new <see cref="ObjectIdTag"/> from a <see cref="IObjectId"/>.
    /// </summary>
    public ObjectIdTag(IObjectId objectId) : this(objectId.Value)
    {

    }

    /// <summary>
    /// Constructs a new <see cref="ObjectIdTag"/>.
    /// </summary>
    private ObjectIdTag(long objectIdValue)
    {
        this.GroupCode = _tagGroupCode;

        this.Id = objectIdValue;
    }

    /// <summary>
    /// Creates an existing <see cref="IObjectIdTag"/> from an <see cref=
    /// "IObjectId"/> value typically for data retrieval purposes, such as
    /// retrieving an existing entity from the AutoCAD database using the
    /// <see cref="IObjectId.Value"/> of the <see cref="IObjectIdTag"/>.
    /// </summary>
    public static IObjectIdTag CreateExisting(long objectIdValue)
    {
        return new ObjectIdTag(objectIdValue);
    }
}