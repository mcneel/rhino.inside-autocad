using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperChangeResponder"/>
public class GrasshopperChangeResponder : IGrasshopperChangeResponder
{
    private readonly IFlushQueueHandler _flushQueue;

    /// <summary>
    /// Constructs a new <see cref="IGrasshopperChangeResponder"/>.
    /// </summary>
    public GrasshopperChangeResponder()
    {
        _flushQueue = new FlushQueueHandler();
    }

    /// <summary>
    /// Returns true if the Grasshopper active object is awaiting solution and will be computed
    /// in this current solution cycle.
    /// </summary>
    private bool AwaitingSolution(bool activeDefinition, IGH_ActiveObject ghActiveObject)
    {
        return activeDefinition && ghActiveObject.Phase == GH_SolutionPhase.Blank;
    }

    /// <summary>
    /// Returns true if the Grasshopper active object is currently being computed which has
    /// triggered the change.
    /// </summary>
    private bool IsActiveObject(IGH_ActiveObject ghActiveObject)
    {
        return ghActiveObject.Phase == GH_SolutionPhase.Computing;
    }

    /// <summary>
    /// Enqueues the specified Grasshopper document change for processing.
    /// </summary>
    private void Enqueue(IGrasshopperDocumentChangedEvent change)
    {
        if (GH_Document.EnableSolutions == false)
            return;

        if (change.Definition.SolutionState != GH_ProcessStep.Process)
        {
            _flushQueue.Add(change);

            _flushQueue.Execute();
        }
    }

    /// <summary>
    /// Updates a specific Grasshopper document according to the specified AutoCAD document change.
    /// </summary>
    private void UpdateDocument(IAutocadDocumentChange documentChange, GH_Document definition)
    {
        var activeDefinition = definition.SolutionState == GH_ProcessStep.Process;

        if (activeDefinition)
            return;

        var grasshopperChange = new GrasshopperDocumentChangedEvent(documentChange, definition);

        foreach (var ghActiveObject in definition.Objects.OfType<IGH_ActiveObject>())
        {
            if (ghActiveObject.Locked
                || this.AwaitingSolution(activeDefinition, ghActiveObject)
                || this.IsActiveObject(ghActiveObject))
                continue;

            try
            {
                switch (ghActiveObject)
                {
                    case IReferenceParam persistentParam:
                        {
                            if (persistentParam.NeedsToBeExpired(documentChange))
                            {
                                grasshopperChange.ExpiredObjects.Add(persistentParam);
                            }

                            break;
                        }
                    case IReferenceComponent persistentComponent:
                        {
                            if (persistentComponent.NeedsToBeExpired(documentChange))
                            {
                                grasshopperChange.ExpiredObjects.Add(persistentComponent);
                            }
                            break;
                        }
                }
            }
            catch { }
        }

        if (grasshopperChange.ExpiredObjects.Any())
            this.Enqueue(grasshopperChange);
    }

    /// <inheritdoc />
    public void Respond(IAutocadDocumentChange documentChange)
    {
        if (documentChange.HasChanges == false) return;

        foreach (GH_Document definition in Instances.DocumentServer)
        {
            this.UpdateDocument(documentChange, definition);
        }
    }
}
