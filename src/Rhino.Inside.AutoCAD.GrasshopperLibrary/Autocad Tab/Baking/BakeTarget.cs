using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Shared helpers for the <see cref="IAutocadBakeable"/> implementations, which all resolve
/// the block table record they append to and apply the same bake settings to the entities
/// they create.
/// </summary>
internal static class BakeTarget
{
    /// <summary>
    /// Resolves the block table record a bake appends its entities to, opened for write.
    /// </summary>
    /// <param name="autocadTransactionManager">
    /// The transaction used to open model space when no target is supplied.
    /// </param>
    /// <param name="targetBlockTableRecord">
    /// The record to bake into, or null to bake into model space.
    /// </param>
    public static BlockTableRecord Resolve(IAutocadTransactionManager autocadTransactionManager,
        IAutocadBlockTableRecord? targetBlockTableRecord)
    {
        var record = targetBlockTableRecord ?? autocadTransactionManager.GetModelSpace(openForWrite: true);

        return record.Unwrap();
    }

    /// <summary>
    /// Applies the bake settings to an entity before it is appended. Properties left
    /// unspecified by the settings are untouched, so the entity keeps its defaults.
    /// </summary>
    public static void ApplySettings(IBakeSettings? settings, Entity entity)
    {
        if (settings is null) return;

        if (settings.Layer != null)
            entity.LayerId = settings.Layer.Id.Unwrap();

        if (settings.LineType != null)
            entity.LinetypeId = settings.LineType.Id.Unwrap();

        if (settings.Color != null)
            entity.Color = settings.Color.Unwrap();

        if (settings.LinetypeScale is double linetypeScale)
            entity.LinetypeScale = linetypeScale;
    }
}
