namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A single Rhino installation discovered on this machine which this build is able to host.
/// </summary>
/// <remarks>
/// Deliberately free of any RhinoCommon types. Instances are created before the RhinoCommon
/// assembly resolvers are registered, so touching a Rhino type here would fail to load.
/// </remarks>
/// <seealso cref="IRhinoInstallationLocator"/>
public interface IRhinoInstallation
{
    /// <summary>
    /// The registry key name this version registered itself under, for example "8.0" or
    /// "9.0". Used as the stable identifier when persisting the user's choice.
    /// </summary>
    /// <seealso cref="IUserSettings.PreferredRhinoVersion"/>
    string VersionKey { get; }

    /// <summary>
    /// The major version number, for example 8 or 9.
    /// </summary>
    int MajorVersion { get; }

    /// <summary>
    /// The name shown to the user, for example "Rhino 9".
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// The Rhino system directory containing the executable and its assemblies.
    /// </summary>
    string SystemDirectory { get; }

    /// <summary>
    /// The default plug-ins directory containing Grasshopper.
    /// </summary>
    string PluginsDirectory { get; }

    /// <summary>
    /// The full path of the RhinoCommon assembly to bind to, already resolved for the
    /// runtime this build targets.
    /// </summary>
    /// <remarks>
    /// The NET8 build binds to the copy in the <c>netcore</c> subfolder of
    /// <see cref="SystemDirectory"/>; the NET48 build binds to the copy in its root.
    /// </remarks>
    string RhinoCommonPath { get; }

    /// <summary>
    /// The directory holding the assemblies to bind to, being the directory part of
    /// <see cref="RhinoCommonPath"/>.
    /// </summary>
    /// <remarks>
    /// Rhino.UI and Mono.Cecil sit alongside RhinoCommon, so they are resolved from here
    /// rather than from <see cref="SystemDirectory"/>.
    /// </remarks>
    string AssemblyDirectory { get; }

    /// <summary>
    /// The file version of the assembly at <see cref="RhinoCommonPath"/>, for example
    /// 8.32.26160.13001, or 0.0 when it could not be read.
    /// </summary>
    /// <remarks>
    /// Read from the file rather than the loaded assembly, so it is known before anything
    /// binds to the installation.
    /// </remarks>
    Version RhinoCommonVersion { get; }

    /// <summary>
    /// True when <see cref="RhinoCommonVersion"/> is older than the minimum this build
    /// supports. An outdated installation is never offered or bound; the user is asked to
    /// update Rhino instead.
    /// </summary>
    bool IsOutdated { get; }
}
