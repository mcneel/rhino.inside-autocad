using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Extracts preview geometry data from Grasshopper document objects.
/// </summary>
public interface IGrasshopperGeometryExtractor
{
    /// <summary>
    /// Extracts preview geometry data from a Grasshopper document object.
    /// </summary>
    /// <param name="ghDocumentObject">
    /// The Grasshopper document object to extract geometry from.
    /// </param>
    /// <returns>
    /// The extracted preview geometry data.
    /// </returns>
    IGrasshopperPreviewData ExtractPreviewGeometry(IGH_DocumentObject ghDocumentObject);
}
