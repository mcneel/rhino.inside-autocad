using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IRhinoInsideAutoCadApplicationSettings"/>
public class ApplicationSettings : IRhinoInsideAutoCadApplicationSettings
{
    /// <inheritdoc/>
    public IFileNameLibrary FileNameLibrary { get; set; }

    /// <inheritdoc/>
    public IJsonNameLibrary JsonNameLibrary { get; set; }
}