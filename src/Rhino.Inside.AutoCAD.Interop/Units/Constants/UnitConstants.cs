using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Unit length and area conversion coefficient lookup tables.
/// </summary>
internal sealed class UnitConstants
{
    /// <summary>
    /// Units conversion table organized with a key of the <see cref="IAutocadDocument.UnitSystem"/>
    /// units system and value of a dictionary with a key of the
    /// <see cref="IUnitSystemManager.RhinoUnits"/> units system and corresponding conversion
    /// factor. E.g. <see cref="IAutocadDocument.UnitSystem"/> = Ft and <see cref="Internal"/>
    /// units = mm, so the units conversion factor is 304.8.
    /// </summary>
    internal static Dictionary<UnitSystem, Dictionary<UnitSystem, double>> LengthConversionFactors = new()
    {
        {
            UnitSystem.None, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1.0 },
                { UnitSystem.Centimeters, 1.0 },
                { UnitSystem.Meters, 1.0 },
                { UnitSystem.Inches, 1.0 },
                { UnitSystem.Feet, 1.0 }
            }
        },
        {
            UnitSystem.Unset, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1.0 },
                { UnitSystem.Centimeters, 1.0 },
                { UnitSystem.Meters, 1.0 },
                { UnitSystem.Inches, 1.0 },
                { UnitSystem.Feet, 1.0 },
            }
        },
        {
            UnitSystem.Millimeters, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1.0 },
                { UnitSystem.Centimeters, 0.1 },
                { UnitSystem.Meters, 0.001 },
                { UnitSystem.Inches, 0.0393700787 },
                { UnitSystem.Feet, 0.003280839895013 }
            }
        },
        {
            UnitSystem.Centimeters, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 10.0 },
                { UnitSystem.Centimeters, 1.0 },
                { UnitSystem.Meters, 0.01 },
                { UnitSystem.Inches, 0.3937007874 },
                { UnitSystem.Feet, 0.032808398950131 }
            }
        },
        {
            UnitSystem.Meters, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1000.0 },
                { UnitSystem.Centimeters, 100.0 },
                { UnitSystem.Meters, 1.0 },
                { UnitSystem.Inches, 39.3700787402 },
                { UnitSystem.Feet, 3.28083989501 }
            }
        },
        {
            UnitSystem.Inches, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 25.4 },
                { UnitSystem.Centimeters, 2.54 },
                { UnitSystem.Meters, 0.0254 },
                { UnitSystem.Inches, 1.0 },
                { UnitSystem.Feet, 0.083333333333 }
            }
        },
        {
            UnitSystem.Feet, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 304.8 },
                { UnitSystem.Centimeters, 30.48 },
                { UnitSystem.Meters, 0.3048 },
                { UnitSystem.Inches, 12.0 },
                { UnitSystem.Feet, 1.0 }
            }
        }
    };

    internal static Dictionary<UnitSystem, Dictionary<UnitSystem, double>> AreaConversionFactors = new()
    {
        {
            UnitSystem.None, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1.0 },
                { UnitSystem.Centimeters, 1.0 },
                { UnitSystem.Meters, 1.0 },
                { UnitSystem.Inches, 1.0 },
                { UnitSystem.Feet, 1.0 }
            }
        },
        {
            UnitSystem.Unset, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1.0 },
                { UnitSystem.Centimeters, 1.0 },
                { UnitSystem.Meters, 1.0 },
                { UnitSystem.Inches, 1.0 },
                { UnitSystem.Feet, 1.0 }
            }
        },
        {
            UnitSystem.Millimeters, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1.0 },
                { UnitSystem.Centimeters, 0.01 },
                { UnitSystem.Meters, 0.000001 },
                { UnitSystem.Inches, 0.0015500031000062 },
                { UnitSystem.Feet, 0.00001076391 }
            }
        },
        {
            UnitSystem.Centimeters, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 100.0 },
                { UnitSystem.Centimeters, 1.0 },
                { UnitSystem.Meters, 0.0001 },
                { UnitSystem.Inches, 0.15500031000062 },
                { UnitSystem.Feet, 0.00107639104 }
            }
        },
        {
            UnitSystem.Meters, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 1000000.0 },
                { UnitSystem.Centimeters, 10000.0 },
                { UnitSystem.Meters, 1.0 },
                { UnitSystem.Inches, 1550.0031 },
                { UnitSystem.Feet, 10.7639104 }
            }
        },
        {
            UnitSystem.Inches, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 645.16 },
                { UnitSystem.Centimeters, 6.4516 },
                { UnitSystem.Meters, 0.00064516 },
                { UnitSystem.Inches, 1.0 },
                { UnitSystem.Feet, 0.006944444444 }
            }
        },
        {
            UnitSystem.Feet, new Dictionary<UnitSystem, double>
            {
                { UnitSystem.None, 1.0 },
                { UnitSystem.Unset, 1.0 },
                { UnitSystem.Millimeters, 92903.04 },
                { UnitSystem.Centimeters, 929.0304 },
                { UnitSystem.Meters, 0.09290304 },
                { UnitSystem.Inches, 144.0 },
                { UnitSystem.Feet, 1.0 }
            }
        }
    };
}