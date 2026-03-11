using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Brep. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleBrep : RhinoConvertibleBase<Rhino.Geometry.Brep>
{
    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleBrep"/> instance.
    /// </summary>
    public RhinoConvertibleBrep(Brep rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadSolids = this.RhinoGeometry.ToAutocadNurbSurfaces();

        var entities = new List<IEntity>();
        foreach (var cadSolid in cadSolids)
        {
            var entity = new AutocadEntityWrapper(cadSolid);

            entities.Add(entity);
        }

        return entities;
    }
}