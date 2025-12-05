using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A reference to an AutoCAD object.
/// </summary>
public interface IGH_AutocadReference
{
    /// <summary>
    /// The AutoCAD object identifier.
    /// </summary>
    IObjectId Id { get; }
}