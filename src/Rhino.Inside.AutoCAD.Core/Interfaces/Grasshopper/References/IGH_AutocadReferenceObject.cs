using Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A reference to an AutoCAD object.
/// </summary>
public interface IGH_AutocadReferenceObject : IGH_AutocadReference
{
    /// <summary>
    /// Gets the AutoCAD object value.
    /// </summary>
    IDbObject ObjectValue { get; }

    /// <summary>
    /// Gets the most update version of the referenced AutoCAD object.
    /// </summary>
    void GetUpdatedObject();
}