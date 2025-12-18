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
    event EventHandler<IGrasshopperObjectModifiedEventArgs>? OnPreviewExpired;

    /// <summary>
    /// Event raised when a Grasshopper object is removed.
    /// </summary>
    event EventHandler<IGrasshopperObjectModifiedEventArgs>? OnObjectRemoved;

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
    /// Validates that the Grasshopper library is loaded into the Grasshopper
    /// component server.
    /// </summary>
    void ValidateGrasshopperLibrary(IValidationLogger validationLogger);

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