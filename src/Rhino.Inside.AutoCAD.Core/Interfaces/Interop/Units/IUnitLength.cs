using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which encapsulates a dimensional length value and its
/// <see cref="UnitSystem"/>.
/// </summary>
/// <remarks>
/// A <see cref="IUnitLength"/> encapsulates a value and its <see cref="UnitSystem"/>.
/// This is useful for binding to UI controls which need to display values in the
/// which require a specific units system that differ form the
/// <see cref="IAutoCadDocument.UnitSystem"/>.
/// </remarks>
public interface IUnitLength
{
    /// <summary>
    /// The event raised when the <see cref="Value"/> changes.
    /// </summary>
    event EventHandler? LengthChanged;

    /// <summary>
    /// The <see cref="UnitSystem"/> of the <see cref="Value"/>.
    /// </summary>
    UnitSystem UnitSystem { get; }

    /// <summary>
    /// The value of this <see cref="IUnitLength"/> in the
    /// <see cref="IAutoCadDocument.UnitSystem"/>.
    /// </summary>
    double Value { get; set; }

    /// <summary>
    /// The display name of the <see cref="UnitSystem"/>.
    /// </summary>
    [JsonIgnore]
    string DisplayName { get; }
}