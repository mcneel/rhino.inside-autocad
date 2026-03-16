using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Surface. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleSurface : RhinoConvertibleBase<Rhino.Geometry.Surface>
{
    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleSurface"/> instance.
    /// </summary>
    public RhinoConvertibleSurface(Surface rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadSolid = this.RhinoGeometry.ToAutocadNurbSurface();

        var entity = new AutocadEntityWrapper(cadSolid);

        var entities = new List<IEntity> { entity };

        return entities;
    }
}