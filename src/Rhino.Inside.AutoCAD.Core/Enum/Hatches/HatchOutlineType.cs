using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// An enum representing enclosure type of <see cref="IHatchLoop"/>.
/// </summary>
public enum HatchOutlineType
{
    /// <summary>
    /// An unsupported <see cref="HatchOutlineType"/>.
    /// </summary>
    Unsupported,

    /// <summary>
    /// A <see cref="IHatchLoop"/> or <see cref="IHatch"/> forms a closed
    /// loop.
    /// </summary>
    Closed,

    /// <summary>
    /// A <see cref="IHatchLoop"/> or <see cref="IHatch"/> is open.
    /// </summary>
    Open,

    /// <summary>
    /// A <see cref="IHatchLoop"/> or <see cref="IHatch"/> with self intersecting
    /// geometry.
    /// </summary>
    SelfIntersecting,

    /// <summary>
    /// A duplicate <see cref="IHatchLoop"/>.
    /// </summary>
    Duplicate
}