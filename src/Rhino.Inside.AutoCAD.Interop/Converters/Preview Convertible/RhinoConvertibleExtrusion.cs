using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Extrusion. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleExtrusion : RhinoConvertibleBase<Rhino.Geometry.Extrusion>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleExtrusion"/> instance.
    /// </summary>
    public RhinoConvertibleExtrusion(Extrusion rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadSolids = _geometryConverter.ToAutoCadType(this.RhinoGeometry, transactionManager);

        var entities = new List<IEntity>();
        foreach (var cadSolid in cadSolids)
        {
            var entity = new AutocadEntityWrapper(cadSolid);

            entities.Add(entity);
        }

        return entities;
    }
}
