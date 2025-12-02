namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents plot settings in the active <see cref="IAutoCadDocument"/>.
/// </summary>
public interface IPlotSettings : IDisposable
{
    /// <summary>
    /// The <see cref="IObjectId"/> of the <see cref="IPlotSettings"/>.
    /// </summary>
    IObjectId Id { get; }

    /// <summary>
    /// The name of the <see cref="IPlotSettings"/>.
    /// </summary>
    string Name { get; }
}