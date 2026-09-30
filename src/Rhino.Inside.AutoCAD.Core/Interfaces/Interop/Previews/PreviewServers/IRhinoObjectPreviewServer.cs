namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Rhino objects in AutoCAD using transient entities.
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
    /// The current visibility state of the preview.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    /// The most entities each underlying preview server draws before the oldest previews are
    /// dropped. Lowering it drops the oldest previews straight away.
    /// </summary>
    /// <seealso cref="IUserSettings.MaxPreviewEntityCount"/>
    int MaxEntityCount { get; set; }

    /// <summary>
    /// Toggles the visibility of all transient entities managed by the <see cref
    /// ="IRhinoObjectPreviewServer"/> which are registered in the <see cref="IObjectRegister"/>.
    /// This will clear the transient entities if they are currently visible, or redraw
    /// them if they are hidden based on the contents of the <see cref="IObjectRegister"/>.
    /// </summary>
    void ToggleVisibility();

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
    /// Deselects all the transient entities which are in the <see cref="IObjectRegister"/>
    /// </summary>
    void DeselectAll();

    /// <summary>
    /// Redraws every previewed entity with the current <see cref="UnSelectedSettings"/>,
    /// so a change to those settings is seen immediately. The current visibility state is
    /// preserved.
    /// </summary>
    void RefreshAppearance();
}