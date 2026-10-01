using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an instance of the Grasshopper application within the Rhino.Inside.AutoCAD
/// environment.
/// </summary>
/// <remarks>
/// This interface provides access to the core functionality of Grasshopper, including its
/// active document, and methods disabling and recalculating the solution. It acts as the
/// primary entry point for managing the Grasshopper instance.
/// </remarks>
public interface IGrasshopperInstance
{
    /// <summary>
    /// Event fired when a Grasshopper object preview expires.
    /// </summary>
    event EventHandler<IGrasshopperObjectModifiedEventArgs>? PreviewExpired;

    /// <summary>
    /// Event raised when a Grasshopper object is removed.
    /// </summary>
    event EventHandler<IGrasshopperObjectModifiedEventArgs>? ObjectRemoved;

    /// <summary>
    /// The event fired when the selection of Grasshopper components changes. This is used to update
    /// the preview geometry in AutoCAD to reflect the selection state of the Grasshopper components.
    /// </summary>
    event EventHandler<IGrasshopperSelectionEventArgs>? ComponentSelectionChanged;

    /// <summary>
    /// Event raised when the document shown on the Grasshopper canvas changes, including
    /// when the last document is closed. Raised before the new document's previews are
    /// requested through <see cref="PreviewExpired"/>, so the previous document's previews
    /// can be cleared first.
    /// </summary>
    event EventHandler? ActiveDocumentChanged;

    /// <summary>
    /// Event raised when the Grasshopper editor window is minimised, restored, hidden or
    /// shown.
    /// </summary>
    /// <seealso cref="IsEditorMinimised"/>
    /// <seealso cref="IsEditorHidden"/>
    event EventHandler? EditorDisplayStateChanged;

    /// <summary>
    /// The current active Rhino document.
    /// </summary>
    GH_Document? ActiveDoc { get; }

    /// <summary>
    /// The version of the Grasshopper application.
    /// </summary>
    Version ApplicationVersion { get; }

    /// <summary>
    /// A value indicating whether the Grasshopper solver is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Access to the Grasshopper editor window manager, which watches the editor for being
    /// minimised, restored, hidden or shown.
    /// </summary>
    IGrasshopperWindowManager WindowManager { get; }

    /// <summary>
    /// True while the Grasshopper editor window is minimised. False before the editor has
    /// been opened.
    /// </summary>
    bool IsEditorMinimised { get; }

    /// <summary>
    /// True while the Grasshopper editor window exists but is hidden, as it is after the
    /// user closes it, which hides the editor in place of destroying it. False before the
    /// editor has been opened.
    /// </summary>
    bool IsEditorHidden { get; }

    /// <summary>
    /// Validates that the Grasshopper library is loaded into the Grasshopper
    /// component server.
    /// </summary>
    void ValidateGrasshopperLibrary(IStartUpLogger startUpLogger);

    /// <summary>
    /// Recomputes the Grasshopper solution in the active Grasshopper document.
    /// </summary>
    void RecomputeSolution();

    /// <summary>
    /// Locks (Disables) the Grasshopper solver.
    /// </summary>
    void DisableSolver();

    /// <summary>
    /// Unlocks (enables) the Grasshopper solver.
    /// </summary>
    void EnableSolver();

    /// <summary>
    /// The steps to take to shutdown this plugin.
    /// </summary>
    void Shutdown();
}