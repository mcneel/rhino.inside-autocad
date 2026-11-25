using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Matches the bake settings supplied to a bake component with the objects it bakes.
/// Grasshopper matches the branches of the Objects and Settings trees; within a branch
/// the settings are paired with the objects by index when the counts agree, otherwise the
/// first setting is applied to every object.
/// </summary>
internal static class BakeSettingsMatcher
{
    /// <summary>
    /// The description of the Settings input shared by the bake components.
    /// </summary>
    public const string SettingsDescription =
        "Optional bake settings (layer, linetype, color). One setting per object is matched by index; " +
        "otherwise the first setting is applied to all objects";

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
    /// Returns true when the settings cannot be paired by index, so the first setting is
    /// applied to every object. A single setting is the intended broadcast, not a mismatch.
    /// </summary>
    public static bool IsMismatched(int objectCount, IReadOnlyList<IBakeSettings?> settings)
    {
        return settings.Count > 1 && settings.Count != objectCount;
    }

    /// <summary>
    /// Gets the settings for the object at <paramref name="objectIndex"/>.
    /// </summary>
    /// <param name="objectCount">The number of objects supplied in this branch.</param>
    /// <param name="settings">The settings supplied in this branch.</param>
    /// <param name="objectIndex">The index of the object in the supplied objects.</param>
    /// <returns>
    /// The setting at the same index when the counts agree, the first setting otherwise,
    /// or null when no settings are supplied.
    /// </returns>
    public static IBakeSettings? ForIndex(int objectCount, IReadOnlyList<IBakeSettings?> settings,
        int objectIndex)
    {
        if (settings.Count == 0)
            return null;

        if (settings.Count == objectCount)
            return settings[objectIndex];

        return settings[0];
    }

    /// <summary>
    /// Creates the remark shown when the settings cannot be paired with the objects by index.
    /// </summary>
    public static string MismatchMessage(int objectCount, IReadOnlyList<IBakeSettings?> settings)
    {
        return $"{settings.Count} settings for {objectCount} objects; the first setting is applied to all";
    }
}
