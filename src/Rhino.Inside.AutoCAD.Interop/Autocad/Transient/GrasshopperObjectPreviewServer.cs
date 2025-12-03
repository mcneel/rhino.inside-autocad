using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperObjectPreviewServer"/>
public class GrasshopperObjectPreviewServer : IGrasshopperObjectPreviewServer
{
    private readonly IPreviewServer _previewServer;
    private readonly IGrasshopperPreviewButtonManager _buttonManager;

    /// <inheritdoc/>
    public GrasshopperPreviewMode PreviewMode { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="IGrasshopperObjectPreviewServer"/>
    /// </summary>
    public GrasshopperObjectPreviewServer()
    {
        _previewServer = new PreviewServer();

        _buttonManager = new GrasshopperPreviewButtonManager();

        this.PreviewMode = GrasshopperPreviewMode.Shaded;

    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void UpdateTransientElements()
    {
        if (this.PreviewMode == GrasshopperPreviewMode.Off)
        {
            foreach (var entities in _previewServer.ObjectRegister)
            {
                _previewServer.RemoveTransientEntities(entities);
            }
        }
        else
        {
            foreach (var entities in _previewServer.ObjectRegister)
            {
                _previewServer.AddTransientEntities(entities);
            }
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
    public void AddObject(Guid rhinoObjectId, List<IEntity> entities)
    {
        _previewServer.AddObject(rhinoObjectId, entities);
    }

    /// <inheritdoc />
    public void RemoveObject(Guid rhinoObjectId)
    {
        _previewServer.RemoveObject(rhinoObjectId);
    }
}