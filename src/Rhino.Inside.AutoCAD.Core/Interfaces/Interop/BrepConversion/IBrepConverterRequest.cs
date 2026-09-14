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

    /// <summary>
    /// Optional bake settings (layer, linetype, color) to apply to the converted
    /// entities. When null the defaults are used: layer 0 with ByLayer properties.
    /// </summary>
    IBakeSettings? Settings { get; }

    /// <summary>
    /// Optional block table record to take ownership of the converted solids once they
    /// have been imported. When null the solids remain in model space, which is the
    /// behaviour used by the bake components.
    /// </summary>
    IObjectId? TargetBlockTableRecordId { get; }
}