namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface defining a list of in-use layers that host <see cref="IHatch"/>es.
/// </summary>
public interface IHatchLayerSet : IEnumerable<IAutocadLayerTableRecord>
{
    /// <summary>
    /// Returns a <see cref="IAutocadLayerTableRecord"/> from this <see cref="IHatchLayerSet"/>
    /// that matches the <paramref name="layerName"/> otherwise returns null.
    /// </summary>
    IAutocadLayerTableRecord? GetLayerByName(string layerName);
}