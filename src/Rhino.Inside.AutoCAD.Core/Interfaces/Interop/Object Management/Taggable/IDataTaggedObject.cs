namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an object that can be tagged with Project Wide Database for data capture.
/// </summary>
public interface IDataTaggedObject
{
    /// <summary>
    /// Captures the data from the <see cref="IDataTaggedObject"/> and stores it in the
    /// provided <paramref name="record"/>.
    /// </summary>
    void CaptureData(IDataTagRecord record);

    /// <summary>
    /// Restore the values in the <see cref="IDataTaggedObject"/> type from the given
    /// <paramref name="record"/>.
    /// </summary>
    void RestoreFrom(IDataTagRecord record);
}