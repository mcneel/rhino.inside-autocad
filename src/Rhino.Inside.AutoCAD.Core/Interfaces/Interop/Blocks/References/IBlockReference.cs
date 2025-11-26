namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface representing a wrapped AutoCAD BlockReference
/// </summary>
public interface IBlockReference : IEntity
{
    /// <summary>
    /// The <see cref="ICustomProperty"/>s of this <see cref="IBlockReference"/>.
    /// </summary>
    ICustomPropertySet CustomProperties { get; }

    /// <summary>
    /// The rotation of this <see cref="IBlockReference"/> in radians.
    /// </summary>
    double Rotation { get; }

    /// <summary>
    /// The name of the <see cref="IBlockReference"/>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Adds <see cref="ICustomProperty"/>s to the <see cref="CustomProperties"/>
    /// from the <see cref="ICustomPropertySet"/>.
    /// </summary>
    void AddCustomProperties(ICustomPropertySet customProperties);

    /// <summary>
    /// Commits the custom properties of the <see cref="IBlockReference"/> in the active
    /// <see cref="IDocument"/>.
    /// </summary>
    void CommitCustomProperties();
}