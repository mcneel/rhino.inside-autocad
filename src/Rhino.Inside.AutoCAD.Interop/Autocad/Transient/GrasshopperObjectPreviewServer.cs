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
    public GrasshopperPreviewMode PreviewMode { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="IGrasshopperObjectPreviewServer"/>
    /// </summary>
    public GrasshopperObjectPreviewServer()
    {
        _shadedPreviewServer = new PreviewServer();
        _wireframePreviewServer = new PreviewServer();

        _buttonManager = new GrasshopperPreviewButtonManager();

        this.PreviewMode = GrasshopperPreviewMode.Shaded;
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void ClearServer(IPreviewServer server)
    {
        foreach (var entities in server.ObjectRegister)
        {
            server.RemoveTransientEntities(entities);
        }
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void PopulateServer(IPreviewServer server)
    {
        foreach (var entities in server.ObjectRegister)
        {
            server.AddTransientEntities(entities);
        }
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void UpdateTransientElements()
    {
        switch (this.PreviewMode)
        {
            case GrasshopperPreviewMode.Off:
                this.ClearServer(_wireframePreviewServer);
                this.ClearServer(_shadedPreviewServer);
                break;
            case GrasshopperPreviewMode.Wireframe:
                this.PopulateServer(_wireframePreviewServer);
                this.ClearServer(_shadedPreviewServer);
                break;
            case GrasshopperPreviewMode.Shaded:
                this.PopulateServer(_wireframePreviewServer);
                this.PopulateServer(_shadedPreviewServer);
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
    public void AddObject(Guid rhinoObjectId, List<IEntity> wireframeEntities, List<IEntity> shadedEntities)
    {
        _shadedPreviewServer.AddObject(rhinoObjectId, shadedEntities);

        _wireframePreviewServer.AddObject(rhinoObjectId, wireframeEntities);

    }

    /// <inheritdoc />
    public void RemoveObject(Guid rhinoObjectId)
    {
        _shadedPreviewServer.RemoveObject(rhinoObjectId);
        _wireframePreviewServer.RemoveObject(rhinoObjectId);

    }
}