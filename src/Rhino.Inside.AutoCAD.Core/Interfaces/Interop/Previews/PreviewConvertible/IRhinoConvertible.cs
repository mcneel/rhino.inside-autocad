namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An object that can be converted from a Rhino object. These are used extensively by
/// the code base, by the preview infrastructure as they can provide bulk conversion
/// using a single autocad transaction, rather than converting objects one at a time
/// with their own transaction. Each <see cref="IRhinoConvertible"/> wraps a single
/// Rhino Geometry object, but can be converted to multiple <see cref="IEntity"/>
/// objects in AutoCAD.
/// </summary>
public interface IRhinoConvertible
{
    /// <summary>
    /// Converts the Rhino object to a list of <see cref="IEntity"/> objects using
    /// the provided <see cref="ITransactionManager"/>. The <see
    /// cref="IGeometryPreviewSettings"/> are applied to the object. This is used
    /// primarily for preview purposes.
    /// </summary>
    List<IEntity> Convert(ITransactionManager transactionManager, IGeometryPreviewSettings previewSettings);

    /// <summary>
    /// Converts the Rhino object to a list of <see cref="IEntity"/> objects using
    /// the provided <see cref="ITransactionManager"/>.
    /// </summary>
    List<IEntity> Convert(ITransactionManager transactionManager);
}