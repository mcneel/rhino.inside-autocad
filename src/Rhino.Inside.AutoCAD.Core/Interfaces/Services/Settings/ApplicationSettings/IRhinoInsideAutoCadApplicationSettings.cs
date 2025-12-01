using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A <see cref="IApplicationServicesCore"/> for the application. This is an example
/// of how to extend the core settings for a specific application.
/// </summary>
public interface IRhinoInsideAutoCadApplicationSettings : IApplicationSettings
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