using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ILoadingScreenConstants"/>
public class LoadingScreenConstants : ILoadingScreenConstants
{
    /// <inheritdoc />
    public string? Copyright { get; set; }

    /// <inheritdoc />
    public string? VersionPrefix { get; set; }

    /// <inheritdoc />
    public string? RhinoVersionPrefix { get; set; }

    /// <inheritdoc />
    public string? FailedServiceMessage { get; set; }
}