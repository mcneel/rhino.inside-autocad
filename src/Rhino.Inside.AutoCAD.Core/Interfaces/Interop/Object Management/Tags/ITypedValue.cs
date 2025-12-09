namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A <see cref="ITypedValue"/> is a key-value pair object comprising a
/// <see cref="GroupCodeValue"/> and object value. <see cref="ITypedValue"/>s are
/// attached to an <see cref="IDbObject"/> in the active <see cref="IAutocadDocument"/>
/// (via Extensible Storage) to provide persistent property-based information
/// that corresponds to a property of an internal API type in this application.
/// <see cref="ITypedValue"/>s are somewhat limited in scope as the
/// <see cref="GroupCodeValue"/>s only have a small range of available values which
/// can be found
/// <see href="https://help.autodesk.com/view/OARX/2023/ENU/?guid=GUID-2553CF98-44F6-4828-82DD-FE3BC7448113">
/// here.</see>
/// A <see cref="GroupCodeValue"/> can be used more than once for a
/// <see cref="ITypedValue"/> in a <see cref="IXRecord"/>, however it is best
/// practice to use the <see cref="GroupCodeValue"/> as a unique key as it makes
/// the use of the tag more determinate and predicable when calling
/// <see IXRecordrecord.GetFirstValueAt{T}"/>. Collections are one exception
/// where the same <see cref="GroupCodeValue"/> must be used more than once (as an
/// array can't be stored in a <see cref="Value"/>). In such cases, the
/// <see cref="GroupCodeValue"/> used must be unique for the collection to prevent
/// data contamination; if the same value type as that of the group is added but
/// is not part of the collection, it must use a different <see cref="GroupCodeValue"/>. 
/// </summary>
/// <remarks>
/// A <see cref="ITypedValue"/> is the equivalent to an AutoCAD TypedValue stored within
/// the XData property of a DBObject. 
/// </remarks>
public interface ITypedValue
{
    /// <summary>
    /// The unique Group Number Value Types Reference (DXF) key. Used for lookup
    /// purposes when stored in a <see cref="IXRecord"/>. See a table
    /// of all the available DXF codes
    /// <see href="https://help.autodesk.com/view/OARX/2023/ENU/?guid=GUID-2553CF98-44F6-4828-82DD-FE3BC7448113">
    ///  here. </see>
    /// </summary>
    GroupCodeValue GroupCode { get; }

    /// <summary>
    /// The value of this <see cref="ITypedValue"/>. The value must be a single
    /// object and cannot be a collection type. If this <see cref="ITypedValue"/>
    /// is part of a collection of values, use a unique <see cref="GroupCode"/>
    /// for all values in the collection.
    /// </summary>
    object Value { get; }
}