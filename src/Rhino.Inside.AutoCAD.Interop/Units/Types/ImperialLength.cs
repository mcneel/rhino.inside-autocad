using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IImperialLength"/>
public class ImperialLength : IImperialLength
{
    /// <inheritdoc/>
    public event EventHandler? LengthChanged;

    /// <inheritdoc/>
    public IUnitLength Feet { get; }

    /// <inheritdoc/>
    public IUnitLength Inches { get; }

    /// <summary>
    /// Constructs a new <see cref="ImperialLength"/>.
    /// </summary>
    public ImperialLength(double feet = 0.0, double inches = 0.0)
    {
        var feeLength = new UnitLength(feet, UnitSystem.Feet);

        var inchLength = new UnitLength(inches, UnitSystem.Inches);

        feeLength.LengthChanged += this.OnFeetChanged;
        inchLength.LengthChanged += this.OnInchesChanged;

        this.Feet = feeLength;

        this.Inches = inchLength;
    }

    /// <summary>
    /// Event which fires when the <see cref="Inches"/>
    /// <see cref="IUnitLength.LengthChanged"/> event is raised.
    /// Raises the <see cref="LengthChanged"/> event.
    /// </summary>
    private void OnInchesChanged(object sender, EventArgs e)
    {
        this.OnLengthChanged();
    }

    /// <summary>
    /// Event which fires when the <see cref="Feet"/>
    /// <see cref="IUnitLength.LengthChanged"/> event is raised.
    /// Raises the <see cref="LengthChanged"/> event.
    /// </summary>
    private void OnFeetChanged(object sender, EventArgs e)
    {
        this.OnLengthChanged();
    }

    /// <inheritdoc/>
    public void SetTo(IImperialLength candidateLength)
    {
        this.Feet.Value = candidateLength.Feet.Value;

        this.Inches.Value = candidateLength.Inches.Value;
    }

    /// <summary>
    /// Raises the <see cref="LengthChanged"/> event.
    /// </summary>
    protected virtual void OnLengthChanged()
    {
        LengthChanged?.Invoke(this, EventArgs.Empty);
    }
}