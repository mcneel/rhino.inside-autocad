namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The application specific settings core, This core is used to define settings that
/// are specific to the host application. All application setting should inherit from
/// this.
/// </summary>
public interface IApplicationSettings
{
    /// <summary>
    /// The file name library specific to the host application.
    /// </summary>
    IFileNameLibrary FileNameLibrary { get; }

    /// <summary>
    /// The JSON name library specific to the host application.
    /// </summary>
    IJsonNameLibrary JsonNameLibrary { get; }
}