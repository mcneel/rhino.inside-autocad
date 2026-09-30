namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Rhino object in AutoCAD using transient entities.
/// </summary>
public interface IPreviewServer
{
    /// <summary>
    /// The <see cref="IObjectRegister"/> used to track the entities being previewed.
    /// </summary>
    IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// A value indicating whether the transient entities in this <see cref="IPreviewServer"/>
    /// are currently displayed. Set to false by <see cref="ClearServer"/> and true by
    /// <see cref="PopulateServer"/>; while false, newly added objects are registered but
    /// not drawn.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    /// The most entities this <see cref="IPreviewServer"/> holds. Adding an object which
    /// would exceed it drops the oldest objects first, and an object larger than it on its
    /// own is truncated. Lowering it drops the oldest objects straight away.
    /// </summary>
    /// <seealso cref="IUserSettings.MaxPreviewEntityCount"/>
    int MaxEntityCount { get; set; }

    /// <summary>
    /// Adds the provided <paramref name="rhinoConvertibleSet"/> into this <see cref=
    /// "IPreviewServer"/>, dropping the oldest objects if needed to stay within
    /// <see cref="MaxEntityCount"/>.
    /// </summary>
    void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet, bool selected);

    /// <summary>
    /// Removes the provided <paramref name="rhinoObjectId"/> from this <see cref=
    /// "IPreviewServer"/>.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);

    /// <summary>
    /// Removes all the transient entities which are in the <see cref="IObjectRegister"/>
    /// from the AutoCAD drawing but keeps them in the register for later re-use.
    /// Used for visibility toggling (preview on/off).
    /// </summary>
    void ClearServer();

    /// <summary>
    /// Removes all transient entities and disposes the underlying AutoCAD entities.
    /// Used during application shutdown to ensure clean disposal.
    /// </summary>
    void ClearAndDisposeAll();

    /// <summary>
    /// Adds all the transient entities which are in the <see cref="IObjectRegister"/>
    /// from the AutoCAD drawing.
    /// </summary>
    void PopulateServer();

    /// <summary>
    /// Deselects all the entities in the preview server by applying the unselected settings to them.
    /// </summary>
    public void DeselectAll();

    /// <summary>
    /// Reapplies the preview settings to every entity in the <see cref="IObjectRegister"/> and
    /// redraws them, so a change to the settings is seen without waiting for the previews to
    /// expire.
    /// </summary>
    /// <remarks>
    /// The register does not record which objects are selected, so entities drawn with the
    /// selected settings revert to the unselected ones until the next selection change.
    /// </remarks>
    void RefreshAppearance();
}
