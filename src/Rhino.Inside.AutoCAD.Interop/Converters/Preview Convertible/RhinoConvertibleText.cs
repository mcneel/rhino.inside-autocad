using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A <see cref="IRhinoConvertible"/> Rhino Text. <inheritdoc
/// cref="IRhinoConvertibleTyped{TRhinoType}"/>.
/// </summary>
public class RhinoConvertibleText : RhinoConvertibleBase<Rhino.Geometry.TextEntity>
{
    /// <summary>
    /// Constructs a new <see cref="RhinoConvertibleText"/> instance.
    /// </summary>
    public RhinoConvertibleText(TextEntity rhinoGeometry) : base(rhinoGeometry)
    {
    }

    /// <inheritdoc />
    protected override List<IEntity> ConvertGeometry(ITransactionManager transactionManager)
    {
        var cadText = this.RhinoGeometry.ToAutocadMText();

        return [new AutocadEntityWrapper(cadText)];
    }
}