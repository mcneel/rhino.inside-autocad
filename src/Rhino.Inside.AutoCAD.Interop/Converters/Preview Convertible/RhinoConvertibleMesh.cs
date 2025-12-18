using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Mesh. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleMesh : RhinoConvertibleBase<Rhino.Geometry.Mesh>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleMesh"/> instance.
    /// </summary>
    public RhinoConvertibleMesh(Mesh rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadMesh = _geometryConverter.ToAutoCadType(this.RhinoGeometry, transactionManager);

        var entity = new AutocadEntityWrapper(cadMesh);

        var entities = new List<IEntity> { entity };

        return entities;
    }
}