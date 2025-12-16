using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A geometry converter class for converting between AutoCAD and Rhino geometry
/// types. The converter provides singleton access to the converter instance
/// for global use. The converter uses the <see cref="IUnitSystemManager"/>
/// to convert all units to <see cref="IUnitSystemManager.RhinoUnits"/> units.
/// </summary>
public partial class GeometryConverter
{
    private readonly IUnitSystemManager _unitSystemManager;
    private readonly IBrepConverterRunner _brepConverterRunner;

    private readonly double _fitTolerance = GeometryConstants.FitTolerance;
    private readonly double _midPointParam = GeometryConstants.NormalizedMidLength;
    private readonly double _zeroTolerance = GeometryConstants.ZeroTolerance;
    private readonly double _zeroAngleTolerance = GeometryConstants.RadianAngleTolerance;
    private readonly double _zeroWidth = GeometryConstants.AbsoluteZeroValue;
    private readonly HatchLoopTypes _externalType = HatchLoopTypes.External;
    private readonly HatchLoopTypes _outermostType = HatchLoopTypes.Outermost;

    /// <summary>
    /// Returns the <see cref="GeometryConverter"/> singleton.
    /// </summary>
    public static GeometryConverter? Instance { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="GeometryConverter"/>.
    /// </summary>
    private GeometryConverter(IUnitSystemManager unitSystemManager,
        IBrepConverterRunner brepConverterRunner)
    {
        _unitSystemManager = unitSystemManager;
        _brepConverterRunner = brepConverterRunner;

    }

    /// <summary>
    /// Explicit static constructor to tell C# compiler
    /// not to mark type as <see cref="beforefieldinit"/>.
    /// </summary>
    static GeometryConverter()
    {

    }

    /// <summary>
    /// Initializes the <see cref="GeometryConverter"/> singleton.
    /// </summary>
    public static void Initialize(IUnitSystemManager unitSystemManager,
        IBrepConverterRunner brepConverterRunner)
    {
        Instance = new GeometryConverter(unitSystemManager, brepConverterRunner);
    }
}