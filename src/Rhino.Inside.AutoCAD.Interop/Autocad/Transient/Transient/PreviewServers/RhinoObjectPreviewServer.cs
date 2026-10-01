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
    public bool IsSuppressed { get; private set; }

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
        IPreviewDrawableBuilder previewDrawableBuilder, int maxEntityCount)
    {
        _previewServer = new PreviewServer(geometryPreviewSettings, selectedPreviewSettings,
            previewDrawableBuilder, maxEntityCount);

        this.Visible = true;

        this.UnSelectedSettings = geometryPreviewSettings;

        this.SelectedSettings = selectedPreviewSettings;
    }

    /// <summary>
    /// Updates the transient elements visibility based on the current state.
    /// </summary>
    /// <remarks>
    /// Drawn only while the user has the preview on and it is not suppressed.
    /// </remarks>
    private void UpdateTransientElements()
    {
        if (this.Visible && this.IsSuppressed == false)
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
    public bool SetSelected(Guid rhinoObjectId, bool selected)
    {
        return _previewServer.SetSelected(rhinoObjectId, selected);
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
    }

    /// <inheritdoc />
    public void ToggleVisibility()
    {
        this.Visible = !this.Visible;

        this.UpdateTransientElements();
    }

    /// <inheritdoc />
    public void SetSuppressed(bool suppressed)
    {
        this.IsSuppressed = suppressed;

        this.UpdateTransientElements();
    }

    /// <inheritdoc />
    /// <remarks>
    /// Used when the Rhino document is closed and during application shutdown.
    /// </remarks>
    public void ClearAll()
    {
        _previewServer.ClearAndDisposeAll();
    }
}