namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A builder which turns a set of Rhino geometries into a single <see cref="IPreviewDrawable"/>.
/// </summary>
public interface IPreviewDrawableBuilder
{
    /// <summary>
    /// Builds an <see cref="IPreviewDrawable"/> which previews the given Rhino geometries,
    /// stopping once <paramref name="maxItems"/> preview items have been built.
    /// </summary>
    /// <param name="rhinoConvertibleSet">The Rhino geometries to preview.</param>
    /// <param name="unselectedSettings">The settings drawn with while not selected.</param>
    /// <param name="selectedSettings">The settings drawn with while selected.</param>
    /// <param name="isSelected">Whether the drawable starts out selected.</param>
    /// <param name="maxItems">The maximum number of preview items to build.</param>
    IPreviewDrawable Build(IRhinoConvertibleSet rhinoConvertibleSet, IGeometryPreviewSettings unselectedSettings,
        IGeometryPreviewSettings selectedSettings, bool isSelected, int maxItems);
}
