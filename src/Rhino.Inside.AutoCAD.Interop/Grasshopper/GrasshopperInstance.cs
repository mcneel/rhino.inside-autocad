using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Represents an implementation of <see cref="IGrasshopperInstance"/> that manages the lifecycle
/// and interactions with Grasshopper within the Rhino.Inside.AutoCAD environment.
/// </summary>
public class GrasshopperInstance : IGrasshopperInstance
{
    private readonly IApplicationDirectories _applicationDirectories;

    /// <inheritdoc />
    public event EventHandler<IGrasshopperObjectModifiedEventArgs>? OnPreviewExpired;

    /// <inheritdoc />
    public event EventHandler<IGrasshopperObjectModifiedEventArgs>? OnObjectRemoved;

    /// <inheritdoc />
    public GH_Document? ActiveDoc { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrasshopperInstance"/> class.
    /// </summary>
    /// <param name="applicationDirectories">The application directories used to locate resources.</param>
    public GrasshopperInstance(IApplicationDirectories applicationDirectories)
    {
        _applicationDirectories = applicationDirectories;
    }

    /// <summary>
    /// Uses reflection to load the Grasshopper library into the Grasshopper component server.
    /// </summary>
    /// <exception cref="TargetException">Thrown if the LoadGHA method is not found.</exception>
    /// <exception cref="Exception">Thrown if an error occurs while invoking the LoadGHA method.</exception>
    private void LoadGrasshopperLibrary()
    {
        var assembliesFolder = _applicationDirectories.Assemblies;
        var grasshopperLibraryPath = System.IO.Path.Combine(assembliesFolder, "Rhino.Inside.AutoCAD.GrasshopperLibrary.dll");

        var loadGhaMethod = typeof(GH_ComponentServer).GetMethod(
            "LoadGHA", BindingFlags.NonPublic | BindingFlags.Instance);

        if (loadGhaMethod == null)
        {
            throw new TargetException("LoadGHA method not found");
        }

        try
        {
            loadGhaMethod.Invoke(Instances.ComponentServer,
                [new GH_ExternalFile(grasshopperLibraryPath), false]
            );
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }

    /// <summary>
    /// Loads and initializes the Grasshopper environment.
    /// </summary>
    /// <param name="validationLogger">The logger to record validation messages.</param>
    /// <returns>The active Grasshopper document.</returns>
    /// <exception cref="Exception">Thrown if Grasshopper fails to initialize.</exception>
    private GH_Document LoadGrasshopper(IValidationLogger validationLogger)
    {
        try
        {
            this.LoadGrasshopperLibrary();

            var activeCanvas = Grasshopper.Instances.ActiveCanvas;
            var document = activeCanvas.Document;

            activeCanvas.DocumentChanged += this.OnDocumentChanged;

            this.AddDocumentSubscriptions(document);

            return document;
        }
        catch
        {
            validationLogger.AddMessage("Failed to initialize Grasshopper");
            throw;
        }
    }

    /// <summary>
    /// Removes subscriptions to events in the current Grasshopper document.
    /// </summary>
    private void RemoveDocumentSubscriptions()
    {
        if (this.ActiveDoc != null)
        {
            this.ActiveDoc.ObjectsAdded -= this.OnObjectsAdded;
            this.ActiveDoc.ObjectsDeleted -= this.OnObjectsDeleted;

            foreach (var obj in this.ActiveDoc.Objects)
            {
                this.UnhookPreviewExpired(obj);
            }
        }
    }

    /// <summary>
    /// Handles the event when objects are added to the Grasshopper document.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnObjectsAdded(object sender, GH_DocObjectEventArgs e)
    {
        foreach (var ghDocumentObject in e.Objects)
        {
            this.HookPreviewExpired(ghDocumentObject);
        }
    }

    /// <summary>
    /// Handles the event when objects are deleted from the Grasshopper document.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnObjectsDeleted(object sender, GH_DocObjectEventArgs e)
    {
        foreach (var ghDocumentObject in e.Objects)
        {
            this.UnhookPreviewExpired(ghDocumentObject);

            this.OnObjectRemoved?.Invoke(this, new GrasshopperObjectModifiedEventArgs(ghDocumentObject));
        }
    }

    /// <summary>
    /// Subscribes to the PreviewExpired event of a Grasshopper document object.
    /// </summary>
    /// <param name="documentObject">The document object to subscribe to.</param>
    private void HookPreviewExpired(IGH_DocumentObject documentObject)
    {
        documentObject.PreviewExpired += this.OnGrasshopperPreviewExpired;
    }

    /// <summary>
    /// Unsubscribes from the PreviewExpired event of a Grasshopper document object.
    /// </summary>
    /// <param name="documentObject">The document object to unsubscribe from.</param>
    private void UnhookPreviewExpired(IGH_DocumentObject documentObject)
    {
        documentObject.PreviewExpired -= this.OnGrasshopperPreviewExpired;
    }

    /// <summary>
    /// Handles the PreviewExpired event for a Grasshopper document object.
    /// </summary>
    /// <param name="sender">The document object that triggered the event.</param>
    /// <param name="e">The event data.</param>
    private void OnGrasshopperPreviewExpired(IGH_DocumentObject sender, GH_PreviewExpiredEventArgs e)
    {
        this.OnPreviewExpired?.Invoke(this, new GrasshopperObjectModifiedEventArgs(sender));
    }

    /// <summary>
    /// Subscribes to events in the specified Grasshopper document.
    /// </summary>
    /// <param name="document">The Grasshopper document to subscribe to.</param>
    private void AddDocumentSubscriptions(GH_Document document)
    {
        document.ObjectsAdded += this.OnObjectsAdded;
        document.ObjectsDeleted += this.OnObjectsDeleted;

        foreach (var ghDocumentObject in document.Objects)
        {
            this.HookPreviewExpired(ghDocumentObject);
        }
    }

    /// <summary>
    /// Handles the event when the active Grasshopper document changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnDocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
    {
        this.RemoveDocumentSubscriptions();
        this.ActiveDoc = e.NewDocument;
        this.AddDocumentSubscriptions(this.ActiveDoc);
    }

    /// <summary>
    /// Validates that the Grasshopper library is loaded into the Grasshopper component server.
    /// </summary>
    /// <param name="validationLogger">The logger to record validation messages.</param>
    public void ValidateGrasshopperLibrary(IValidationLogger validationLogger)
    {
        this.ActiveDoc ??= this.LoadGrasshopper(validationLogger);
    }

    /// <summary>
    /// Recomputes the Grasshopper solution in the active Grasshopper document.
    /// </summary>
    public void RecomputeSolution()
    {
        if (this.ActiveDoc is null) return;

        this.ActiveDoc.NewSolution(true);
    }

    /// <summary>
    /// Disables the Grasshopper solver, preventing solutions from being recomputed.
    /// </summary>
    public void DisableSolver()
    {
        Grasshopper.Kernel.GH_Document.EnableSolutions = false;
    }

    /// <summary>
    /// Enables the Grasshopper solver, allowing solutions to be recomputed.
    /// </summary>
    public void EnableSolver()
    {
        Grasshopper.Kernel.GH_Document.EnableSolutions = true;
    }

    /// <summary>
    /// Shuts down the Grasshopper instance, releasing resources and removing subscriptions.
    /// </summary>
    public void Shutdown()
    {
        this.RemoveDocumentSubscriptions();
        this.ActiveDoc?.Dispose();
    }
}