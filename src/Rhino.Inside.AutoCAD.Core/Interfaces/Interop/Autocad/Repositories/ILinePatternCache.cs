namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a cache of <see cref="IAutocadLinePattern"/>s from the active
/// <see cref="IAutocadDocument"/>.
/// </summary>
public interface ILinePatternCache : IDisposable
{
    /// <summary>
    /// Returns the <see cref="IAutocadLinePattern"/> that matches the <paramref name="linePatternId"/>
    /// otherwise returns the default solid (CONTINUOUS) pattern.
    /// </summary>
    IAutocadLinePattern GetById(IObjectId linePatternId);
}