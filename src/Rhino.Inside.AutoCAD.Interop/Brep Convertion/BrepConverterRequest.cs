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

    /// <inheritdoc />
    public IBakeSettings? Settings { get; }

    /// <inheritdoc />
    public IObjectId? TargetBlockTableRecordId { get; }

    /// <summary>
    /// Constructs a new <see cref="IBrepConverterRequest"/>
    /// </summary>
    /// <param name="brep">
    /// The Brep to be converted
    /// </param>
    /// <param name="callback">
    /// A callback function invoked once the conversion has occured.
    /// </param>
    /// <param name="settings">
    /// Optional bake settings (layer, linetype, color) applied to the converted entities.
    /// </param>
    /// <param name="targetBlockTableRecordId">
    /// Optional block table record to take ownership of the converted solids. When null
    /// the solids remain in model space.
    /// </param>
    public BrepConverterRequest(Brep brep, Func<IBrepConverterResult, bool> callback,
        IBakeSettings? settings = null, IObjectId? targetBlockTableRecordId = null)
    {
        this.BrepToConvert = brep;
        this.Callback = callback;
        this.Settings = settings;
        this.TargetBlockTableRecordId = targetBlockTableRecordId;
    }
}