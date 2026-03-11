namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents a wrapper for an AutoCAD Database.
/// </summary>
public interface IDatabase : IDisposable
{
    /// <summary>
    /// Returns the BlockTableId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId BlockTableId { get; }

    /// <summary>
    /// Returns the LinetypeTableId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId LinetypeTableId { get; }

    /// <summary>
    /// Returns the LayerTableId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId LayerTableId { get; }

    /// <summary>
    /// Returns the LayoutDictionaryId of this <see cref="IDatabase"/>.
    /// </summary>
    IObjectId LayoutDictionaryId { get; }
}