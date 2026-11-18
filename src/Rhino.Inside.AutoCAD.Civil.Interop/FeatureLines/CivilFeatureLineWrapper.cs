using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using RhinoCurve = Rhino.Geometry.Curve;
using RhinoPoint3d = Rhino.Geometry.Point3d;

namespace Rhino.Inside.AutoCAD.Civil.Interop;

/// <inheritdoc cref="ICivilFeatureLineWrapper"/>
public class CivilFeatureLineWrapper : AutocadEntityWrapper, ICivilFeatureLineWrapper
{
    private readonly FeatureLine _featureLine;

    /// <inheritdoc />
    public ICivilFeatureLineProperties Properties { get; }

    /// <summary>
    /// Constructs a new instance of <see cref="CivilFeatureLineWrapper"/> by wrapping a Civil
    /// 3D Feature Line object.
    /// </summary>
    public CivilFeatureLineWrapper(FeatureLine featureLine) : base(featureLine)
    {
        _featureLine = featureLine;
        this.Properties = new CivilFeatureLineProperties(featureLine);
    }

    /// <inheritdoc />
    public override IDbObject ShallowClone()
    {
        return new CivilFeatureLineWrapper(_featureLine);
    }

    /// <inheritdoc />
    public RhinoCurve? GetCurve() => _featureLine.ToRhinoCurve();

    /// <inheritdoc />
    public List<RhinoPoint3d> GetPIPoints() => this.GetPoints(FeatureLinePointType.PIPoint);

    /// <inheritdoc />
    public List<RhinoPoint3d> GetElevationPoints() => this.GetPoints(FeatureLinePointType.ElevationPoint);

    private List<RhinoPoint3d> GetPoints(FeatureLinePointType pointType)
    {
        var result = new List<RhinoPoint3d>();

        try
        {
            foreach (Autodesk.AutoCAD.Geometry.Point3d point in _featureLine.GetPoints(pointType))
            {
                result.Add(point.ToRhinoPoint3d());
            }
        }
        catch
        {
            // Return what was collected
        }

        return result;
    }
}
