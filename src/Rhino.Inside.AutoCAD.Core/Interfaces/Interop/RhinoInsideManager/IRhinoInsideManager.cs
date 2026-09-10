namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the manager responsible for coordinating the interaction between Rhino and AutoCAD.
/// </summary>
/// <remarks>
/// This interface provides access to the core instances of Rhino and AutoCAD, enabling seamless
/// integration between the two applications. It acts as the central point for managing the lifecycle
/// and interactions of these instances within the Rhino.Inside.AutoCAD environment.
/// </remarks>
public interface IRhinoInsideManager
{
    /// <summary>
    /// Gets the instance of Rhino that is being managed.
    /// </summary>
    /// <value>
    /// An <see cref="IRhinoInstance"/> representing the Rhino core extension and its associated
    /// functionality, such as the active document and command execution.
    /// </value>
    /// <remarks>
    /// This property provides access to the Rhino environment, including its core extension,
    /// active document, and methods for validating and interacting with Rhino commands.
    /// </remarks>
    IRhinoInstance RhinoInstance { get; }

    /// <summary>
    /// Gets the instance of AutoCAD that is being managed.
    /// </summary>
    /// <value>
    /// An <see cref="IAutoCadInstance"/> representing the AutoCAD application instance, including
    /// its document, validation logger, and various database managers.
    /// </value>
    /// <remarks>
    /// This property provides access to the AutoCAD environment, including its document lifecycle,
    /// event handling, and database management capabilities. It ensures that the AutoCAD instance
    /// is properly managed and synchronized with the Rhino instance.
    /// </remarks>
    IAutoCadInstance AutoCadInstance { get; }

    /// <summary>
    /// Gets the instance of Grasshopper that is being managed.
    /// </summary>
    IGrasshopperInstance GrasshopperInstance { get; }

    /// <summary>
    /// The <see cref="IUnitConverter"/> for managing unit systems between Rhino and AutoCAD.
    /// </summary>
    IUnitConverter UnitConverter { get; }

    /// <summary>
    /// The <see cref="IRhinoObjectPreviewServer"/> for previewing objects between Rhino and
    /// AutoCAD. This is used to manage the transient objects previewed in the AutoCAD viewport.
    /// </summary>
    IRhinoObjectPreviewServer RhinoPreviewServer { get; }

    /// <summary>
    /// The <see cref="IRhinoObjectPreviewServer"/> for previewing objects between grasshopper and
    /// AutoCAD. This is used to manage the transient objects previewed in the AutoCAD viewport.
    /// </summary>
    IGrasshopperObjectPreviewServer GrasshopperPreviewServer { get; }

    /// <summary>
    /// Draws the previews in the given AutoCAD Color Indices from here on, and redraws the
    /// previews which are already on screen in them.
    /// </summary>
    /// <remarks>
    /// Called when the user changes the colors on the settings page. Persisting the choice is
    /// the caller's job; this only applies it to the running session.
    /// </remarks>
    /// <param name="rhinoColorIndex">The color of unselected Rhino previews.</param>
    /// <param name="grasshopperColorIndex">The color of unselected Grasshopper previews.</param>
    /// <param name="selectedColorIndex">The color of selected previews of either kind.</param>
    /// <seealso cref="IUserSettings.RhinoPreviewColorIndex"/>
    void UpdatePreviewColors(int rhinoColorIndex, int grasshopperColorIndex,
        int selectedColorIndex);

    /// <summary>
    /// Requests the preview materials for the active document, creating any that are missing
    /// once AutoCAD is idle.
    /// </summary>
    /// <remarks>
    /// Called from the AutoCAD command that launches Rhino. Materials are otherwise only
    /// requested when a document is activated, which does not happen for the document that
    /// is already active, so without this the first preview of the session finds none.
    /// </remarks>
    void EnsurePreviewMaterials();

    /// <summary>
    /// Shuts down the Rhino.Inside.AutoCAD manager, ensuring all document are saved and
    /// releasing any resources.
    /// </summary>
    void Shutdown();
}