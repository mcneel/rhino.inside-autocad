namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped AutoCAD TextStyleTableRecord.
/// </summary>
public interface ITextStyleTableRecord : IDbObject
{
    /// <summary>
    /// The name of the <see cref="ITextStyleTableRecord"/>.
    /// </summary>
    string Name { get; }
}