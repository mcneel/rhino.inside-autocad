using Rhino.Inside.AutoCAD.Core.Interfaces;
using Point = Rhino.Geometry.Point;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Point. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertiblePoint : RhinoConvertibleBase<Rhino.Geometry.Point>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertiblePoint"/> instance.
    /// </summary>
    public RhinoConvertiblePoint(Point rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadPoint3d = _geometryConverter.ToAutoCadType(this.RhinoGeometry.Location);

        var dbPoint = new Autodesk.AutoCAD.DatabaseServices.DBPoint(cadPoint3d);

        dbPoint.Thickness = 10;

        var entity = new AutocadEntityWrapper(dbPoint);

        return [entity];
    }
}