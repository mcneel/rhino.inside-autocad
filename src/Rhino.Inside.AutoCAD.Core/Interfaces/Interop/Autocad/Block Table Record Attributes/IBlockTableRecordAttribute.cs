namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents attributes of <see cref="IBlockTableRecord"/>.
/// </summary>
public interface IBlockTableRecordAttribute
{
    /// <summary>
    /// Tag of the <see cref="Autodesk.AutoCAD.DatabaseServices.AttributeReference"/> in 
    /// the <see cref="IBlockTableRecord"/> within the active <see cref="IAutocadDocument"/>. 
    /// </summary>
    string Tag { get; }
}