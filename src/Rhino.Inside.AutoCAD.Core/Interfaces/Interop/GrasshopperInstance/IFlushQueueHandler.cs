namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Handles the queuing and execution of Grasshopper document changes in response to
/// AutoCAD document changes.
/// </summary>
public interface IFlushQueueHandler
{
    /// <summary>
    /// Adds a Grasshopper document change event to the processing queue.
    /// </summary>
    /// <param name="change">The Grasshopper document change event to enqueue.</param>
    void Add(IGrasshopperDocumentChangedEvent change);

    /// <summary>
    /// Executes all queued Grasshopper document changes, processing them sequentially.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Execute();
}
