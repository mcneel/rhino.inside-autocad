using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDegreeAngle"/>
public class DegreeAngle : IDegreeAngle, INotifyPropertyChanged, IDataErrorInfo
{
    private const double _pi = Math.PI;

    private readonly double _degreeToRadians = _pi / 180.0;
    private readonly double _radiansToDegree = 180.0 / _pi;

    private double _rotation = 0.0;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event EventHandler? AngleChanged;

    /// <inheritdoc/>
    public double Value
    {
        get => _rotation;
        set
        {
            if (value.Equals(_rotation) || this.ValidateAngle(value) == false)
                return;

            _rotation = value;

            OnPropertyChanged();
            OnAngleChanged(EventArgs.Empty);
        }
    }

    /// <inheritdoc/>
    public double MinAngle { get; }

    /// <inheritdoc/>
    public double MaxAngle { get; }

    /// <inheritdoc/>
    public string Error { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string this[string name] => this.Error;

    /// <inheritdoc/>
    public double AsRadians()
    {
        return this.Value * _degreeToRadians;
    }

    /// <summary>
    /// Constructs a new <see cref="DegreeAngle"/>.
    /// </summary>
    /// <param name="value">
    /// The degree angle value.
    /// </param>
    /// <param name="maxAngle">
    /// The <see cref="MaxAngle"/> in radians. 
    /// </param>
    /// <param name="minAngle">
    /// The <see cref="MinAngle"/> in radians. 
    /// </param>
    [JsonConstructor]
    public DegreeAngle(double value, double maxAngle = _pi * 2, double minAngle = -_pi * 2)
    {
        this.MaxAngle = maxAngle;

        this.MinAngle = minAngle;

        // Important: Must set last due to validation check. 
        this.Value = value;
    }

    /// <summary>
    /// Default constructor. 
    /// </summary>
    public DegreeAngle()
    {
        this.MaxAngle = _pi;

        this.MinAngle = -_pi;
    }

    /// <summary>
    /// Returns the radians as a string with no decimal places.
    /// </summary>
    private string GetDegrees(double radians)
    {
        return Math.Round(radians * _radiansToDegree, 0).ToString("F0");
    }

    /// <summary>
    /// Validates the angle <see cref="Value"/>. Returns <see langword="true"/> if the angle
    /// is between the <see cref="MinAngle"/> and <see cref="MaxAngle"/> otherwise false.
    /// </summary>
    private bool ValidateAngle(double angleDegrees)
    {
        var angleRadians = angleDegrees * _degreeToRadians;

        var minAngle = this.MinAngle;

        var maxAngle = this.MaxAngle;

        var isValid = angleRadians >= minAngle & angleRadians <= maxAngle;

        this.Error = isValid ? string.Empty : $"Input an angle between the range {this.GetDegrees(minAngle)} ~ {this.GetDegrees(maxAngle)}";

        return isValid;
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Event handler which raises the <see cref="AngleChanged"/> event.
    /// </summary>
    protected virtual void OnAngleChanged(EventArgs e)
    {
        this.AngleChanged?.Invoke(this, e);
    }

    /// <inheritdoc/>
    public void SetRadians(double radians)
    {
        var degrees = _radiansToDegree * radians;

        this.Value = degrees;
    }
}