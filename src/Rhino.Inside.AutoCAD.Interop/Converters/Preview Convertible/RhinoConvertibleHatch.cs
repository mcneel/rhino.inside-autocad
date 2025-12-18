using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Hatch. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleHatch : RhinoConvertibleBase<Rhino.Geometry.Hatch>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleHatch"/> instance.
    /// </summary>
    public RhinoConvertibleHatch(Hatch rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadSolid = _geometryConverter.ToAutoCadType(this.RhinoGeometry, transactionManager);

        var entity = new AutocadEntityWrapper(cadSolid);

        var entities = new List<IEntity> { entity };

        return entities;
    }
}
