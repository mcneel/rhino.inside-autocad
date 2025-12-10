using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino SubD. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleSubD : RhinoConvertibleBase<Rhino.Geometry.SubD>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleSubD"/> instance.
    /// </summary>
    public RhinoConvertibleSubD(SubD rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadMesh = _geometryConverter.ToAutoCadType(this.RhinoGeometry);

        var entity = new AutocadEntityWrapper(cadMesh);

        var entities = new List<IEntity> { entity };

        return entities;
    }
}
