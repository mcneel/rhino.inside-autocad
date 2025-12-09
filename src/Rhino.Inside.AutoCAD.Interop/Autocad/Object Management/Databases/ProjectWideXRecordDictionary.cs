using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IProjectWideXRecordDictionary"/>
public class ProjectWideXRecordDictionary : IProjectWideXRecordDictionary
{
    /// <inheritdoc />
    public IXRecordDictionary Database { get; }

    /// <inheritdoc />
    public IDbObject DbObjectOwner => this.Database.DbObjectOwner;


    /// <summary>
    /// Constructs a new <see cref="IProjectWideXRecordDictionary"/> instance.
    /// </summary>
    public ProjectWideXRecordDictionary(IDbObject dbObject)
    {
        this.Database = new XRecordDictionary(dbObject);
    }

    /// <inheritdoc />
    public IXRecord GetRecord(string key)
    {
        return this.Database.GetRecord(key);
    }

    /// <inheritdoc />
    public bool TryGetRecord(string key, out IXRecord? dataTagRecord)
    {
        return this.Database.TryGetRecord(key, out dataTagRecord);
    }
}