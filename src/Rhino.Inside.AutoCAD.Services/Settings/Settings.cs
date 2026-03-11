using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ISettings"/>
public class Settings : ISettings
{
    /// <inheritdoc/>
    public ILoadingScreenConstants LoadingScreenConstants { get; set; }
}