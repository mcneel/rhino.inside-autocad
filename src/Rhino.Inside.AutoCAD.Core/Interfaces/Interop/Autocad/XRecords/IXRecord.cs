namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a wrapped AutoCAD Xrecord object.
/// </summary>
public interface IXRecord : IDbObject
{
    /// <summary>
    /// Gets the typed values stored in the XRecord as a list of (type code, value) pairs.
    /// The type code represents the DXF group code, and the value is the associated data.
    /// </summary>
    IReadOnlyList<(short TypeCode, object Value)> Data { get; }

    /// <summary>
    /// Creates a shallow clone of the <see cref="IXRecord"/>.
    /// </summary>
    new IXRecord ShallowClone();
}