using Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A Grasshopper Goo reference to an AutoCAD database object.
/// </summary>
public interface IGH_AutocadReferenceDatabaseObject : IGH_AutocadReference
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