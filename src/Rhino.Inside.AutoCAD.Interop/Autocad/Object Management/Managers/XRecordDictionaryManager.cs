using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IXRecordDictionaryManager"/>
public class XRecordDictionaryManager : IXRecordDictionaryManager
{
    // ObjectId key is the IDataTagDatabase DbObjectOwner.Id
    private readonly Dictionary<IObjectId, IXRecordDictionary> _dataTagDatabases;

    private readonly IAutocadDocument _autocadDocument;

    /// <inheritdoc/>
    public IProjectWideXRecordDictionary ProjectWideXRecordDictionary { get; }

    /// <summary>
    /// Constructs a new <see cref="XRecordDictionaryManager"/>.
    /// </summary>
    public XRecordDictionaryManager(IAutocadDocument autocadDocument)
    {
        _autocadDocument = autocadDocument;

        _dataTagDatabases = new Dictionary<IObjectId, IXRecordDictionary>(new ObjectIdEqualityComparer());

        this.ProjectWideXRecordDictionary = this.GetProjectWideDatabase(autocadDocument);
    }

    /// <inheritdoc/>
    public IXRecordDictionary GetDatabase(IDbObject dbObject)
    {
        return _dataTagDatabases.TryGetValue(dbObject.Id, out var dataTagDatabase)
            ? dataTagDatabase
            : new XRecordDictionary(dbObject);
    }

    /// <summary>
    /// The project-wide <see cref="IXRecordDictionary"/> for storing <see cref="ITypedValue"/>s
    /// that pertain to the project and have no corresponding <see cref="IDbObject"/>.
    /// </summary>
    private IProjectWideXRecordDictionary GetProjectWideDatabase(IAutocadDocument autocadDocument)
    {
        return autocadDocument.Transaction<IProjectWideXRecordDictionary>(transactionManager =>
        {
            var modelSpace = transactionManager.GetModelSpaceBlockTableRecord();

            var dbObject = new DbObjectWrapper(modelSpace.Unwrap());

            var dataTagDatabase = new ProjectWideXRecordDictionary(dbObject);

            return dataTagDatabase;
        });
    }

    /// <inheritdoc/>
    public void RegisterForCommit(IXRecordDictionary xrecordDictionary)
    {
        var objectId = xrecordDictionary.DbObjectOwner.Id;

        if (_dataTagDatabases.ContainsKey(objectId))
            return;

        _dataTagDatabases.Add(objectId, xrecordDictionary);
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
                        var typedValue = new Autodesk.AutoCAD.DatabaseServices.TypedValue((short)dataTag.GroupCode, dataTag.Value);

                        resultBuffer.Add(typedValue);
                    }

                    Autodesk.AutoCAD.DatabaseServices.Xrecord xRecord;

                    if (extensionDictionary.Contains(key) == false)
                    {
                        xRecord = new Autodesk.AutoCAD.DatabaseServices.Xrecord { Data = resultBuffer };

                        extensionDictionary.SetAt(key, xRecord);

                        transactionManager.AddNewlyCreatedDBObject(xRecord, true);
                    }

                    xRecord = (Autodesk.AutoCAD.DatabaseServices.Xrecord)extensionDictionary.GetAt(key).GetObject(OpenMode.ForWrite);

                    xRecord.Data = resultBuffer;
                }
            }

            return true;
        });
    }
}