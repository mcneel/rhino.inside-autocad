using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperObjectPreviewServer"/>
public class GrasshopperObjectPreviewServer : IGrasshopperObjectPreviewServer
{
    private readonly IRhinoConvertibleFactory _rhinoConvertibleFactory;
    private readonly IPreviewServer _shadedPreviewServer;
    private readonly IPreviewServer _wireframePreviewServer;
    private readonly IGrasshopperPreviewButtonManager _buttonManager;

    /// <inheritdoc/>
    public IGeometryPreviewSettings Settings { get; }

    /// <inheritdoc/>
    public GrasshopperPreviewMode PreviewMode { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="IGrasshopperObjectPreviewServer"/>
    /// </summary>
    public GrasshopperObjectPreviewServer(IGeometryPreviewSettings geometryPreviewSettings,
        IPreviewGeometryConverter previewGeometryConverter, IRhinoConvertibleFactory rhinoConvertibleFactory)
    {
        _rhinoConvertibleFactory = rhinoConvertibleFactory;
        _shadedPreviewServer = new PreviewServer(geometryPreviewSettings, previewGeometryConverter);
        _wireframePreviewServer = new PreviewServer(geometryPreviewSettings, previewGeometryConverter);

        _buttonManager = new GrasshopperPreviewButtonManager();

        this.PreviewMode = GrasshopperPreviewMode.Shaded;
        this.Settings = geometryPreviewSettings;
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

        _shadedPreviewServer.AddObject(rhinoObjectId, shadedSet);

        _wireframePreviewServer.AddObject(rhinoObjectId, wireFrameSet);

    }

    /// <inheritdoc />
    public void RemoveObject(Guid rhinoObjectId)
    {
        _shadedPreviewServer.RemoveObject(rhinoObjectId);
        _wireframePreviewServer.RemoveObject(rhinoObjectId);
    }
}