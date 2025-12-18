namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A grasshopper Goo reference to an AutoCAD object of any type.
/// </summary>
public interface IGH_AutocadReference
{
    /// <summary>
    /// The AutoCAD object identifier.
    /// </summary>
    IAutocadReferenceId Reference { get; }
}