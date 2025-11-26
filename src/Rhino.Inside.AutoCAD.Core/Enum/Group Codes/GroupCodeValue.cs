using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// DXF group codes define the type of the associated value in a
/// <see cref="IObjectIdTag"/> as an integer, a floating-point number,
/// or a string, according to the following table of group code ranges.
/// <see href="https://help.autodesk.com/view/OARX/2023/ENU/?guid=GUID-3F0380A5-1C15-464D-BC66-2C5F094BCFB9">
/// See a table of all the available DXF codes here.
/// </see>
/// </summary>
public enum GroupCodeValue : short
{
    /// <summary>
    /// Unknown/null group code value. -5 to 0 are reserved for internal use by AutoCAD. </summary>
    None = -6,

    /// <summary>
    /// String (255-character maximum, less for Unicode strings) using group code 1.
    /// </summary>
    _1 = 1,

    /// <summary>
    /// String (255-character maximum, less for Unicode strings) using group code 2.
    /// </summary>
    _2 = 2,

    /// <summary>
    /// String (255-character maximum, less for Unicode strings) using group code 3.
    /// </summary>
    _3 = 3,

    /// <summary>
    /// String (255-character maximum, less for Unicode strings) using group code 4.
    /// </summary>
    _4 = 4,

    /// <summary>
    /// String (255-character maximum, less for Unicode strings) using group code 4.
    /// </summary>
    _5 = 5,

    /// <summary>
    ///  Double precision 3D point value using group code 10.
    /// </summary>
    _10 = 10,

    /// <summary>
    /// Other points. DXF: X value of other points (followed by Y value codes 21-28
    /// and Z value codes 31-38). 3D point (list of three reals)
    /// </summary>
    _11 = 11,

    /// <summary>
    ///  Double precision 3D point value using group code 12.
    /// </summary>
    _12 = 12,

    /// <summary>
    ///  Double precision 3D point value using group code 13.
    /// </summary>
    _13 = 13,

    /// <summary>
    ///  Double precision 3D point value using group code 14.
    /// Other points. DXF: X value of other points (followed by Y value codes 21-28
    /// and Z value codes 31-38). 3D point (list of three reals)
    /// </summary>
    _14 = 14,

    /// <summary>
    /// Double precision 3D point value using group code 30.
    /// </summary>
    _30 = 30,

    /// <summary>
    /// Double-precision floating-point value using group code 40.
    /// </summary>
    _40 = 40,

    /// <summary>
    /// Double-precision floating-point value using group code 41.
    /// </summary>
    _41 = 41,

    /// <summary>
    /// 32-bit integer value using group code 90.
    /// </summary>
    _90 = 90,

    /// <summary>
    /// 32-bit integer value using group code 91.
    /// </summary>
    _91 = 91,

    /// <summary>
    /// 32-bit integer value using group code 92.
    /// </summary>
    _92 = 92,

    /// <summary>
    /// 32-bit integer value using group code 93.
    /// </summary>
    _93 = 93,

    /// <summary>
    /// 64-bit integer value using group code 160.
    /// </summary>
    _160 = 160,

    /// <summary>
    /// Boolean flag value using group code 290.
    /// </summary>
    _290 = 290,

    /// <summary>
    /// Boolean flag value using group code 291.
    /// </summary>
    _291 = 291,

    /// <summary>
    /// ObjectID flag value using group code 330.
    /// </summary>
    _330 = 330,

    /// <summary>
    /// String.
    /// </summary>
    _410 = 410,

    /// <summary>
    /// String.
    /// </summary>
    _411 = 411,

    /// <summary>
    /// String.
    /// </summary>
    _412 = 412,

    /// <summary>
    /// Double-precision floating-point value using group code 460.
    /// </summary>
    _460 = 460,

    /// <summary>
    /// String (255-character maximum, less for Unicode strings) using group code 470.
    /// </summary>
    _470 = 470,

    /// <summary>
    /// String (255-character maximum, less for Unicode strings) using group code 471.
    /// </summary>
    _471 = 471,

    /// <summary>
    /// ASCII string (up to 255 bytes long) in extended data.
    /// </summary>
    _1000 = 1000,

    /// <summary>
    /// Registered application name (ASCII string up to 31 bytes long) for extended data.
    /// </summary>
    _1001 = 1001,

    /// <summary>
    /// A 3D world space position in extended data 1011 DXF: X value (followed by 1021
    /// and 1031 groups) APP: 3D point
    /// </summary>
    _1011 = 1011,

    /// <summary>
    /// DXF: Y value of a world space position.
    /// </summary>
    _1021 = 1021,

    /// <summary>
    /// DXF: Z value of a world space position.
    /// </summary>
    _1031 = 1031,

    /// <summary>
    /// Extended data 16-bit signed integer.
    /// </summary>
    _1070 = 1070
}