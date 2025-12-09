namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A Grasshopper Goo interface for AutoCAD geometry that can provide a Rhino geometry
/// equivalent, This is used to display AutoCAD geometry in Grasshopper previews.
/// </summary>
public interface IGH_AutocadGeometryPreview
{
    /// <summary>
    /// Adds the point, wires and meshes to the preview in the autoCAD environment.
    /// </summary>
    public void DrawAutocadPreview(IGrasshopperPreviewData previewData);

}