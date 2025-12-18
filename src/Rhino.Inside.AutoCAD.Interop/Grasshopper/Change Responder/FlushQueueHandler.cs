using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IFlushQueueHandler"/>
public class FlushQueueHandler : IFlushQueueHandler
{
    private readonly Queue<IGrasshopperDocumentChangedEvent> _changeQueue = new Queue<IGrasshopperDocumentChangedEvent>();

    /// <inheritdoc />
    public void Add(IGrasshopperDocumentChangedEvent change)
    {
        _changeQueue.Enqueue(change);
    }

    /// <inheritdoc />
    public void Execute()
    {
        var solutions = new List<GH_Document>();
        while (_changeQueue.Count > 0)
        {
            if (_changeQueue.Dequeue().NewSolution() is GH_Document solution)
            {
                if (solutions.Contains(solution) == false)
                    solutions.Add(solution);
            }
        }

        foreach (var solution in solutions)
            solution.NewSolution(false);
    }
}