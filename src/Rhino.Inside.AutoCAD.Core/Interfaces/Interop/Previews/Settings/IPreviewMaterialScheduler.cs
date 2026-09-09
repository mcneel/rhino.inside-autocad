namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Creates preview materials in the AutoCAD database at a point where AutoCAD permits it.
/// </summary>
/// <remarks>
/// <see cref="IGeometryPreviewSettings.CreateMaterial"/> writes to the database and so takes
/// a document lock, which AutoCAD only grants between commands. Previews are built under
/// Rhino, Grasshopper and AutoCAD reactors, where a lock cannot be taken, so they ask for
/// their material through this scheduler instead: it defers the write to the application's
/// idle loop, then restyles the previews already drawn so they pick the material up.
/// </remarks>
public interface IPreviewMaterialScheduler
{
    /// <summary>
    /// Requests that each of <paramref name="settings"/> has a live preview material in
    /// <paramref name="document"/>, creating any that are missing at the next idle.
    /// </summary>
    /// <remarks>
    /// Safe to call from any context, including reactor callbacks, and safe to call
    /// repeatedly: requests coalesce, and settings which already have a material are
    /// dropped without scheduling anything.
    /// </remarks>
    /// <param name="document">The document the materials belong to.</param>
    /// <param name="settings">The preview settings needing a material.</param>
    void EnsureCreated(IAutocadDocument document, params IGeometryPreviewSettings[] settings);

    /// <summary>
    /// Drops any pending request and detaches from the idle loop.
    /// </summary>
    void Shutdown();
}
