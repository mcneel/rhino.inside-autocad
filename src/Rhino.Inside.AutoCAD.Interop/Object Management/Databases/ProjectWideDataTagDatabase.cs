using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IProjectWideDataTagDatabase"/>
public class ProjectWideDataTagDatabase : IProjectWideDataTagDatabase
{
    /// <inheritdoc />
    public IDataTagDatabase Database { get; }

    /// <inheritdoc />
    public IDbObject DbObjectOwner => this.Database.DbObjectOwner;


    /// <summary>
    /// Constructs a new <see cref="IProjectWideDataTagDatabase"/> instance.
    /// </summary>
    public ProjectWideDataTagDatabase(IDbObject dbObject)
    {
        this.Database = new DataTagDatabase(dbObject);
    }

    /// <inheritdoc />
    public IDataTagRecord GetRecord(string key)
    {
        return this.Database.GetRecord(key);
    }

    /// <inheritdoc />
    public bool TryGetRecord(string key, out IDataTagRecord? dataTagRecord)
    {
        return this.Database.TryGetRecord(key, out dataTagRecord);
    }
}