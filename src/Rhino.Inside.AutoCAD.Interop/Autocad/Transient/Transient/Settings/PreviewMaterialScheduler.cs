using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.State;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IPreviewMaterialScheduler"/>
public class PreviewMaterialScheduler : IPreviewMaterialScheduler
{
    private readonly IAutocadGuard _autocadGuard = new AutocadGuard();

    private readonly HashSet<IGeometryPreviewSettings> _pendingSettings = [];

    private readonly Action _onMaterialsCreated;

    /// <summary>
    /// The document the pending settings were requested against.
    /// </summary>
    /// <remarks>
    /// A material belongs to one database, so a request made against a document which is no
    /// longer the one being requested supersedes the earlier one rather than joining it.
    /// </remarks>
    private IAutocadDocument? _pendingDocument;

    private bool _subscribed;

    /// <summary>
    /// Constructs a new <see cref="PreviewMaterialScheduler"/>.
    /// </summary>
    /// <param name="onMaterialsCreated">
    /// Invoked once, after materials have been created, so the caller can restyle previews
    /// which were drawn while the material was still missing. Not invoked when nothing
    /// needed creating.
    /// </param>
    public PreviewMaterialScheduler(Action onMaterialsCreated)
    {
        _onMaterialsCreated = onMaterialsCreated;
    }

    /// <inheritdoc/>
    public void EnsureCreated(IAutocadDocument document, params IGeometryPreviewSettings[] settings)
    {
        if (ApplicationState.IsShuttingDown) return;

        if (ReferenceEquals(_pendingDocument, document) == false)
        {
            _pendingSettings.Clear();

            _pendingDocument = document;
        }

        foreach (var previewSettings in settings)
        {
            if (previewSettings.HasMaterialFor(document))
                continue;

            _ = _pendingSettings.Add(previewSettings);
        }

        if (_pendingSettings.Count == 0)
            return;

        this.Subscribe();
    }

    /// <summary>
    /// Attaches to the idle loop, if it is not already attached.
    /// </summary>
    private void Subscribe()
    {
        if (_subscribed)
            return;

        Application.Idle += this.OnIdle;

        _subscribed = true;
    }

    /// <summary>
    /// Detaches from the idle loop, if it is attached.
    /// </summary>
    private void Unsubscribe()
    {
        if (_subscribed == false)
            return;

        Application.Idle -= this.OnIdle;

        _subscribed = false;
    }

    /// <summary>
    /// Creates the pending materials once AutoCAD is idle.
    /// </summary>
    /// <remarks>
    /// Guarded because AutoCAD raises this from native code: an exception escaping here has
    /// no managed frame above it to be caught by and would terminate the host. Unsubscribes
    /// before doing any work, so a failure cannot put the creation attempt into a loop that
    /// runs on every idle tick.
    /// </remarks>
    private void OnIdle(object? sender, EventArgs e)
    {
        this.Unsubscribe();

        _autocadGuard.Run(this.CreatePendingMaterials, nameof(this.OnIdle));
    }

    /// <summary>
    /// Creates every pending material and restyles the previews already drawn.
    /// </summary>
    private void CreatePendingMaterials()
    {
        if (ApplicationState.IsShuttingDown) return;

        var document = _pendingDocument;

        var settings = _pendingSettings.ToList();

        _pendingSettings.Clear();

        _pendingDocument = null;

        if (document == null || settings.Count == 0)
            return;

        var created = false;

        foreach (var previewSettings in settings)
        {
            // Re-checked rather than trusted from the request: a document switch or an
            // earlier idle pass may have created the material in the meantime.
            if (previewSettings.HasMaterialFor(document))
                continue;

            previewSettings.CreateMaterial(document);

            created = true;
        }

        if (created == false)
            return;

        _onMaterialsCreated.Invoke();
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        this.Unsubscribe();

        _pendingSettings.Clear();

        _pendingDocument = null;
    }
}
