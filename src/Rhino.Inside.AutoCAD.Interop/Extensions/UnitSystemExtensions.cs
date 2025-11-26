namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An extension class for the <see cref="UnitSystem"/> enumeration.
/// </summary>
public static class UnitSystemExtensions
{
    /// <summary>
    /// Converts a <see cref="UnitSystem"/> value to symbolic or abbreviated form.
    /// </summary>
    public static string GetDisplayName(this UnitSystem unitSystem)
    {
        switch (unitSystem)
        {
            case UnitSystem.Millimeters: return "mm";

            case UnitSystem.Centimeters: return "cm";

            case UnitSystem.Meters: return "m";

            case UnitSystem.Inches: return "in";

            case UnitSystem.Feet: return "ft";

            default: return "?";
        }
    }
}