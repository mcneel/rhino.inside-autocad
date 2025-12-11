namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A factory that creates <see cref="IRhinoConvertible"/> instances from Rhino geometries.
/// </summary>

public interface IRhinoConvertibleFactory
{
    /// <summary>
    /// Creates an <see cref="IRhinoConvertible"/> from the provided Rhino geometry if
    /// the conversion is supported.
    /// </summary>
    bool MakeConvertible<TRhinoType>(TRhinoType rhinoGeometry,
        out IRhinoConvertible? result)
        where TRhinoType : Rhino.Geometry.GeometryBase;
}

