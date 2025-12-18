namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A strongly-typed Rhino geometry convertible interface.
/// </summary>
public interface IRhinoConvertibleTyped<TRhinoType> : IRhinoConvertible
    where TRhinoType : Rhino.Geometry.GeometryBase
{
    /// <summary>
    /// The underlying Rhino geometry.
    /// </summary>
    TRhinoType RhinoGeometry { get; }
}