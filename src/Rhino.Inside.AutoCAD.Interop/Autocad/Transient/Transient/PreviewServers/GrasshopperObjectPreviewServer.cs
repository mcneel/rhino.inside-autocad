using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperObjectPreviewServer"/>
public class GrasshopperObjectPreviewServer : IGrasshopperObjectPreviewServer
{

    private readonly IPreviewServer _shadedPreviewServer;
    private readonly IPreviewServer _wireframePreviewServer;
    private readonly IGrasshopperPreviewButtonManager _buttonManager;

    /// <inheritdoc/>
    public IGeometryPreviewSettings UnSelectedSettings { get; }

    /// <inheritdoc/>
    public IGeometryPreviewSettings SelectedSettings { get; }

    /// <inheritdoc/>
    public GrasshopperPreviewMode PreviewMode { get; private set; }

    /// <inheritdoc/>
    /// <remarks>
    /// Applied to the shaded and wireframe servers separately, so each draws up to this many.
    /// </remarks>
    public int MaxEntityCount
    {
        get => _shadedPreviewServer.MaxEntityCount;
        set
        {
            _shadedPreviewServer.MaxEntityCount = value;
            _wireframePreviewServer.MaxEntityCount = value;
        }
    }

    /// <summary>
    /// Constructs a new <see cref="IGrasshopperObjectPreviewServer"/>
    /// </summary>
    public GrasshopperObjectPreviewServer(IGeometryPreviewSettings geometryPreviewSettings,
        IGeometryPreviewSettings selectedPreviewSettings, IPreviewGeometryConverter previewGeometryConverter,
        int maxEntityCount)
    {
        _shadedPreviewServer = new PreviewServer(geometryPreviewSettings, selectedPreviewSettings,
            previewGeometryConverter, maxEntityCount);
        _wireframePreviewServer = new PreviewServer(geometryPreviewSettings, selectedPreviewSettings,
            previewGeometryConverter, maxEntityCount);

        _buttonManager = new GrasshopperPreviewButtonManager();

        this.PreviewMode = GrasshopperPreviewMode.Shaded;
        this.UnSelectedSettings = geometryPreviewSettings;
        this.SelectedSettings = selectedPreviewSettings;
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void UpdateTransientElements()
    {
        switch (this.PreviewMode)
        {
            case GrasshopperPreviewMode.Off:
                _wireframePreviewServer.ClearServer();
                _shadedPreviewServer.ClearServer();
                break;
            case GrasshopperPreviewMode.Wireframe:
                _wireframePreviewServer.PopulateServer();
                _shadedPreviewServer.ClearServer();
                break;
            case GrasshopperPreviewMode.Shaded:
                _wireframePreviewServer.PopulateServer();
                _shadedPreviewServer.PopulateServer();
                break;
        }
    }

    /// <inheritdoc />
    public void SetMode(GrasshopperPreviewMode previewMode)
    {
        this.PreviewMode = previewMode;

        _buttonManager.SetPreviewMode(previewMode);

        this.UpdateTransientElements();
    }

    /// <inheritdoc />
    public void AddObject(Guid rhinoObjectId, IGrasshopperPreviewData grasshopperPreviewData)
    {
        var shadedSet = grasshopperPreviewData.GetShadedObjects();

        var wireFrameSet = grasshopperPreviewData.GetWireframeObjects();

        _shadedPreviewServer.AddObject(rhinoObjectId, shadedSet, grasshopperPreviewData.IsSelected);

        _wireframePreviewServer.AddObject(rhinoObjectId, wireFrameSet, grasshopperPreviewData.IsSelected);

    }

    /// <inheritdoc />
    public void RemoveObject(Guid rhinoObjectId)
    {
        _shadedPreviewServer.RemoveObject(rhinoObjectId);
        _wireframePreviewServer.RemoveObject(rhinoObjectId);
    }

    /// <inheritdoc />
    public void RefreshAppearance()
    {
        _shadedPreviewServer.RefreshAppearance();
        _wireframePreviewServer.RefreshAppearance();

        // Refreshing adds the transients back, which would show previews the current preview
        // mode hides, so the mode is reapplied.
        this.UpdateTransientElements();
    }

    /// <summary>
    /// Clears all preview objects from both shaded and wireframe servers and disposes entities.
    /// Used during application shutdown to ensure clean disposal.
    /// </summary>
    public void ClearAll()
    {
        _shadedPreviewServer.ClearAndDisposeAll();
        _wireframePreviewServer.ClearAndDisposeAll();
    }
}