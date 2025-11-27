namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents any type of setting input(s) by either the user or set by the application
/// that stores the values in <see cref="IDataTag"/>s so its state can be stored and
/// retrieved from the active <see cref="IAutoCadDocument"/>, enabling the application to
/// retain the state of the input across all sessions.
/// </summary>
public interface IDataTaggedSettings
{
    /// <summary>
    /// Captures the data from the type and stores it in the <paramref name="record"/>.
    /// </summary>
    void CaptureData(IDataTagRecord record);

    /// <summary>
    /// Restore the values in the <see cref="IDataTaggedSettings"/> type from the given
    /// <paramref name="record"/>.
    /// </summary>
    void RestoreFrom(IDataTagRecord record);
}