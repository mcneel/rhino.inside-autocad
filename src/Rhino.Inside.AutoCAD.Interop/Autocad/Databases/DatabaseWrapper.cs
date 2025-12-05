using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which provides a wrapped version of the AutoCAD
/// <see cref="Database"/>.
/// </summary>
public class DatabaseWrapper : WrapperDisposableBase<Database>, IDatabase
{
    /// <inheritdoc/>
    public IObjectId BlockTableId { get; }

    /// <inheritdoc/>
    public IObjectId LinetypeTableId { get; }

    /// <inheritdoc/>
    public IObjectId ByLayerLineTypeId { get; }

    /// <summary>
    /// constructs a new <see cref="DatabaseWrapper"/> using
    /// the provided AutoCAD <see cref="Database"/>.
    /// </summary>
    public DatabaseWrapper(Database database) : base(database)
    {
        this.BlockTableId = new AutocadObjectId(database.BlockTableId);

        this.LinetypeTableId = new AutocadObjectId(database.LinetypeTableId);

        this.ByLayerLineTypeId = new AutocadObjectId(database.ByLayerLinetype);
    }

    /// <inheritdoc/>
    public IObjectId GetObjectId(long id, out bool isValid)
    {
        var handle = new Handle(id);

        var cadObjectId = _wrappedValue.GetObjectId(true, handle, 0);

        var objectId = new AutocadObjectId(cadObjectId);

        isValid = cadObjectId is { IsValid: true, IsNull: false, IsErased: false, IsEffectivelyErased: false };

        return objectId;
    }

    /// <summary>
    /// Opens the <see cref="INamedObjectsDictionary"/> for reading.
    /// </summary>
    public INamedObjectsDictionary GetNamedObjectsDictionary()
    {
        var namedObjectsDictionaryId = _wrappedValue.NamedObjectsDictionaryId;

        var dbDictionary = (DBDictionary)namedObjectsDictionaryId.GetObject(OpenMode.ForRead);

        return new NamedObjectsDictionary(dbDictionary);
    }
}