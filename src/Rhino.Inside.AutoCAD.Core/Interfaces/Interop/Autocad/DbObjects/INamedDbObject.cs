namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A subinterface of <see cref="IDbObject"/> which represents a named AutoCAD database
/// object for example a layer, line type, or block definition.
/// </summary>
/// <seealso cref="IDbObject"/>
public interface INamedDbObject : IDbObject
{
    /// <summary>
    /// The name of the <see cref="INamedDbObject"/>.
    /// </summary>
    string Name { get; }
}