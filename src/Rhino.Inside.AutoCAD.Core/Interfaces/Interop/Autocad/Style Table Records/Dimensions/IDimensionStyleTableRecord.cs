namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped Autodesk.AutoCAD.DatabaseServices.DimStyleTableRecord
/// </summary>
public interface IDimensionStyleTableRecord : IDbObject
{
    /// <summary>
    /// The name of the <see cref="IDimensionStyleTableRecord"/>.
    /// </summary>
    string Name { get; }
}