using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IApplicationSettings"/>
public class ApplicationSettings : IApplicationSettings
{
    /// <inheritdoc/>
    public IFileNameLibrary FileNameLibrary { get; set; }

    /// <inheritdoc/>
    public IJsonNameLibrary JsonNameLibrary { get; set; }
}