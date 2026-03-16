using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// Represents the types of changes that are supported by <see cref="IAutocadDocumentChange"/>
/// in an<see cref="IAutocadDocument"/>.
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// Indicates that an object was created in the AutoCAD document.
    /// </summary>
    ObjectCreated,

    /// <summary>
    /// Indicates that an object was modified in the AutoCAD document.
    /// </summary>
    ObjectModified,

    /// <summary>
    /// Indicates that an object was erased from the AutoCAD document.
    /// </summary>
    ObjectErased,

    /// <summary>
    /// Indicates that the units of the AutoCAD document were changed.
    /// </summary>
    UnitsChanged
}