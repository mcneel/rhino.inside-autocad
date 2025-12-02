namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining a list of in-use layers that host <see cref="IHatch"/>es.
/// </summary>
public interface IHatchLayerSet : IEnumerable<ILayer>
{
    /// <summary>
    /// Returns a <see cref="ILayer"/> from this <see cref="IHatchLayerSet"/>
    /// that matches the <paramref name="layerName"/> otherwise returns null.
    /// </summary>
    ILayer? GetLayerByName(string layerName);
}