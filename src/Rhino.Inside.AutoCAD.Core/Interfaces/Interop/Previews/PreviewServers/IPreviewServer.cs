namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Rhino object in AutoCAD using transients, one
/// <see cref="IPreviewDrawable"/> per registered object.
/// </summary>
public interface IPreviewServer
{
    /// <summary>
    /// The <see cref="IObjectRegister"/> used to track the drawables being previewed.
    /// </summary>
    IObjectRegister ObjectRegister { get; }

    /// <summary>
    /// A value indicating whether the transients in this <see cref="IPreviewServer"/>
    /// are currently displayed. Set to false by <see cref="ClearServer"/> and true by
    /// <see cref="PopulateServer"/>; while false, newly added objects are registered but
    /// not drawn.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    /// The most preview items this <see cref="IPreviewServer"/> holds. Adding an object which
    /// would exceed it drops the oldest objects first, and an object larger than it on its
    /// own is truncated. Lowering it drops the oldest objects straight away.
    /// </summary>
    /// <seealso cref="IUserSettings.MaxPreviewEntityCount"/>
    int MaxEntityCount { get; set; }

    /// <summary>
    /// Adds the provided <paramref name="rhinoConvertibleSet"/> into this <see cref=
    /// "IPreviewServer"/> as a single drawable, replacing any already registered under
    /// <paramref name="rhinoObjectId"/> and dropping the oldest objects if needed to stay
    /// within <see cref="MaxEntityCount"/>.
    /// </summary>
    void AddObject(Guid rhinoObjectId, IRhinoConvertibleSet rhinoConvertibleSet, bool selected);

    /// <summary>
    /// Removes the provided <paramref name="rhinoObjectId"/> from this <see cref=
    /// "IPreviewServer"/>, erasing and disposing its drawable.
    /// </summary>
    void RemoveObject(Guid rhinoObjectId);

    /// <summary>
    /// Sets the selection state of the drawable registered under
    /// <paramref name="rhinoObjectId"/> and redraws it, without rebuilding it.
    /// </summary>
    /// <returns>
    /// True when an object is registered under <paramref name="rhinoObjectId"/>, otherwise
    /// false, in which case the caller has to add the object to preview it.
    /// </returns>
    bool SetSelected(Guid rhinoObjectId, bool selected);

    /// <summary>
    /// Erases all the transients which are in the <see cref="IObjectRegister"/> from the
    /// AutoCAD drawing but keeps their drawables in the register for later re-use.
    /// Used for visibility toggling (preview on/off).
    /// </summary>
    void ClearServer();

    /// <summary>
    /// Erases all transients, disposes their drawables and empties the register.
    /// Used during application shutdown to ensure clean disposal.
    /// </summary>
    void ClearAndDisposeAll();

    /// <summary>
    /// Adds a transient to the AutoCAD drawing for every drawable which is in the
    /// <see cref="IObjectRegister"/>.
    /// </summary>
    void PopulateServer();

    /// <summary>
    /// Deselects every drawable in the preview server, so it draws with the unselected
    /// settings.
    /// </summary>
    public void DeselectAll();

    /// <summary>
    /// Redraws every drawable in the <see cref="IObjectRegister"/> with the current preview
    /// settings, so a change to the settings is seen without waiting for the previews to
    /// expire.
    /// </summary>
    /// <remarks>
    /// Each drawable keeps its selection state, so selected previews stay drawn with the
    /// selected settings. The visibility state is preserved.
    /// </remarks>
    void RefreshAppearance();
}
