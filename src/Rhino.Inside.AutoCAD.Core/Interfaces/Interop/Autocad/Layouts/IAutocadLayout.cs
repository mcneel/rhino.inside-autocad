namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an AutoCAD layout, which defines a paper space view for plotting and printing.
/// </summary>
/// <remarks>
/// Wraps an AutoCAD Layout object to expose its properties. Layouts include both the
/// "Model" tab and paper space layouts (e.g., "Layout1", "Layout2"). Each layout has an
/// associated block table record that contains the entities visible in that layout.
/// Used by Grasshopper components including AutocadLayoutComponent, GetAutocadLayoutsComponent,
/// and CreateAutocadLayoutComponent.
/// </remarks>
/// <seealso cref="ILayoutRegister"/>
/// <seealso cref="IAutocadBlockTableRecord"/>
/// <seealso cref="INamedDbObject"/>
public interface IAutocadLayout : INamedDbObject
{
    /// <summary>
    /// Gets the <see cref="IObjectId"/> of the block table record containing this layout's entities.
    /// </summary>
    /// <remarks>
    /// Each layout has a dedicated block table record. For the Model layout, this references
    /// "*Model_Space"; for paper space layouts, it references "*Paper_Space" or "*Paper_Space0", etc.
    /// </remarks>
    /// <seealso cref="IAutocadBlockTableRecord"/>
    IObjectId BlockTableRecordId { get; }

    /// <summary>
    /// Gets the tab display order for this layout in the AutoCAD interface.
    /// </summary>
    /// <remarks>
    /// The Model tab always has order 0. Paper space layouts are ordered starting from 1,
    /// corresponding to their left-to-right position in the layout tab bar.
    /// </remarks>
    int TabOrder { get; }
}