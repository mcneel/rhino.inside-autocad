namespace Rhino.Inside.AutoCAD.Core.Interfaces;
/// <summary>
/// Represents a repository for storing <see cref="ITextStyleTableRecord"/>s of the
/// active <see cref="IDocument"/>.
/// </summary>
public interface ITextStyleTableRecordRepository : IRepository<ITextStyleTableRecord>
{
    /// <summary>
    /// Returns the default <see cref="ITextStyleTableRecord"/> from the active <see 
    /// cref="IDocument"/>, basically the first item of the <see cref=
    /// "ITextStyleTableRecordRepository"/>. The default one is used when no required
    /// text style is found. It's guarantee at least one text style is available in the
    /// active <see cref="IDocument"/>.
    /// </summary>
    ITextStyleTableRecord GetDefault();
}