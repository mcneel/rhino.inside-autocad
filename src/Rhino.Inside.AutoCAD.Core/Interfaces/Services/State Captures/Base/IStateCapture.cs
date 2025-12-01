namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which defines a type which can capture and restore copies
/// of its internal state.
/// </summary>
public interface IStateCapture<TCapture>
{
    /// <summary>
    /// Restores this <see cref="ICeilingInstance"/> to the state stored in the
    /// <paramref name="capture"/>.
    /// </summary>
    void RestoreState(TCapture capture);

    /// <summary>
    /// Captures the current state of this <see cref="ICeilingInstance"/>.
    /// </summary>
    TCapture CaptureState();
}