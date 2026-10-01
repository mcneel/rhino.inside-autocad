namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Rhino objects in AutoCAD using transients, one per previewed
/// object.
/// </summary>
public interface IRhinoObjectPreviewServer
{
    /// <summary>
    /// The settings used to configure the geometry preview in the non-selected state.
    /// </summary>
    IGeometryPreviewSettings UnSelectedSettings { get; }

    /// <summary>
    /// The settings used to configure the geometry preview in the selected state.
    /// </summary>
    IGeometryPreviewSettings SelectedSettings { get; }

    /// <summary>
    /// The visibility the user has chosen for the preview.
    /// </summary>
    /// <remarks>
    /// The preview is only drawn while this is true and <see cref="IsSuppressed"/> is false,
    /// so this keeps reporting the user's choice while the preview is suppressed.
    /// </remarks>
    bool Visible { get; }

    /// <summary>
    /// True while the preview is hidden regardless of <see cref="Visible"/>, for example
    /// while the Rhino window is minimised.
    /// </summary>
    bool IsSuppressed { get; }

    /// <summary>
    /// The most preview items the underlying preview server draws before the oldest previews are
    /// dropped. Lowering it drops the oldest previews straight away.
    /// </summary>
    /// <seealso cref="IUserSettings.MaxPreviewEntityCount"/>
    int MaxEntityCount { get; set; }

    /// <summary>
    /// Toggles the visibility of all transients managed by the <see cref
    /// ="IRhinoObjectPreviewServer"/> which are registered in the <see cref="IObjectRegister"/>.
    /// This will erase the transients if they are currently visible, or redraw them if they
    /// are hidden based on the contents of the <see cref="IObjectRegister"/>.
    /// </summary>
    void ToggleVisibility();

    /// <summary>
    /// Sets <see cref="IsSuppressed"/>, hiding the preview while it is true and showing it
    /// again afterwards if <see cref="Visible"/> is true. <see cref="Visible"/> is left as it
    /// is.
    /// </summary>
    void SetSuppressed(bool suppressed);

    /// <summary>
    /// Removes every preview, disposing its drawable and emptying the
    /// <see cref="IObjectRegister"/>. <see cref="Visible"/> and <see cref="IsSuppressed"/>
    /// are left as they are, so previews added afterwards are drawn as before.
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Adds the provided <paramref name="rhinoConvertibleSet"/> into this <see cref=
    /// "IRhinoObjectPreviewServer"/>.
    /// </summary>
    void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet,
        bool isSelected);

    /// <summary>
    /// Removes the provided <paramref name="rhinoObjectId"/> from this <see cref=
    /// "IRhinoObjectPreviewServer"/>.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);

    /// <summary>
    /// Sets the selection state of the preview registered under <paramref name="rhinoObjectId"/>
    /// and redraws it, without rebuilding its geometry.
    /// </summary>
    /// <returns>
    /// True when a preview is registered under <paramref name="rhinoObjectId"/>, otherwise
    /// false, in which case the caller has to add the object to preview it.
    /// </returns>
    bool SetSelected(Guid rhinoObjectId, bool selected);

    /// <summary>
    /// Deselects all the previews which are in the <see cref="IObjectRegister"/>, redrawing
    /// them with the <see cref="UnSelectedSettings"/>.
    /// </summary>
    void DeselectAll();

    /// <summary>
    /// Redraws every preview with the current <see cref="UnSelectedSettings"/> and
    /// <see cref="SelectedSettings"/>, so a change to those settings is seen immediately.
    /// Nothing is rebuilt, each preview keeps its selection state and the current visibility
    /// state is preserved.
    /// </summary>
    void RefreshAppearance();
}