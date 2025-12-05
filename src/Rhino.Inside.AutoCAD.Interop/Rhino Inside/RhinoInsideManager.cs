using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoInsideManager"/>
public class RhinoInsideManager : IRhinoInsideManager
{
    private readonly UnitSystem _defaultUnitSystem = InteropConstants.FallbackUnitSystem;
    private readonly IRhinoObjectConverter _rhinoObjectConvert;
    private readonly IGrasshopperGeometryExtractor _grasshopperGeometryExtractor;
    private readonly IGrasshopperChangeResponder _grasshopperChangeResponder;

    /// <inheritdoc />
    public IRhinoInstance RhinoInstance { get; }

    /// <inheritdoc />
    public IAutoCadInstance AutoCadInstance { get; }

    /// <inheritdoc />
    public IGrasshopperInstance GrasshopperInstance { get; }

    /// <inheritdoc />
    public IUnitSystemManager UnitSystemManager { get; private set; }

    /// <inheritdoc />
    public IRhinoObjectPreviewServer RhinoPreviewServer { get; }

    /// <inheritdoc />
    public IGrasshopperObjectPreviewServer GrasshopperPreviewServer { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInsideManager"/> instance.
    /// </summary>
    public RhinoInsideManager(IRhinoInstance rhinoInstance, IGrasshopperInstance grasshopperInstance,
        IAutoCadInstance autoCadInstance)
    {

        this.RhinoPreviewServer = new RhinoObjectPreviewServer();

        this.GrasshopperPreviewServer = new GrasshopperObjectPreviewServer();

        this.AutoCadInstance = autoCadInstance;
        autoCadInstance.DocumentCreated += this.UpdateUnitSystem;
        autoCadInstance.UnitsChanged += this.UpdateUnitSystem;
        autoCadInstance.DocumentChanged += this.AutocadDocumentChange;

        this.RhinoInstance = rhinoInstance;
        rhinoInstance.DocumentCreated += this.UpdateUnitSystem;
        rhinoInstance.UnitsChanged += this.UpdateUnitSystem;
        rhinoInstance.ObjectModifiedOrAppended += this.RhinoObjectModifiedOrAppended;
        rhinoInstance.ObjectRemoved += this.RhinoObjectRemoved;

        this.GrasshopperInstance = grasshopperInstance;
        grasshopperInstance.OnPreviewExpired += this.UpdateGrasshopperPreview;
        grasshopperInstance.OnObjectRemoved += this.OnGrasshopperObjectRemoved;

        var unitsSystemManager = new UnitSystemManager(_defaultUnitSystem, _defaultUnitSystem);

        GeometryConverter.Initialize(unitsSystemManager);
        _rhinoObjectConvert = new RhinoObjectConverter(GeometryConverter.Instance!);

        this.UnitSystemManager = unitsSystemManager;
        _grasshopperGeometryExtractor = new GrasshopperGeometryExtractor();
        _grasshopperChangeResponder = new GrasshopperChangeResponder();
    }

    private void AutocadDocumentChange(object sender, IAutocadDocumentChangeEventArgs e)
    {
        _grasshopperChangeResponder.Respond(e.Change);
    }

    /// <summary>
    /// Removes the preview of a Grasshopper object from the <see cref="GrasshopperPreviewServer"/>
    /// when it is removed from the Grasshopper document.
    /// </summary>
    private void OnGrasshopperObjectRemoved(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        this.GrasshopperPreviewServer.RemoveObject(e.GrasshopperObject.InstanceGuid);
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Grasshopper object's preview expires.
    /// </summary>
    private void UpdateGrasshopperPreview(object sender, IGrasshopperObjectModifiedEventArgs e)
    {
        var ghDocumentObject = e.GrasshopperObject;

        var instanceGuid = ghDocumentObject.InstanceGuid;

        this.GrasshopperPreviewServer.RemoveObject(e.GrasshopperObject.InstanceGuid);

        var previewGeometryData = _grasshopperGeometryExtractor.ExtractPreviewGeometry(ghDocumentObject);

        var convertedWireframeEntities = previewGeometryData.GetWireframeEntities();

        var convertedShadedEntities = previewGeometryData.GetShadedEntities();

        this.GrasshopperPreviewServer.AddObject(instanceGuid, convertedWireframeEntities, convertedShadedEntities);

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();

    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is removed.
    /// </summary>
    private void RhinoObjectRemoved(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();
    }

    /// <summary>
    /// Updates the AutoCAD transient preview when a Rhino object is modified or appended.
    /// </summary>
    private void RhinoObjectModifiedOrAppended(object sender, IRhinoObjectModifiedEventArgs e)
    {
        var rhinoObject = e.RhinoObject;

        this.RhinoPreviewServer.RemoveObject(rhinoObject.Id);

        if (_rhinoObjectConvert.TryConvert(rhinoObject, out var newEntities))
        {
            this.RhinoPreviewServer.AddObject(rhinoObject.Id, newEntities);
        }

        this.AutoCadInstance.ActiveDocument?.UpdateScreen();
    }

    private void UpdateUnitSystem(object sender, EventArgs e)
    {
        var autoCadUnits = this.AutoCadInstance.ActiveDocument?.UnitSystem ?? _defaultUnitSystem;
        var rhinoUnits = this.RhinoInstance.ActiveDoc?.ModelUnitSystem ?? _defaultUnitSystem;

        if (this.UnitSystemManager.AutoCadUnits != autoCadUnits ||
            this.UnitSystemManager.RhinoUnits != rhinoUnits)
        {
            var unitsSystemManager = new UnitSystemManager(autoCadUnits, rhinoUnits);
            this.UnitSystemManager = unitsSystemManager;

            GeometryConverter.Initialize(unitsSystemManager);

        }
    }
}

/// <summary>
/// A Service that responds to AutoCAD document changes and updates Grasshopper documents accordingly.
/// </summary>
public interface IGrasshopperChangeResponder
{
    /// <summary>
    /// Updates the all the Grasshopper documents according to the specified AutoCAD document change.
    /// </summary>
    void Respond(IAutocadDocumentChange documentChange);
}

/// <inheritdoc cref="IGrasshopperChangeResponder"/>
public class GrasshopperChangeResponder : IGrasshopperChangeResponder
{
    private readonly IFlushQueueHandler _flushQueue;

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
            obj.ExpireSolution(false);

        return this.Definition;
    }
}

/// <summary>
/// Base interface for all Parameter types in Rhino.Inside.Autocad that reference Autocad
/// elements.
/// </summary>
public interface IReferenceParam : IGH_Param
{
    /// <summary>
    /// Each implementing Parameter must define whether it needs to be expired based on
    /// the given <see cref="IAutocadDocumentChange"/> in the <see cref="IAutocadDocument"/>.
    /// If it does, the Parameter will be expired to trigger computation.
    /// </summary>
    bool NeedsToBeExpired(IAutocadDocumentChange change);
}

/// <summary>
/// Base interface for all Component types in Rhino.Inside.Autocad that reference Autocad
/// elements.
/// </summary>
public interface IReferenceComponent : IGH_Component
{
    /// <summary>
    /// Each implementing Component must define whether it needs to be expired based on
    /// the given <see cref="IAutocadDocumentChange"/> in the <see cref="IAutocadDocument"/>.
    /// If it does, the Component will be expired to trigger computation.
    /// </summary>
    bool NeedsToBeExpired(IAutocadDocumentChange change);
}

/// <summary>
/// Handles the queuing and execution of Grasshopper document changes in response to
/// AutoCAD document changes.
/// </summary>
internal interface IFlushQueueHandler
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

/// <inheritdoc cref="IFlushQueueHandler"/>
class FlushQueueHandler : IFlushQueueHandler
{
    private readonly Queue<IGrasshopperDocumentChangedEvent> _changeQueue = new Queue<IGrasshopperDocumentChangedEvent>();

    /// <inheritdoc />
    public void Add(IGrasshopperDocumentChangedEvent change)
    {
        _changeQueue.Enqueue(change);
    }

    /// <inheritdoc />
    public async Task Execute()
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