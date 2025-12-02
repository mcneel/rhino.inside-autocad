namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a name of the <see cref="ICustomProperty"/>. It's used as
/// a mapping name between internal names and names of the <see cref=
/// "Autodesk.AutoCAD.DatabaseServices.DynamicBlockReferenceProperty"/>s
/// of the <see cref="IBlockReference"/>.
/// </summary>
public interface ICustomPropertyName
{
    /// <summary>
    /// Type of the <see cref="ICustomProperty"/>.
    /// </summary>
    CustomPropertyType Type { get; }

    /// <summary>
    /// Relative property name of the <see cref="IBlockTableRecord"/>.
    /// </summary>
    string Name { get; }
}