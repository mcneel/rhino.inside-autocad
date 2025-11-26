namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// Represents a list of dynamic properties created by AWI and found in
/// various <see cref="IBlockTableRecord"/>s that are used as CAD resources
/// by the application to generate drawing information in the <see cref=
/// "IDocument"/>, such as the RCP Solver and Section Detail Tool.
/// </summary>
public enum CustomPropertyType
{
    /// <summary>
    /// Invalid property type.
    /// </summary>
    None,

    /// <summary>
    /// The Length property of various <see cref="ISectionComponent"/>s.
    /// </summary>
    Length,

    /// <summary>
    /// TS-Code combo-box property of the left side of the <see cref=
    /// "IPanelInstance"/>.
    /// </summary>
    LeftSide,

    /// <summary>
    /// TS-Code combo-box property of the right side of the <see cref=
    /// "IPanelInstance"/>.
    /// </summary>
    RightSide,

    /// <summary>
    /// Single combo-box property of the <see cref="ISpring"/> dynamic
    /// <see cref="IBlockTableRecord"/>.
    /// </summary>
    Visibility,

    /// <summary>
    /// Distance property of the <see cref="ISectionComponent"/>. Typically,
    /// this property refers to any kind of vertical length.
    /// </summary>
    Distance,

    /// <summary>
    /// Material thickness custom property of the <see cref="ITrimTypeBlockTableRecord"/>.
    /// </summary>
    MaterialThickness,

    /// <summary>
    /// Sheet type custom property of the <see cref="ITrimTypeBlockTableRecord"/>.
    /// </summary>
    SheetType,

    /// <summary>
    /// Bottom flange custom property of the <see cref="ITrimTypeBlockTableRecord"/>.
    /// </summary>
    BottomFlange,

    /// <summary>
    /// Top and bottom flanges custom property of the <see cref="ITrimTypeBlockTableRecord"/>.
    /// </summary>
    TopBottomFlange,

    /// <summary>
    /// Height property custom of the <see cref="ITrimTypeBlockTableRecord"/>.
    /// </summary>
    Height,

    /// <summary>
    /// The property for selecting the view of an <see cref="ITee"/>. These views are
    /// either a Top View or a Side View but also include all the possible <see
    /// cref="ITeeCoupler"/> combinations for each view.
    /// </summary>
    TeeView,

    /// <summary>
    /// The flip property for setting the start <see cref="ITeeCoupler"/> in an
    /// <see cref="IExtrudedTee"/>.
    /// </summary>
    TeeCouplerStartFlippedState,

    /// <summary>
    /// The flip property for setting the end of a <see cref="ITeeCoupler"/> in an
    /// <see cref="IExtrudedTee"/>.
    /// </summary>
    TeeCouplerEndFlippedState,

    /// <summary>
    /// The property for setting the name of the <see cref="ISpring"/>'s view.
    /// </summary>
    SpringViewName,

    /// <summary>
    /// The property for selecting the view of an <see cref="IAccessory"/>. These views are
    /// either a Front View or a Side View.
    /// </summary>
    AccessoryView,
}