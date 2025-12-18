using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Diagnostics;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <inheritdoc cref="IVersionHistory"/>
public class VersionHistory : IVersionHistory
{
    /// <summary>
    /// The current updated version of the component, if any.
    /// </summary>
    private readonly Version? _updatedVersion;

    /// <summary>
    /// The version when the component was deprecated, if any.
    /// </summary>
    private readonly Version? _deprecatedVersion;

    /// <inheritdoc />
    public Version Introduced { get; }

    /// <inheritdoc />
    public bool HasUpdatedVersion { get; }

    /// <inheritdoc />
    public bool IsDeprecated { get; }

    /// <summary>
    /// Constructs a new <see cref="IVersionHistory"/>.
    /// </summary>
    public VersionHistory(Version introduced, Version? updatedVersion, Version? deprecatedVersion)
    {
        _updatedVersion = updatedVersion;

        _deprecatedVersion = deprecatedVersion;

        this.HasUpdatedVersion = _updatedVersion is not null;

        this.IsDeprecated = _deprecatedVersion is not null;

        this.Introduced = introduced;

#if DEBUG
        var updatedAssert = updatedVersion ?? introduced;
        var deprecatedAssert = deprecatedVersion ?? updatedAssert;

        Debug.Assert(updatedAssert >= introduced);
        Debug.Assert(deprecatedAssert >= updatedAssert);
#endif
    }

    /// <inheritdoc />
    public bool TryGetUpdatedVersion(out Version? version)
    {
        version = _updatedVersion;
        return this.HasUpdatedVersion;
    }

    /// <inheritdoc />
    public bool TryGetDepreciatedVersion(out Version? version)
    {
        version = _deprecatedVersion;
        return this.IsDeprecated;
    }

    /// <inheritdoc />
    public Version GetMaxVersion()
    {
        return this.HasUpdatedVersion ? _updatedVersion! : this.Introduced;
    }
}