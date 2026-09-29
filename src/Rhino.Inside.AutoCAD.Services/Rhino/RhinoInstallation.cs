using Microsoft.Win32;
using System.Diagnostics;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IRhinoInstallation"/>
/// <remarks>
/// Reads itself from the registry key the Rhino installer wrote, so everything known about
/// an install - how its version key is parsed, where its assemblies live, and whether this
/// build can host it - is decided here rather than spread across the locator.
/// </remarks>
/// <seealso cref="RhinoInstallationLocator"/>
public class RhinoInstallation : IRhinoInstallation
{
    private const string _rhinoDisplayNameFormat = ApplicationConstants.RhinoDisplayNameFormat;
    private const string _rhinoInstallPathValueName = ApplicationConstants.RhinoInstallPathValueName;
    private const string _rhinoPluginsFolderValueName = ApplicationConstants.RhinoPluginsFolderValueName;
    private const string _rhinoCommonDllName = ApplicationConstants.RhinoCommonDllName;
    private const string _rhinoNetCoreFolderName = ApplicationConstants.RhinoNetCoreFolderName;

    private static readonly IReadOnlyList<int> _supportedMajorVersions =
        ApplicationConstants.SupportedRhinoMajorVersions;

    private static readonly Version _minimumRhinoCommonVersion =
        ApplicationConstants.MinimumRhinoCommonVersion;

    private static readonly Version _unknownVersion = new(0, 0);

    /// <inheritdoc/>
    public string VersionKey { get; }

    /// <inheritdoc/>
    public int MajorVersion { get; }

    /// <inheritdoc/>
    public string DisplayName { get; }

    /// <inheritdoc/>
    public string SystemDirectory { get; }

    /// <inheritdoc/>
    public string PluginsDirectory { get; }

    /// <inheritdoc/>
    public string RhinoCommonPath { get; }

    /// <inheritdoc/>
    public string AssemblyDirectory { get; }

    /// <inheritdoc/>
    public Version RhinoCommonVersion { get; }

    /// <inheritdoc/>
    public bool IsOutdated => this.RhinoCommonVersion < _minimumRhinoCommonVersion;

    /// <summary>
    /// True when this build can host the installation.
    /// </summary>
    /// <remarks>
    /// Requires all of: a version key naming a supported major version, both installation
    /// directories present in the registry, and the RhinoCommon assembly this build's
    /// runtime needs actually on disk. The last condition rejects a broken install and an
    /// install with no build for this runtime alike.
    /// </remarks>
    /// <seealso cref="ApplicationConstants.SupportedRhinoMajorVersions"/>
    public bool IsHostable { get; }

    /// <summary>
    /// Constructs a new <see cref="RhinoInstallation"/> from the registry key a Rhino
    /// installer wrote.
    /// </summary>
    /// <remarks>
    /// Never throws for an install this build cannot use; inspect <see cref="IsHostable"/>
    /// instead. Being unable to host a version is an ordinary outcome of enumerating the
    /// registry, not an error.
    /// </remarks>
    /// <param name="versionKey">
    /// The registry key name the version registered under, for example "8.0" or "9.0-WIP".
    /// </param>
    /// <param name="installKey">
    /// The <c>Install</c> subkey beneath that version key, holding the paths.
    /// </param>
    public RhinoInstallation(string versionKey, RegistryKey installKey)
    {
        this.VersionKey = versionKey;
        this.MajorVersion = this.ParseMajorVersion(versionKey);
        this.DisplayName = this.BuildDisplayName(versionKey, this.MajorVersion);

        this.SystemDirectory = installKey.GetValue(_rhinoInstallPathValueName) as string ??
                               string.Empty;

        this.PluginsDirectory = installKey.GetValue(_rhinoPluginsFolderValueName) as string ??
                                string.Empty;

        this.RhinoCommonPath = this.BuildRhinoCommonPath(this.SystemDirectory);

        this.AssemblyDirectory = Path.GetDirectoryName(this.RhinoCommonPath) ??
                                 this.SystemDirectory;

        this.RhinoCommonVersion = this.ReadRhinoCommonVersion(this.RhinoCommonPath);

        this.IsHostable = _supportedMajorVersions.Contains(this.MajorVersion) &&
                          !string.IsNullOrWhiteSpace(this.SystemDirectory) &&
                          !string.IsNullOrWhiteSpace(this.PluginsDirectory) &&
                          File.Exists(this.RhinoCommonPath);
    }

    /// <summary>
    /// Parses the major version from the registry version key, tolerating the suffixes used
    /// by pre-release installs such as "9.0-WIP".
    /// </summary>
    /// <param name="versionKey">The registry key name the version registered under.</param>
    /// <returns>The major version, or zero when the key does not start with one.</returns>
    private int ParseMajorVersion(string versionKey)
    {
        if (string.IsNullOrWhiteSpace(versionKey))
            return 0;

        var digitCount = 0;

        while (digitCount < versionKey.Length && char.IsDigit(versionKey[digitCount]))
            digitCount++;

        if (digitCount == 0)
            return 0;

        return int.TryParse(versionKey.Substring(0, digitCount), out var majorVersion)
            ? majorVersion
            : 0;
    }

    /// <summary>
    /// Returns the name shown to the user, qualifying it with the registry key name for
    /// pre-release installs so two installs of the same major version can be told apart.
    /// </summary>
    /// <remarks>
    /// A release registers under "&lt;major&gt;.0" and is named plainly, for example
    /// "Rhino 9". Anything else keeps its key, for example "Rhino 9 (9.0-WIP)".
    /// </remarks>
    /// <param name="versionKey">The registry key name the version registered under.</param>
    /// <param name="majorVersion">The major version number.</param>
    private string BuildDisplayName(string versionKey, int majorVersion)
    {
        var displayName = string.Format(_rhinoDisplayNameFormat, majorVersion);

        return versionKey == $"{majorVersion}.0"
            ? displayName
            : $"{displayName} ({versionKey})";
    }

    /// <summary>
    /// Returns the full path of the RhinoCommon assembly this build must bind to.
    /// </summary>
    /// <remarks>
    /// Rhino ships its .NET Core assemblies in a subfolder of the system directory and its
    /// .NET Framework assemblies in its root.
    /// </remarks>
    /// <param name="systemDirectory">The Rhino system directory.</param>
    private string BuildRhinoCommonPath(string systemDirectory)
    {
#if NET8_0_OR_GREATER
        return Path.Combine(systemDirectory, _rhinoNetCoreFolderName, _rhinoCommonDllName);
#else
        return Path.Combine(systemDirectory, _rhinoCommonDllName);
#endif
    }

    /// <summary>
    /// Reads the file version of the RhinoCommon assembly without loading it.
    /// </summary>
    /// <remarks>
    /// The file version carries the service release, for example 8.32.26160.13001, which
    /// the major version in the registry key does not. It is read from the file rather than
    /// the assembly so nothing is loaded before the assembly resolvers are registered.
    /// </remarks>
    /// <param name="rhinoCommonPath">The full path of the RhinoCommon assembly.</param>
    /// <returns>The file version, or 0.0 when the file is missing or unreadable.</returns>
    private Version ReadRhinoCommonVersion(string rhinoCommonPath)
    {
        try
        {
            if (!File.Exists(rhinoCommonPath))
                return _unknownVersion;

            var versionInfo = FileVersionInfo.GetVersionInfo(rhinoCommonPath);

            return new Version(versionInfo.FileMajorPart, versionInfo.FileMinorPart,
                versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
        }
        catch (Exception e)
        {
            LoggerService.Instance.LogError(e);

            return _unknownVersion;
        }
    }

    /// <summary>
    /// Returns the name shown to the user.
    /// </summary>
    public override string ToString() => this.DisplayName;
}
