namespace Rhino.Inside.AutoCAD.UI.Resources.Models;

/// <summary>
/// Provides constant values used throughout the Rhino.Inside.AutoCAD UI.
/// </summary>
public class UIConstants
{
    /// <summary>
    /// The URL for the Rhino.Inside.AutoCAD documentation.
    /// </summary>
    public const string DocumentationUrl = "https://www.bimorph.com/products/rhino-inside-autocad";

    /// <summary>
    /// The URL for the Rhino.Inside.AutoCAD forum.
    /// </summary>
    public const string ForumUrl = "https://discourse.mcneel.com/c/rhino-inside/autocad/185";

    /// <summary>
    /// The URL for contacting Bimorph.
    /// </summary>
    public const string BimorphUrl = "https://bimorph.com/contact/";

    /// <summary>
    /// The message displayed when the version cannot be determined.
    /// </summary>
    public const string NotDetermined = "The version could not be determined.";

    /// <summary>
    /// The message prompting the user to launch Rhino.Inside to determine the version.
    /// </summary>
    public const string OpenForVersion = "Launch Rhino Inside to determine the version";

    /// <summary>
    /// Format string for the label of the button which uses the selected Rhino version for
    /// this session only. The placeholder receives the version's display name.
    /// </summary>
    /// <seealso cref="AlwaysUseVersionButtonFormat"/>
    public const string UseVersionButtonFormat = "Use {0}";

    /// <summary>
    /// Format string for the label of the button which uses the selected Rhino version and
    /// stops asking. The placeholder receives the version's display name.
    /// </summary>
    /// <seealso cref="UseVersionButtonFormat"/>
    public const string AlwaysUseVersionButtonFormat = "Always use {0}";

    /// <summary>
    /// Stands in for a version's display name in the selection dialog's button labels while
    /// nothing is selected, when those buttons are disabled anyway.
    /// </summary>
    /// <seealso cref="UseVersionButtonFormat"/>
    public const string RhinoFallbackName = "Rhino";

    /// <summary>
    /// The note shown wherever the Rhino version can be changed, explaining that the change
    /// only takes effect on the next AutoCAD session.
    /// </summary>
    /// <remarks>
    /// The assembly resolvers which bind Rhino.Inside to a Rhino installation are registered
    /// once per process and cannot be re-pointed, so the version cannot be switched in place.
    /// </remarks>
    public const string RhinoVersionRestartNote =
        "Changing the Rhino version takes effect the next time AutoCAD starts.";

    /// <summary>
    /// Format string for the name of an AutoCAD Color Index. The placeholder receives the
    /// index.
    /// </summary>
    public const string AciColorNameFormat = "ACI {0}";

    /// <summary>
    /// The heading of the preview colors section of the settings page.
    /// </summary>
    public const string PreviewColorsHeading = "Preview Colours";

    /// <summary>
    /// The description of the preview colors section of the settings page.
    /// </summary>
    public const string PreviewColorsDescription =
        "The AutoCAD colours the Rhino and Grasshopper previews are drawn in. Changes apply " +
        "to the previews on screen straight away.";

    /// <summary>
    /// The label of the color used for unselected Rhino previews.
    /// </summary>
    public const string RhinoPreviewColorLabel = "Rhino previews";

    /// <summary>
    /// The label of the color used for unselected Grasshopper previews.
    /// </summary>
    public const string GrasshopperPreviewColorLabel = "Grasshopper previews";

    /// <summary>
    /// The label of the color used for selected previews of either kind.
    /// </summary>
    public const string SelectedPreviewColorLabel = "Selected previews";

    /// <summary>
    /// The label of the most entities each preview server draws.
    /// </summary>
    public const string PreviewEntityLimitLabel = "Preview entity limit";

    /// <summary>
    /// The tooltip explaining the preview entity limit.
    /// </summary>
    public const string PreviewEntityLimitToolTip =
        "The most AutoCAD entities drawn for the Rhino preview, and for each of the " +
        "Grasshopper shaded and wireframe previews. When a new preview exceeds it, the " +
        "oldest previews are removed. Large values can make AutoCAD unresponsive.";
}