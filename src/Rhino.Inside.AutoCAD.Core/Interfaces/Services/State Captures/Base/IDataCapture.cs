namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface which defines a type which can capture copies of its internal data.
/// This is similar to <see cref="IStateCapture{TCapture}"/> but without the restore
/// method. Instead, it is used to capture data which can be restored via the constructor.
/// Typically, this is used by class which inherit from <see cref="IDataCaptureCache"/>.
/// </summary>
public interface IDataCapture<out TCapture>
{
    /// <summary>
    /// Captures the current state of this <typeparamref name="TCapture"/>.
    /// </summary>
    TCapture CaptureData();
}