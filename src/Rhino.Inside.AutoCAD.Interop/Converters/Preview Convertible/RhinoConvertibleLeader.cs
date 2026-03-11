using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Leader. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleLeader : RhinoConvertibleBase<Rhino.Geometry.Leader>
{
    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleLeader"/> instance.
    /// </summary>
    public RhinoConvertibleLeader(Leader rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadLeader = this.RhinoGeometry.ToAutocadMLeader();

        if (cadLeader == null) return [];

        return [new AutocadEntityWrapper(cadLeader)];
    }
}