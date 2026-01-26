namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// A mapping of AutoCAD Dynamic Block Reference Property type codes. Do not change the
/// values as they map to AutoCAD's internal representation.
/// </summary>
public enum DynamicPropertyTypeCode
{
    /// <summary>
    /// Represents a null or undefined property type.
    /// </summary>
    Null = 0,

    /// <summary>
    /// Represents a real (double-precision floating-point) property type.
    /// </summary>
    Real = 1,

    /// <summary>
    /// Represents a 32-bit integer property type.
    /// </summary>
    Int32 = 2,

    /// <summary>
    /// Represents a 16-bit integer property type.
    /// </summary>
    Int16 = 3,

    /// <summary>
    /// Represents an 8-bit integer property type.
    /// </summary>
    Int8 = 4,

    /// <summary>
    /// Represents a text (string) property type.
    /// </summary>
    Text = 5,

    /// <summary>
    /// Represents a binary chunk property type.
    /// </summary>
    BChunk = 6,

    /// <summary>
    /// Represents a handle property type.
    /// </summary>
    Handle = 7,

    /// <summary>
    /// Represents a hard ownership ID property type.
    /// </summary>
    HardOwnershipId = 8,

    /// <summary>
    /// Represents a soft ownership ID property type.
    /// </summary>
    SoftOwnershipId = 9,

    /// <summary>
    /// Represents a hard pointer ID property type.
    /// </summary>
    HardPointerId = 10,

    /// <summary>
    /// Represents a soft pointer ID property type.
    /// </summary>
    SoftPointerId = 11,

    /// <summary>
    /// Represents a 3D real (vector or point) property type.
    /// </summary>
    Real3 = 12,

    /// <summary>
    /// Represents a 64-bit integer property type.
    /// </summary>
    Int64 = 13,

    /// <summary>
    /// Represents an unrecognized or unsupported property type.
    /// </summary>
    NotRecognized = 19
};