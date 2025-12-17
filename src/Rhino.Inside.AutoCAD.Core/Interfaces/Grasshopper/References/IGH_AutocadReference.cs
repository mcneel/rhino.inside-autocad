namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A reference to an AutoCAD object.
/// </summary>
public interface IGH_AutocadReference
{
    /// <summary>
    /// The AutoCAD object identifier.
    /// </summary>
    IAutocadReferenceId Reference { get; }
}
