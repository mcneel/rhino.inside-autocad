namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides settings for previewing geometry in AutoCAD.
/// </summary>
public interface IGeometryPreviewSettings
{
    /// <summary>
    /// Gets or sets the AutoCAD Color Index used for the preview geometry.
    /// </summary>
    /// <remarks>
    /// Setting this only changes the previews drawn from here on. Existing previews are
    /// restyled by <see cref="IPreviewServer.RefreshAppearance"/>, and the material named by
    /// <see cref="MaterialName"/> has to be recreated with <see cref="CreateMaterial"/>.
    /// </remarks>
    int ColorIndex { get; set; }

    /// <summary>
    /// Gets the transparency level used for the preview geometry.
    /// </summary>
    byte Transparency { get; }

    /// <summary>
    /// Gets the material ID used for the preview geometry.
    /// </summary>
    IObjectId MaterialId { get; }

    /// <summary>
    /// The name of the material to use.
    /// </summary>
    string MaterialName { get; }

    /// <summary>
    /// Creates the preview material in the AutoCAD database if it does not already exist.
    /// </summary>
    /// <remarks>
    /// This writes to the document's database and so takes a document lock, which AutoCAD
    /// only grants between commands. Call it from an AutoCAD command, or through
    /// <see cref="IPreviewMaterialScheduler"/>, which defers it to the application's idle
    /// loop. Code running under a reactor asks <see cref="HasMaterialFor"/> instead.
    /// </remarks>
    void CreateMaterial(IAutocadDocument document);

    /// <summary>
    /// Returns whether <see cref="MaterialId"/> already references a live material in the
    /// given document's database.
    /// </summary>
    /// <remarks>
    /// A pure test with no side effect, so it is safe to call from the preview path, which
    /// runs under native reactors. The cached id goes stale when the material's document is
    /// closed, the material creation is undone, or a PURGE erases it (transient entities do
    /// not count as database references). It is also stale when it belongs to a different
    /// document than the one previewed into. Previews drawn while this is <c>false</c> are
    /// simply not shaded; <see cref="IPreviewMaterialScheduler"/> creates the material at
    /// the next idle and restyles them.
    /// </remarks>
    bool HasMaterialFor(IAutocadDocument document);

    /// <summary>
    /// Applies these preview settings to the given entity.
    /// </summary>
    void ApplyTo(IEntity entity);
}
