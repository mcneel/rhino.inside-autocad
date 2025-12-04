namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a repository for handling <see cref="IDimensionStyleTableRecord"/>s.
/// </summary>
public interface IDimensionStyleTableRecordRepository : IRepository<IDimensionStyleTableRecord>
{
    /// <summary>
    /// Returns the default <see cref="IDimensionStyleTableRecord"/> from the active
    /// <see cref="IAutocadDocument"/>, basically the first item of the
    /// <see cref="IDimensionStyleTableRecordRepository"/>.
    /// </summary>
    IDimensionStyleTableRecord GetDefault();
}