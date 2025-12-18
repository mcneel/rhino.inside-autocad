using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Surface. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleSurface : RhinoConvertibleBase<Rhino.Geometry.Surface>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleSurface"/> instance.
    /// </summary>
    public RhinoConvertibleSurface(Surface rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadSolid = _geometryConverter.ToAutoCadType(this.RhinoGeometry);

        var entity = new AutocadEntityWrapper(cadSolid);

        var entities = new List<IEntity> { entity };

        return entities;
    }
}