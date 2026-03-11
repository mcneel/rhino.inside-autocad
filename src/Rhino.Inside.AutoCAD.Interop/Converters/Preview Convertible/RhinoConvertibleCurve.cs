using Rhino.Inside.AutoCAD.Core.Interfaces;
using Curve = Rhino.Geometry.Curve;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino curve. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleCurve : RhinoConvertibleBase<Rhino.Geometry.Curve>
{
    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleCurve"/> instance.
    /// </summary>
    public RhinoConvertibleCurve(Curve rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadCurves = this.RhinoGeometry.ToAutocadCurves();

        var entities = new List<IEntity>();
        foreach (var cadCurve in cadCurves)
        {
            var entity = new AutocadEntityWrapper(cadCurve);

            entities.Add(entity);
        }

        return entities;
    }
}