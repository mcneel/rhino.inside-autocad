using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which represents an AutoCAD attribute reference.
/// </summary>
public interface IAttributeWrapper : IDbObject
{
    /// <summary>
    /// The tag of the attribute. This is the name used to identify the attribute within
    /// a block definition.
    /// </summary>
    string Tag { get; }

    /// <summary>
    /// The text content of the attribute.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Boolean value indicating whether the attribute is multiline Text (MText).
    /// </summary>
    bool IsMultiline { get; }

    /// <summary>
    /// The alignment point of the attribute.
    /// </summary>
    Point3d AlignmentPoint { get; }
}