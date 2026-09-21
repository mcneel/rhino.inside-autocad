namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class containing the user-facing strings shown on Grasshopper
/// components: canvas button captions, message capsule text, runtime messages,
/// context menu items and their tooltips.
/// </summary>
/// <remarks>
/// Members suffixed <c>Format</c> are composite format strings intended for
/// <see cref="string.Format(string, object?[])"/>.
/// </remarks>
public class GrasshopperMessages
{
    /// <summary>
    /// Caption of the canvas button that manually triggers a bake.
    /// </summary>
    public const string BakeButton = "Bake";

    /// <summary>
    /// Caption of the canvas button that manually runs a filter query.
    /// </summary>
    public const string QueryButton = "Query";

    /// <summary>
    /// Caption of the canvas button that clears stale state and re-solves a component.
    /// </summary>
    public const string RefreshButton = "Refresh";

    /// <summary>
    /// The message capsule text shown while a component's data is stale.
    /// </summary>
    public const string StaleDataMessage = "Stale Data";

    /// <summary>
    /// Format string describing how many tracked objects were added.
    /// Argument 0 is the count.
    /// </summary>
    public const string StaleAddedFormat = "{0} added";

    /// <summary>
    /// Format string describing how many tracked objects were deleted.
    /// Argument 0 is the count.
    /// </summary>
    public const string StaleDeletedFormat = "{0} deleted";

    /// <summary>
    /// Format string describing how many tracked objects were modified.
    /// Argument 0 is the count.
    /// </summary>
    public const string StaleModifiedFormat = "{0} modified";

    /// <summary>
    /// Format string for the runtime remark describing the accumulated changes,
    /// for example "Data is stale: 2 added, 1 deleted. Press Refresh to update.".
    /// Argument 0 is the comma separated list of change descriptions.
    /// </summary>
    /// <seealso cref="StaleAddedFormat"/>
    /// <seealso cref="StaleDeletedFormat"/>
    /// <seealso cref="StaleModifiedFormat"/>
    public const string StaleDescriptionFormat = "Data is stale: {0}. Press Refresh to update.";

    /// <summary>
    /// The message capsule text shown when a component tracks exactly one object.
    /// </summary>
    /// <seealso cref="TrackingObjectsFormat"/>
    public const string TrackingSingleObject = "Tracking 1 Object";

    /// <summary>
    /// Format string for the message capsule text shown when a component tracks more
    /// than one object. Argument 0 is the count.
    /// </summary>
    /// <seealso cref="TrackingSingleObject"/>
    public const string TrackingObjectsFormat = "Tracking {0} Objects";

    /// <summary>
    /// The runtime warning shown when exactly one object picked in AutoCAD could not be
    /// converted to a parameter's type and was left out. Argument 0 is the type name.
    /// </summary>
    /// <seealso cref="SkippedSelectionFormat"/>
    public const string SkippedSelectionSingleFormat = "1 selected object could not be converted to {0} and was skipped.";

    /// <summary>
    /// The runtime warning shown when more than one object picked in AutoCAD could not be
    /// converted to a parameter's type and was left out. Argument 0 is the count,
    /// argument 1 is the type name.
    /// </summary>
    /// <seealso cref="SkippedSelectionSingleFormat"/>
    public const string SkippedSelectionFormat = "{0} selected objects could not be converted to {1} and were skipped.";

    /// <summary>
    /// Label of the Auto Update toggle menu item.
    /// </summary>
    public const string AutoUpdateMenuItem = "Auto Update";

    /// <summary>
    /// Label of the Driven Button toggle menu item.
    /// </summary>
    public const string DrivenButtonMenuItem = "Driven Button";

    /// <summary>
    /// Label of the Replace Previous Object toggle menu item.
    /// </summary>
    public const string ReplacePreviousObjectMenuItem = "Replace Previous Object";

    /// <summary>
    /// Label of the Save Connection Between Sessions toggle menu item.
    /// </summary>
    public const string SaveConnectionMenuItem = "Save Connection Between Sessions";

    /// <summary>
    /// Label of the Forget Connections menu item.
    /// </summary>
    public const string ForgetConnectionsMenuItem = "Forget Connections";

    /// <summary>
    /// Label of the Delete Connected Objects menu item, also used as the caption of
    /// its confirmation dialog.
    /// </summary>
    public const string DeleteConnectedObjectsMenuItem = "Delete Connected Objects";

    /// <summary>
    /// Tooltip for the Auto Update toggle on components that show a Refresh button
    /// when their data goes stale.
    /// </summary>
    /// <remarks>
    /// Deliberately worded differently from <see cref="AutoUpdateFilterTooltip"/>,
    /// which names a different button.
    /// </remarks>
    public const string AutoUpdateStaleTooltip = "When enabled, the component automatically updates when the AutoCAD document changes. " +
                                                 "When disabled, document changes mark the data as stale until Refresh is pressed.";

    /// <summary>
    /// Tooltip for the Auto Update toggle on the filter query component.
    /// </summary>
    /// <seealso cref="AutoUpdateStaleTooltip"/>
    public const string AutoUpdateFilterTooltip = "When enabled, the component automatically updates when the AutoCAD document changes. " +
                                                  "When disabled, use the Query button to manually refresh.";

    /// <summary>
    /// Tooltip for the Driven Button toggle on the bake component.
    /// </summary>
    /// <remarks>
    /// The button and the Bake input are alternatives, never shown together, so the
    /// tooltip describes the toggle as a swap between the two.
    /// </remarks>
    public const string DrivenButtonTooltip = "When enabled, a Bake button is shown on the component for manually triggering a bake. When disabled, the button is replaced by a Bake input that triggers the bake when it is true.";

    /// <summary>
    /// Tooltip for the Replace Previous Object toggle on creation components.
    /// </summary>
    public const string ReplacePreviousObjectTooltip = "When enabled, previously created objects will be deleted before creating new ones.";

    /// <summary>
    /// Tooltip for the Save Connection Between Sessions toggle on creation components.
    /// </summary>
    public const string SaveConnectionTooltip = "When enabled, object connections persist when saving/loading the Grasshopper file.";

    /// <summary>
    /// Tooltip for the Forget Connections menu item on creation components.
    /// </summary>
    public const string ForgetConnectionsTooltip = "Forgets all tracked object connections without deleting the objects from AutoCAD.";

    /// <summary>
    /// Tooltip for the Delete Connected Objects menu item on creation components.
    /// </summary>
    public const string DeleteConnectedObjectsTooltip = "Deletes all tracked objects from the AutoCAD database.";

    /// <summary>
    /// Describes a single tracked object in the delete confirmation prompt.
    /// </summary>
    /// <seealso cref="ConnectedObjectsFormat"/>
    public const string SingleConnectedObject = "1 connected object";

    /// <summary>
    /// Format string describing more than one tracked object in the delete
    /// confirmation prompt. Argument 0 is the count.
    /// </summary>
    /// <seealso cref="SingleConnectedObject"/>
    public const string ConnectedObjectsFormat = "{0} connected objects";

    /// <summary>
    /// Format string for the delete confirmation prompt. Argument 0 is the object
    /// description.
    /// </summary>
    /// <seealso cref="SingleConnectedObject"/>
    /// <seealso cref="ConnectedObjectsFormat"/>
    public const string DeleteConnectedObjectsPromptFormat = "This will permanently delete {0} from the AutoCAD database.\n\n" +
                                                             "Are you sure you want to continue?";
}
