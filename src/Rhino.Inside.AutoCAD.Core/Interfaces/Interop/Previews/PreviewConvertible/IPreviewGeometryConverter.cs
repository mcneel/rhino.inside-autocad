namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A converter that converts Rhino geometries to AutoCAD preview-able entities.
/// </summary>
public interface IPreviewGeometryConverter
{
    /// <summary>
    /// Converts a set of Rhino geometries into AutoCAD preview-able entities.
    /// </summary>
    List<IEntity> Convert(IRhinoConvertibleSet rhinoGeometries, IGeometryPreviewSettings previewSettings);
}