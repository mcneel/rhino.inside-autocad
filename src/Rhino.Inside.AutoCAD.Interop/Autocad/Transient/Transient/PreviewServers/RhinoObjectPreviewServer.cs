using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoObjectPreviewServer"/>
public class RhinoObjectPreviewServer : IRhinoObjectPreviewServer
{
    private readonly IPreviewServer _previewServer;

    /// <inheritdoc />
    public IGeometryPreviewSettings UnSelectedSettings { get; }

    /// <inheritdoc />
    public IGeometryPreviewSettings SelectedSettings { get; }

    /// <inheritdoc/>
    public bool Visible { get; private set; }

    /// <inheritdoc/>
    public int MaxEntityCount
    {
        get => _previewServer.MaxEntityCount;
        set => _previewServer.MaxEntityCount = value;
    }

    /// <summary>
    /// Constructs a new <see cref="RhinoObjectPreviewServer"/>
    /// </summary>
    public RhinoObjectPreviewServer(IGeometryPreviewSettings geometryPreviewSettings,
        IGeometryPreviewSettings selectedPreviewSettings,
        IPreviewGeometryConverter previewGeometryConverter, int maxEntityCount)
    {
        _previewServer = new PreviewServer(geometryPreviewSettings, selectedPreviewSettings,
            previewGeometryConverter, maxEntityCount);

        this.Visible = true;

        this.UnSelectedSettings = geometryPreviewSettings;

        this.SelectedSettings = selectedPreviewSettings;
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
    public void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet, bool isSelected)
    {
        _previewServer.AddObject(rhinoObjectId, rhinoConvertibleSet, isSelected);

    }

    /// <inheritdoc />
    public void RemoveObject(Guid rhinoObjectId)
    {
        _previewServer.RemoveObject(rhinoObjectId);
    }

    /// <inheritdoc />
    public void DeselectAll()
    {
        _previewServer.DeselectAll();
    }

    /// <inheritdoc />
    public void RefreshAppearance()
    {
        _previewServer.RefreshAppearance();

        // Refreshing adds the transients back, which would show previews the user has
        // toggled off, so the visibility state is reapplied.
        this.UpdateTransientElements();
    }

    /// <inheritdoc />
    public void ToggleVisibility()
    {
        this.Visible = !this.Visible;

        this.UpdateTransientElements();
    }

    /// <summary>
    /// Clears all preview objects from the server and disposes entities.
    /// Used during application shutdown to ensure clean disposal.
    /// </summary>
    public void ClearAll()
    {
        _previewServer.ClearAndDisposeAll();
    }
}