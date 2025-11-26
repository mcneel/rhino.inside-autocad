using Autodesk.AutoCAD.DatabaseServices;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Extensions class for the <see cref="UnitsValue"/> enums.
/// </summary>
public static class UnitsValueExtensions
{
    /// <summary>
    /// Converts a <see cref="UnitsValue"/> to a <see cref="UnitSystem"/>.
    /// </summary>
    public static UnitSystem ToUnitSystem(this UnitsValue unitsValue)
    {
        var unitSystemResult = Enum.TryParse(unitsValue.ToString(), out UnitSystem documentUnitSystem);

        return unitSystemResult ? documentUnitSystem : UnitSystem.Unset;
    }
}