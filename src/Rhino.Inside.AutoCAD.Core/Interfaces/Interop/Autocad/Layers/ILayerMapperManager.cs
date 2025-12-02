namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which stores and manage <see cref="ILayerMapper"/>s.
/// </summary>
public interface ILayerMapperManager : IEnumerable<ILayerMapper>
{
    /// <summary>
    /// Event raised when any <see cref="ILayerMapper.SelectedLayerChanged"/>
    /// event is raised.
    /// </summary>
  //  event EventHandler<ILayerSelectedEventArgs>? SelectedLayerChanged;

    /// <summary>
    /// Remaps <see cref="IEntity"/> instances linked with the <see cref="ILayerMapper.HostLayer"/> to the
    /// <see cref="ILayerMapper.SelectedLayer"/> across all <see cref="ILayerMapper"/> instances within this
    /// <see cref="ILayerMapperManager"/>.
    /// </summary>
    /// <remarks>
    /// In the course of this operation, the <see cref="IEntity.Layer"/> property of each associated entity 
    /// originally aligned with the <see cref="ILayerMapper.HostLayer"/> is updated to reference the
    /// <see cref="ILayerMapper.SelectedLayer"/>, thereby shifting the entity's layer affiliation.
    /// </remarks>
    void MapAll();
}