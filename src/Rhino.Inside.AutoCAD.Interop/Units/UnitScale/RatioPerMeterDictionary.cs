namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides a lookup dictionary that maps <see cref="UnitSystem"/> values to their corresponding
/// <see cref="Ratio"/> relative to meters.
/// </summary>
/// <remarks>
/// This class stores exact rational representations of unit conversion factors to avoid floating-point
/// precision loss during unit conversions. Each ratio represents how many meters one unit equals
/// (e.g., 1 inch = 254/10000 meters = 0.0254 meters). Imperial units use exact definitions based on
/// the international yard and pound agreement (1 inch = 25.4 mm exactly).
/// </remarks>
/// <seealso cref="Ratio"/>
/// <seealso cref="UnitSystem"/>
public class RatioPerMeterDictionary
{
    /// <summary>
    /// Dictionary mapping each <see cref="UnitSystem"/> to its <see cref="Ratio"/> relative to meters.
    /// </summary>
    /// <remarks>
    /// Special cases:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <see cref="UnitSystem.None"/> returns (-1, -1) to indicate an invalid/undefined state.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <see cref="UnitSystem.CustomUnits"/> returns (0, 1) as a placeholder requiring user-defined scaling.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <see cref="UnitSystem.Unset"/> returns (0, 0) resulting in NaN to indicate uninitialized state.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <see cref="UnitSystem.Parsecs"/> uses π in the denominator for the exact astronomical definition.
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    private static readonly Dictionary<UnitSystem, Ratio> _ratios = new()
    {
        { UnitSystem.None,              new Ratio(-1, -1)                     },
        { UnitSystem.Microns,           new Ratio(1, 1_000_000)               },
        { UnitSystem.Millimeters,       new Ratio(1, 1_000)                   },
        { UnitSystem.Centimeters,       new Ratio(1, 100)                     },
        { UnitSystem.Meters,            new Ratio(1, 1)                       },
        { UnitSystem.Kilometers,        new Ratio(1_000, 1)                   },
        { UnitSystem.Microinches,       new Ratio(254, 10_000_000_000)        },
        { UnitSystem.Mils,              new Ratio(254, 10_000_000)            },
        { UnitSystem.Inches,            new Ratio(254, 10_000)                },
        { UnitSystem.Feet,              new Ratio(3_048, 10_000)              },
        { UnitSystem.Miles,             new Ratio(1_609_344, 1_000)           },
        { UnitSystem.CustomUnits,       new Ratio(0, 1)                       },
        { UnitSystem.Angstroms,         new Ratio(1, 10_000_000_000)          },
        { UnitSystem.Nanometers,        new Ratio(1, 1_000_000_000)           },
        { UnitSystem.Decimeters,        new Ratio(1, 10)                      },
        { UnitSystem.Dekameters,        new Ratio(10, 1)                      },
        { UnitSystem.Hectometers,       new Ratio(100, 1)                     },
        { UnitSystem.Megameters,        new Ratio(1_000_000, 1)               },
        { UnitSystem.Gigameters,        new Ratio(1_000_000_000, 1)           },
        { UnitSystem.Yards,             new Ratio(9_144, 10_000)              },
        { UnitSystem.PrinterPoints,     new Ratio(254, 72_000)                },
        { UnitSystem.PrinterPicas,      new Ratio(254, 6_000)                 },
        { UnitSystem.NauticalMiles,     new Ratio(1_852, 1)                   },
        { UnitSystem.AstronomicalUnits, new Ratio(149_597_870_700, 1)         },
        { UnitSystem.LightYears,        new Ratio(9_460_730_472_580_800, 1)   },
        { UnitSystem.Parsecs,           new Ratio(96_939_420_213_600_000, Math.PI) },
        { UnitSystem.Unset,             new Ratio(0, 0)                       },
    };

    /// <summary>
    /// Retrieves the <see cref="Ratio"/> representing how many meters one unit of the specified
    /// <see cref="UnitSystem"/> equals.
    /// </summary>
    /// <param name="unitSystem">
    /// The unit system to look up.
    /// </param>
    /// <returns>
    /// A <see cref="Ratio"/> where the quotient represents the number of meters per unit.
    /// For example, <see cref="UnitSystem.Inches"/> returns 254/10000 (0.0254 meters per inch).
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when <paramref name="unitSystem"/> is not defined in the dictionary.
    /// </exception>
    public static Ratio Lookup(UnitSystem unitSystem)
    {
        if (_ratios.TryGetValue(unitSystem, out var ratio))
            return ratio;

        throw new KeyNotFoundException($"No ratio defined for UnitSystem '{unitSystem}'.");
    }
}