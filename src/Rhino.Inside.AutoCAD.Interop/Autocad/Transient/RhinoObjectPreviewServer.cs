using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoObjectPreviewServer"/>
public class RhinoObjectPreviewServer : IRhinoObjectPreviewServer
{

    private readonly IPreviewServer _previewServer;

    /// <inheritdoc/>
    public bool Visible { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="RhinoObjectPreviewServer"/>
    /// </summary>
    public RhinoObjectPreviewServer()
    {
        _previewServer = new PreviewServer();
        this.Visible = true;
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void UpdateTransientElements()
    {
        if (this.Visible)
        {
            foreach (var entities in _previewServer.ObjectRegister)
            {
                _previewServer.AddTransientEntities(entities);
            }
        }
        else
        {
            foreach (var entities in _previewServer.ObjectRegister)
            {
                _previewServer.RemoveTransientEntities(entities);
            }
        }
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

    /// <inheritdoc />
    public void ToggleVisibility()
    {
        this.Visible = !this.Visible;

        this.UpdateTransientElements();
    }
}