using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBrepConverterRequest"/>
public class BrepConverterRequest : IBrepConverterRequest
{
    /// <inheritdoc />
    public Brep BrepToConvert { get; }

    /// <inheritdoc />
    public Func<IBrepConverterResult, bool> Callback { get; }

    /// <summary>
    /// Constructs a new <see cref="IBrepConverterRequest"/>
    /// </summary>
    /// <param name="brep">
    /// The Brep to be converted
    /// </param>
    /// <param name="callback">
    /// A callback function invoked once the conversion has occured.
    /// </param>
    public BrepConverterRequest(Brep brep, Func<IBrepConverterResult, bool> callback)
    {
        this.BrepToConvert = brep;
        this.Callback = callback;
    }
}