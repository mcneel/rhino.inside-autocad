namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A wrapper for the <see cref="ITypedValueWrapper"/> struct that allows it to be used in
/// contexts where an object is required, such as in selection filters. This wrapper
/// provides access to the type code and value of the underlying <see
/// cref="ITypedValueWrapper"/> instance.
/// </summary>
public interface ITypedValueWrapper
{
    /// <summary>
    /// The type code of the wrapped <see cref="ITypedValueWrapper"/>. This code relates to
    /// the DxfCode of data stored in the <see cref="Value"/> property and is used
    /// by AutoCAD to interpret the value correctly.
    /// </summary>
    short TypeCode { get; }

    /// <summary>
    /// The value of the wrapped <see cref="ITypedValueWrapper"/>. This is the actual data that
    /// will be used in selection filters or other contexts where a <see
    /// cref="ITypedValueWrapper"/> is required. The type of this value should correspond to
    /// the <see cref="TypeCode"/> property to ensure correct interpretation by
    /// AutoCAD.
    /// </summary>
    object Value { get; }
}