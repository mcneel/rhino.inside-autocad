using Rhino.Geometry;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A request to convert a Rhino <see cref="Brep"/> into an AutoCAD native entity.
/// </summary>
public interface IBrepConverterRequest
{
    /// <summary>
    /// The Rhino <see cref="Brep"/> to convert.
    /// </summary>
    Brep BrepToConvert { get; }

    /// <summary>
    /// A callback function to be invoked upon completion of the conversion
    /// process, both successful and unsuccessful.
    /// </summary>
    Func<IBrepConverterResult, bool> Callback { get; }
}