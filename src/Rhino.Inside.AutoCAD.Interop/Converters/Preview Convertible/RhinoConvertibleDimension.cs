using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Dimension. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleDimension : RhinoConvertibleBase<Rhino.Geometry.Dimension>
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;

    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleDimension"/> instance.
    /// </summary>
    public RhinoConvertibleDimension(Dimension rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadDimension = _geometryConverter.ToAutoCadType(this.RhinoGeometry);

        if (cadDimension == null) return [];

        return [new AutocadEntityWrapper(cadDimension)];
    }
}
