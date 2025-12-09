using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;
using CADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IXRecordDictionary"/>
public class XRecordDictionary : IXRecordDictionary
{
    private readonly Dictionary<string, IXRecord> _tagRecords;

    /// <inheritdoc/>
    public IDbObject DbObjectOwner { get; }

    /// <summary>
    /// Constructs a new <see cref="XRecordDictionary"/>.
    /// </summary>
    public XRecordDictionary(IDbObject dbObject)
    {
        _tagRecords = this.GetExistingRecords(dbObject.UnwrapObject());

        this.DbObjectOwner = dbObject;
    }

    /// <summary>
    /// Returns the <see cref="DBDictionary"/> corresponding to the
    /// <see cref="IObjectIdTagDatabase"/>. If no <see cref="DBDictionary"/>
    /// exists, a new one is created and added to the
    /// <see cref="INamedObjectsDictionary"/> in AutoCAD before being
    /// returned.
    /// </summary>
    private Dictionary<string, IXRecord> GetExistingRecords(DBObject dbObject)
    {
        var database = dbObject.Database;

        using var transaction = database.TransactionManager.StartTransaction();

        var extensionDictionaryId = dbObject.ExtensionDictionary;

        var dataRecords = new Dictionary<string, IXRecord>();
        if (extensionDictionaryId != CADObjectId.Null)
        {
            var extensionDictionary = (DBDictionary)transaction.GetObject(extensionDictionaryId, OpenMode.ForRead);

            foreach (var dictionaryEntry in extensionDictionary)
            {
                var key = dictionaryEntry.Key;

                var value = dictionaryEntry.Value;

                using var dictionaryObject = transaction.GetObject(value, OpenMode.ForRead);

                if (dictionaryObject is not Autodesk.AutoCAD.DatabaseServices.Xrecord xRecord) continue;

                using var resultBuffer = xRecord.Data;

                if (resultBuffer == null) continue;

                var dataTagRecord = new XRecord(key);
                foreach (var typedValue in resultBuffer)
                {
                    dataTagRecord.Add((GroupCodeValue)typedValue.TypeCode, typedValue.Value);
                }

                dataRecords.Add(key, dataTagRecord);
            }
        }

        transaction.Commit();

        return dataRecords;
    }

    /// <inheritdoc/>
    public IXRecord GetRecord(string key)
    {
        if (this.TryGetRecord(key, out var dataTagRecord))
            return dataTagRecord!;

        dataTagRecord = new XRecord(key);

        _tagRecords.Add(key, dataTagRecord);

        return dataTagRecord;
    }

    /// <inheritdoc/>
    public bool TryGetRecord(string key, out IXRecord? dataTagRecord)
    {
        return _tagRecords.TryGetValue(key, out dataTagRecord);
    }

    public IEnumerator<IXRecord> GetEnumerator()
    {
        foreach (var dataTagRecord in _tagRecords.Values)
        {
            yield return dataTagRecord;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

