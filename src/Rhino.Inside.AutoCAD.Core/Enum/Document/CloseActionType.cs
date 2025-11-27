using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// An enum storing actions that determine how an <see cref="IAutoCadDocument"/>
/// and the underlying AutoCAD document should be closed.
/// </summary>
public enum CloseActionType
{
    /// <summary>
    /// Leaves the active underlying AutoCAD document unchanged.
    /// </summary>
    Unmodified,

    /// <summary>
    /// Closes the <see cref="IAutoCadDocument"/> and saves the active underlying
    /// AutoCAD document without closing it.
    /// </summary>
    Save,

    /// <summary>
    /// Closes the <see cref="IAutoCadDocument"/> and saves and closes the underlying
    /// AutoCAD document. 
    /// </summary>
    SaveAndClose
}