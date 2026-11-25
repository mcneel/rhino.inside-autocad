using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Matches the bake settings supplied to a bake component with the objects it bakes.
/// Grasshopper matches the branches of the Objects and Settings trees; within a branch
/// the settings are paired with the objects by index following Grasshopper's longest list
/// convention, so when there are fewer settings than objects the last setting is repeated.
/// </summary>
internal static class BakeSettingsMatcher
{
    /// <summary>
    /// The description of the Settings input shared by the bake components.
    /// </summary>
    public const string SettingsDescription =
        "Optional bake settings (layer, linetype, color), matched to the objects by index. " +
        "When there are fewer settings than objects the last setting is repeated";

    /// <summary>
    /// Sets the Settings input to list access. Definitions saved before the input took a
    /// list store it as item access, which would solve once per setting.
    /// </summary>
    public static void UpgradeSettingsParam(IGH_Param settingsParam)
    {
        settingsParam.Access = GH_ParamAccess.list;
        settingsParam.Description = SettingsDescription;
    }

    /// <summary>
    /// Returns true when the settings cannot be paired one-to-one with the objects. A
    /// single setting is the intended broadcast, not a mismatch.
    /// </summary>
    public static bool IsMismatched(int objectCount, IReadOnlyList<IBakeSettings?> settings)
    {
        return settings.Count > 1 && settings.Count != objectCount;
    }

    /// <summary>
    /// Creates the remark shown when the settings cannot be paired one-to-one with the objects.
    /// </summary>
    public static string MismatchMessage(int objectCount, IReadOnlyList<IBakeSettings?> settings)
    {
        var outcome = settings.Count < objectCount
            ? "the last setting is repeated for the remaining objects"
            : "the extra settings are ignored";

        return $"{settings.Count} settings for {objectCount} objects; {outcome}";
    }

    /// <summary>
    /// Gets the settings for the object at <paramref name="objectIndex"/>.
    /// </summary>
    /// <param name="settings">The settings supplied in this branch.</param>
    /// <param name="objectIndex">The index of the object in the supplied objects.</param>
    /// <returns>
    /// The setting at the same index, the last setting when the index is beyond the end
    /// of the settings, or null when no settings are supplied.
    /// </returns>
    public static IBakeSettings? ForIndex(IReadOnlyList<IBakeSettings?> settings, int objectIndex)
    {
        if (settings.Count == 0)
            return null;

        var settingsIndex = Math.Min(objectIndex, settings.Count - 1);

        return settings[settingsIndex];
    }
}
