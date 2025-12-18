namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the version history of an Autocad Grasshopper component. It provides
/// information about the version it was introduced,  whether it has been updated or
/// deprecated, and methods to retrieve version details.
/// </summary>
public interface IVersionHistory
{
    /// <summary>
    /// Gets the version in which the component or feature was introduced.
    /// </summary>
    Version Introduced { get; }

    /// <summary>
    /// Gets a value indicating whether the component or feature has an updated version
    /// available.
    /// </summary>
    bool HasUpdatedVersion { get; }

    /// <summary>
    /// Gets a value indicating whether the component or feature is deprecated.
    /// </summary>
    bool IsDeprecated { get; }

    /// <summary>
    /// Attempts to retrieve the updated version of the component or feature, if available.
    /// </summary>
    /// <param name="version">
    /// When this method returns, contains the updated version, if
    /// available; otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if an updated version is available; otherwise, <c>false</c>.
    /// </returns>
    bool TryGetUpdatedVersion(out Version? version);

    /// <summary>
    /// Attempts to retrieve the deprecated version of the component or feature, if applicable.
    /// </summary>
    /// <param name="version">
    /// When this method returns, contains the deprecated version, if applicable;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if a deprecated version is available; otherwise, <c>false</c>.
    /// </returns>
    bool TryGetDepreciatedVersion(out Version? version);

    /// <summary>
    /// Gets the maximum version supported by the component or feature.
    /// </summary>
    /// <returns>The maximum version supported.</returns>
    Version GetMaxVersion();
}