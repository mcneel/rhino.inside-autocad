using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Specifies versioning information for a component.  This attribute is used to track the
/// version history of a component, including when it was introduced, updated, and
/// deprecated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ComponentVersionAttribute : Attribute
{
    /// <summary>
    /// Gets the version history of the component.
    /// </summary>
    public IVersionHistory VersionHistory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentVersionAttribute"/> class
    /// with the version when the component was introduced.
    /// </summary>
    /// <param name="introduced">
    /// The version when the component was introduced.
    /// </param>
    internal ComponentVersionAttribute(string introduced) : this(introduced, null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentVersionAttribute"/> class
    /// with the version when the component was introduced and updated.
    /// </summary>
    /// <param name="introduced">
    /// The version when the component was introduced.
    /// </param>
    /// <param name="updated">
    /// The version when the component was updated, or <c>null</c> if not updated.
    /// </param>
    internal ComponentVersionAttribute(string introduced, string? updated) : this(introduced, updated, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentVersionAttribute"/> class
    /// with the version when the component was introduced, updated, and deprecated.
    /// </summary>
    /// <param name="introduced">
    /// The version when the component was introduced.
    /// </param>
    /// <param name="updated">
    /// The version when the component was updated, or <c>null</c> if not updated.
    /// </param>
    /// <param name="deprecated">
    /// The version when the component was deprecated, or <c>null</c> if not deprecated.
    /// </param>
    internal ComponentVersionAttribute(string introduced, string? updated, string? deprecated)
    {
        var introducedVersion = Version.Parse(introduced);
        var updatedVersion = updated is not null ? Version.Parse(updated!) : null;
        var deprecatedVersion = deprecated is not null ? Version.Parse(deprecated!) : null;

        this.VersionHistory =
            new VersionHistory(introducedVersion, updatedVersion, deprecatedVersion);
    }

    /// <summary>
    /// Gets the most recent version of the component for the specified type.
    /// </summary>
    /// <param name="type">
    /// The type of the component.
    /// </param>
    /// <returns>
    /// The most recent version of the component.
    /// </returns>
    internal static Version GetCurrentVersion(Type type)
    {
        var maxVersion = new Version();

        var assembly = typeof(ComponentVersionAttribute).Assembly;

        for (; type != null; type = type.BaseType)
        {
            if (type.Assembly != assembly) continue;

            var typeVersion = (ComponentVersionAttribute[])type.GetCustomAttributes(typeof(ComponentVersionAttribute), false);

            if (typeVersion.Length <= 0) continue;

            var updated = typeVersion[0].VersionHistory.GetMaxVersion();

            if (updated > maxVersion) maxVersion = updated;
        }

        return maxVersion;
    }

    /// <summary>
    /// Attempts to retrieve the version history for the specified type.
    /// </summary>
    /// <param name="type">
    /// The type of the component.
    /// </param>
    /// <param name="versionHistory">
    /// When this method returns, contains the version history if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the version history was found; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryGetVersionHistory(Type type, out IVersionHistory? versionHistory)
    {
        var versions = (ComponentVersionAttribute[])type.GetCustomAttributes(typeof(ComponentVersionAttribute), false);

        versionHistory = versions.Length == 1 ? versions[0]!.VersionHistory : null;

        return versionHistory != null;
    }
}

