using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDataTagDatabaseManager"/>
public class DataTagDatabaseManager : IDataTagDatabaseManager
{
    // ObjectId key is the IDataTagDatabase DbObjectOwner.Id
    private readonly Dictionary<IObjectId, IDataTagDatabase> _dataTagDatabases;

    private readonly IAutocadDocument _autocadDocument;

    /// <inheritdoc/>
    public IProjectWideDataTagDatabase ProjectWideDataTagDatabase { get; }

    /// <summary>
    /// Constructs a new <see cref="DataTagDatabaseManager"/>.
    /// </summary>
    public DataTagDatabaseManager(IAutocadDocument autocadDocument)
    {
        _autocadDocument = autocadDocument;

        _dataTagDatabases = new Dictionary<IObjectId, IDataTagDatabase>(new ObjectIdEqualityComparer());

        this.ProjectWideDataTagDatabase = this.GetProjectWideDatabase(autocadDocument);
    }

    /// <inheritdoc/>
    public IDataTagDatabase GetDatabase(IDbObject dbObject)
    {
        return _dataTagDatabases.TryGetValue(dbObject.Id, out var dataTagDatabase)
            ? dataTagDatabase
            : new DataTagDatabase(dbObject);
    }

    /// <summary>
    /// The project-wide <see cref="IDataTagDatabase"/> for storing <see cref="IDataTag"/>s
    /// that pertain to the project and have no corresponding <see cref="IDbObject"/>.
    /// </summary>
    private IProjectWideDataTagDatabase GetProjectWideDatabase(IAutocadDocument autocadDocument)
    {
        return autocadDocument.Transaction<IProjectWideDataTagDatabase>(transactionManager =>
        {
            var modelSpace = transactionManager.GetModelSpaceBlockTableRecord();

            var dbObject = new DbObjectWrapper(modelSpace.Unwrap());

            var dataTagDatabase = new ProjectWideDataTagDatabase(dbObject);

            return dataTagDatabase;
        });
    }

    /// <inheritdoc/>
    public void RegisterForCommit(IDataTagDatabase dataTagDatabase)
    {
        var objectId = dataTagDatabase.DbObjectOwner.Id;

        if (_dataTagDatabases.ContainsKey(objectId))
            return;

        _dataTagDatabases.Add(objectId, dataTagDatabase);
    }

    /// <inheritdoc/>
    public void CommitAll()
    {
        _ = _autocadDocument.Transaction(transactionManagerWrapper =>
        {
            var transactionManager = transactionManagerWrapper.Unwrap();

            foreach (var dataTagDatabase in _dataTagDatabases.Values)
            {
                var dbObjectOwner = dataTagDatabase.DbObjectOwner;

                if (dbObjectOwner.IsValid == false) continue;

                var dbObject = dataTagDatabase.DbObjectOwner.UnwrapObject();

                var extensionDictionaryId = dbObject.ExtensionDictionary;

                if (extensionDictionaryId.IsNull)
                {
                    using var dbObjectOpened = transactionManager.GetObject(dbObject.Id, OpenMode.ForWrite);

                    dbObjectOpened.CreateExtensionDictionary();

                    extensionDictionaryId = dbObjectOpened.ExtensionDictionary;
                }

                using var extensionDictionary = (DBDictionary)transactionManager.GetObject(extensionDictionaryId, OpenMode.ForWrite);

                foreach (var tagRecord in dataTagDatabase)
                {
                    var key = tagRecord.Key;

                    using var resultBuffer = new ResultBuffer();

                    foreach (var dataTag in tagRecord)
                    {
                        var typedValue = new TypedValue((short)dataTag.GroupCode, dataTag.Value);

                        resultBuffer.Add(typedValue);
                    }

                    Xrecord xRecord;

                    if (extensionDictionary.Contains(key) == false)
                    {
                        xRecord = new Xrecord { Data = resultBuffer };

                        extensionDictionary.SetAt(key, xRecord);

                        transactionManager.AddNewlyCreatedDBObject(xRecord, true);
                    }

                    xRecord = (Xrecord)extensionDictionary.GetAt(key).GetObject(OpenMode.ForWrite);

                    xRecord.Data = resultBuffer;
                }
            }

            return true;
        });
    }
}