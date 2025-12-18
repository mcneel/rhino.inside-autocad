using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperDocumentChangedEvent"/>
public class GrasshopperDocumentChangedEvent : IGrasshopperDocumentChangedEvent
{
    /// <inheritdoc/>
    public IAutocadDocumentChange Change { get; }

    /// <inheritdoc/>
    public GH_Document Definition { get; }

    /// <inheritdoc/>
    public List<IGH_ActiveObject> ExpiredObjects { get; }

    /// <summary>
    /// Constructs a new <see cref="IGrasshopperDocumentChangedEvent"/>
    /// </summary>
    public GrasshopperDocumentChangedEvent(IAutocadDocumentChange change, GH_Document definition)
    {
        this.Change = change;
        this.Definition = definition;
        this.ExpiredObjects = new List<IGH_ActiveObject>();
    }

    /// <inheritdoc/>
    public GH_Document NewSolution()
    {
        foreach (var obj in this.ExpiredObjects)
        {
            if (obj is IGH_Param ghParamsActiveObject)
            {
                foreach (var ghParam in ghParamsActiveObject.VolatileData.AllData(true))
                {
                    if (ghParam is IGH_AutocadReferenceDatabaseObject referenceObject)
                    {
                        referenceObject.GetUpdatedObject();
                    }
                }
            }

            obj.ExpireSolution(true);
        }

        return this.Definition;
    }
}