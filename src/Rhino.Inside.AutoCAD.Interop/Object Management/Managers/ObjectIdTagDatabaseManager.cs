using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IObjectIdTagDatabaseManager"/>
public class ObjectIdTagDatabaseManager : IObjectIdTagDatabaseManager
{
    private readonly IAutoCadDocument _autoCadDocument;

    // A cache of ITagDatabase's for rapid access - lazily populated.
    private readonly Dictionary<string, IObjectIdTagDatabase> _tagDatabaseCache;

    /// <summary>
    /// Constructs a new <see cref="ObjectIdTagDatabaseManager"/>.
    /// </summary>
    public ObjectIdTagDatabaseManager(IAutoCadDocument autoCadDocument)
    {
        _autoCadDocument = autoCadDocument;

        _tagDatabaseCache = new Dictionary<string, IObjectIdTagDatabase>();
    }

    /// <summary>
    /// Creates a <see cref="IObjectIdTagDatabase"/>, from the existing <see cref="DBDictionary"/>.
    /// The <paramref name="dbTagDictionary"/> <see cref="Xrecord"/>s correspond to a
    /// <see cref="IObjectIdTagRecord"/> in the <see cref="IObjectIdTagDatabase"/>, and each
    /// <see cref="TypedValue"/> in the <see cref="Xrecord"/> correspond to an
    /// <see cref="IObjectIdTag"/>. The tags are added to their respective
    /// <see cref="IObjectIdTagRecord"/> using the dictionary key name to access the collection
    /// property name.
    /// </summary>
    private IObjectIdTagDatabase CreateExistingTagDatabase(DBDictionary dbTagDictionary, TransactionManager transactionManager, string databaseKey)
    {
        var tagDatabase = new ObjectIdTagDatabase(databaseKey);

        foreach (var dbEntry in dbTagDictionary)
        {
            var xRecord = (Xrecord)transactionManager.GetObject(dbEntry.Value, OpenMode.ForRead);

            using var resultBuffer = xRecord.Data;

            if (resultBuffer == null) continue;

            var tagCollectionName = dbEntry.Key;

            var tagCollection = tagDatabase.GetTagRecord(tagCollectionName);

            foreach (var typedValue in resultBuffer)
            {
                var id = (long)typedValue.Value;

                var objectTag = ObjectIdTag.CreateExisting(id);

                tagCollection.AddExisting(objectTag);
            }
        }

        return tagDatabase;
    }

    /// <summary>
    /// Returns the <see cref="DBDictionary"/> corresponding to the
    /// <see cref="IObjectIdTagDatabase"/>. If no <see cref="DBDictionary"/>
    /// exists, a new one is created and added to the
    /// <see cref="INamedObjectsDictionary"/> in AutoCAD before being
    /// returned.
    /// </summary>
    private DBDictionary GetTagDbDictionary(IObjectIdTagDatabase objectIdTagDatabase,
        INamedObjectsDictionary namedObjectsDictionary, TransactionManager transactionManager)
    {
        var tagDatabaseKey = objectIdTagDatabase.Key;

        var namedObjectsDictionaryUnwrapped = namedObjectsDictionary.Unwrap();

        if (namedObjectsDictionary.TryGetValue(tagDatabaseKey, out _) == false)
        {
            using var dbDictionary = new DBDictionary();

            namedObjectsDictionaryUnwrapped.SetAt(tagDatabaseKey, dbDictionary);

            transactionManager.AddNewlyCreatedDBObject(dbDictionary, true);
        }

        var tagDbDictionary =
            (DBDictionary)transactionManager.GetObject(namedObjectsDictionaryUnwrapped.GetAt(tagDatabaseKey),
                OpenMode.ForWrite);

        return tagDbDictionary;
    }

    /// <summary>
    /// Creates a new <see cref="Xrecord"/> from the <paramref name="objectIdTagRecord"/>.
    /// </summary>
    private Xrecord CreateTagRecord(IObjectIdTagRecord objectIdTagRecord)
    {
        var xRecord = new Xrecord();

        using var resultBuffer = new ResultBuffer();

        var registeredTags = objectIdTagRecord.GetRegisteredTags();

        foreach (var objectTag in registeredTags)
        {
            var typedValue = new TypedValue((int)objectTag.GroupCode, objectTag.Id);

            resultBuffer.Add(typedValue);
        }

        xRecord.Data = resultBuffer;

        return xRecord;
    }

    /// <inheritdoc/>
    public IObjectIdTagDatabase GetDatabase(string key)
    {
        if (_tagDatabaseCache.TryGetValue(key, out var tagDatabase))
            return tagDatabase;

        tagDatabase = _autoCadDocument.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            using var namedObjectsDictionary = _autoCadDocument.Database.GetNamedObjectsDictionary();

            if (namedObjectsDictionary.TryGetValue(key, out var objectId) == false) 
                return new ObjectIdTagDatabase(key);

            using var dbTagDictionary =
                (DBDictionary)transactionManager.GetObject(objectId!.Unwrap(), OpenMode.ForWrite);

            var existingTagDatabase = this.CreateExistingTagDatabase(dbTagDictionary, transactionManager, key);
                
            return existingTagDatabase;

        });

        _tagDatabaseCache[key] = tagDatabase;

        return tagDatabase;
    }

    /// <inheritdoc/>
    public void CommitAll()
    {
        _ = _autoCadDocument.Transaction(transactionManagerWrapper =>
        {
            using var namedObjectsDictionary = _autoCadDocument.Database.GetNamedObjectsDictionary();
            namedObjectsDictionary.UpgradeOpen();

            foreach (var tagDatabase in _tagDatabaseCache.Values)
            {
                var transactionManager = transactionManagerWrapper.Unwrap();
                
                using var tagDbDictionary = this.GetTagDbDictionary(tagDatabase, namedObjectsDictionary, transactionManager);

                foreach (var tagRecord in tagDatabase)
                {
                    var recordKey = tagRecord.Key;

                    if (tagDbDictionary.Contains(recordKey))
                    {
                        var tagRecordId = tagDbDictionary.GetAt(recordKey);

                        using var oldXRecord = (Xrecord)transactionManager.GetObject(tagRecordId, OpenMode.ForWrite);

                        oldXRecord.Erase();
                    }

                    using var xRecord = this.CreateTagRecord(tagRecord);

                    tagDbDictionary.SetAt(recordKey, xRecord);

                    transactionManager.AddNewlyCreatedDBObject(xRecord, true);
                }
            }

            return true;
        });
    }
}