using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoObjectPreviewServer"/>
public class RhinoObjectPreviewServer : IRhinoObjectPreviewServer
{
    private readonly IPreviewServer _previewServer;

    /// <inheritdoc />
    public IGeometryPreviewSettings Settings { get; }

    /// <inheritdoc/>
    public bool Visible { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="RhinoObjectPreviewServer"/>
    /// </summary>
    public RhinoObjectPreviewServer(IGeometryPreviewSettings geometryPreviewSettings,
        IPreviewGeometryConverter previewGeometryConverter)
    {
        _previewServer = new PreviewServer(geometryPreviewSettings, previewGeometryConverter);

        this.Visible = true;

        this.Settings = geometryPreviewSettings;
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    private void UpdateTransientElements()
    {
        if (this.Visible)
        {
            _previewServer.PopulateServer();
        }
        else
        {
            _previewServer.ClearServer();
        }
    }

    /// <inheritdoc />
    public void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet)
    {
        _previewServer.AddObject(rhinoObjectId, rhinoConvertibleSet);

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