using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IUnitLength"/>
public class UnitLength : INotifyPropertyChanged, IUnitLength
{
    private double _value;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Returns the empty / zero <see cref="UnitLength"/>. Only used for cases
    /// where the value is not unset, e.g. null or unused.
    /// </summary>
    public static IUnitLength Zero = new UnitLength();

    /// <inheritdoc/>
    public event EventHandler? LengthChanged;

    /// <inheritdoc/>
    public double Value
    {
        get => _value;
        set
        {
            if (value.Equals(_value)) return;

            _value = value;

            this.OnPropertyChanged();
            this.OnLengthChanged();
        }
    }

    /// <inheritdoc/>
    public UnitSystem UnitSystem { get; }

    /// <inheritdoc/>
    [JsonIgnore]
    public string DisplayName { get; }

    /// <summary>
    /// Constructs a new <see cref="UnitLength"/>.
    /// </summary>
    [JsonConstructor]
    public UnitLength(double value, UnitSystem unitSystem)
    {
        this.Value = value;

        this.UnitSystem = unitSystem;

        this.DisplayName = unitSystem.GetDisplayName();
    }

    /// <summary>
    /// Constructs the empty / null <see cref="UnitLength"/>.
    /// </summary>
    private UnitLength()
    {
        var unitSystem = UnitSystem.None;

        this.Value = 0.0;

        this.UnitSystem = unitSystem;

        this.DisplayName = unitSystem.GetDisplayName();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Raises the <see cref="LengthChanged"/> event.
    /// </summary>
    protected virtual void OnLengthChanged()
    {
        LengthChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Returns the <see cref="Value"/> property as a string formatted with
    /// two decimal places.
    /// </summary>
    public override string ToString()
    {
        return $"{this.Value:F1}";
    }
}