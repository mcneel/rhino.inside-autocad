namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides access to all block definitions in an <see cref="IAutocadDocument"/>.
/// </summary>
/// <remarks>
/// Caches and provides lookup for <see cref="IAutocadBlockTableRecord"/> objects including
/// user-defined blocks Accessed via <see cref="IAutocadDocument.BlockTableRecordRegister"/>.
/// Used by Grasshopper components such as GetAutocadBlockTableRecordsComponent and
/// AutocadBlockTableRecordComponent. Supports enumeration to iterate over all block
/// definitions in the document.
/// </remarks>
/// <seealso cref="IAutocadBlockTableRecord"/>
/// <seealso cref="IRegister{T}"/>
/// <seealso cref="IAutocadDocument.BlockTableRecordRegister"/>
public interface IBlockTableRecordRegister :
    IRegister<IAutocadBlockTableRecord>
{
}