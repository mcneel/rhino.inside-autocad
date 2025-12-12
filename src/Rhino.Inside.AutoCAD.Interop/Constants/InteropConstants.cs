using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constant class for interop-related values.
/// </summary>
public class InteropConstants
{

    /// <summary>
    /// The default AutoCAD layer name. 
    /// </summary>
    public const string DefaultLayerName = "0";

    /// <summary>
    /// The internal unit system used by AWI applications.
    /// </summary>
    public const UnitSystem FallbackUnitSystem = UnitSystem.Millimeters;

    /// <summary>
    /// The internal name of the application.
    /// </summary>
    public const string ApplicationName = "RHINO.INSIDE.AUTOCAD";

    /// <summary>
    /// The length in <see cref="IUnitSystemManager.RhinoUnits"/> of a pattern point
    /// in a <see cref="IAutocadLinetypeTableRecord"/> that is 0-length. The length is used to represent
    /// the point as a line internally.
    /// </summary>
    public const double LinePatternPointLength = 0.1;

    /// <summary>
    /// The total length in <see cref="IUnitSystemManager.RhinoUnits"/> of a
    /// <see cref="IAutocadLinetypeTableRecord"/>.
    /// </summary>
    public const double LinePatternTotalLength = 85.0;

    /// <summary>
    /// The file name of the Grasshopper library DLL.
    /// </summary>
    public const string GrasshopperLibraryFileName = "Rhino.Inside.AutoCAD.GrasshopperLibrary.dll";

    /// <summary>
    /// The fully qualified type name for the GH_AutocadGeometricGoo generic base type.
    /// </summary>
    public const string GooBaseTypeName = "Rhino.Inside.AutoCAD.GrasshopperLibrary.GH_AutocadGeometricGoo`2, Rhino.Inside.AutoCAD.GrasshopperLibrary";
}
