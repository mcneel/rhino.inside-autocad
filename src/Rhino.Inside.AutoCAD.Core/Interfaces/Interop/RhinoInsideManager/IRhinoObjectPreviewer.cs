namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Manages the preview of a Rhino object in AutoCAD using transient entities.
/// </summary>
public interface IRhinoObjectPreviewer
{
    /// <summary>
    /// The current visibility state of the preview.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    /// Toggles the visibility of all transient entities managed by the <see
    /// cref="ITransientManager"/> which are registered in the <see cref="IObjectRegister"/>.
    /// This will clear the transient entities if they are currently visible, or redraw them
    /// if they are  hidden based on the contents of the <see cref="IObjectRegister"/>.
    /// </summary>
    void ToggleVisibility();

    /// <summary>
    /// Adds the provided <paramref name="entity"/> into this <see cref="ITransientManager"/>.
    /// </summary>
    void AddEntity(IEntity entity);

    /// <summary>
    /// Adds the provided <paramref name="entities"/> into this <see cref="ITransientManager"/>.
    /// </summary>
    void AddEntities(IEnumerable<IEntity> entities);

    /// <summary>
    /// Removes the provided <paramref name="entity"/> from this <see cref="ITransientManager"/>.
    /// </summary>
    void RemoveEntity(IEntity entity);

    /// <summary>
    /// Removes the provided <paramref name="entities"/> from this <see cref="ITransientManager"/>.
    /// </summary>
    void RemoveEntities(IEnumerable<IEntity> entities);
}