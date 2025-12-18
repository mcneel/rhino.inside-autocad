using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a change event for a Grasshopper document triggered by an AutoCAD document change.
/// </summary>
public interface IGrasshopperDocumentChangedEvent
{
    /// <summary>
    /// Gets the AutoCAD document change that triggered this event.
    /// </summary>
    IAutocadDocumentChange Change { get; }

    /// <summary>
    /// Gets the Grasshopper document associated with this change event.
    /// </summary>
    GH_Document Definition { get; }

    /// <summary>
    /// Gets the list of Grasshopper objects that are marked as expired due to the change.
    /// </summary>
    List<IGH_ActiveObject> ExpiredObjects { get; }

    /// <summary>
    /// Creates a new solution for the Grasshopper document based on the expired objects.
    /// </summary>
    /// <returns>The updated Grasshopper document after processing the changes.</returns>
    GH_Document NewSolution();
}