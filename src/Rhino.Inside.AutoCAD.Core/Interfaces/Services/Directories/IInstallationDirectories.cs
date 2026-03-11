namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Defines the directory paths and naming conventions used by the application to locate
/// resources and assembly files.
/// </summary>
public interface IInstallationDirectories
{
    /// <summary>
    /// Gets the absolute path to the root installation directory.
    /// </summary>
    /// <remarks>
    /// This is the base path from which
    /// <see cref="Resources"/> and
    /// <see cref="VersionedAssemblies"/> are derived.
    /// </remarks>
    string RootInstallationLocation { get; }

    /// <summary>
    /// Gets the path to the resources directory containing configuration files,
    /// images, and other static assets.
    /// </summary>
    /// <remarks>
    /// Located within  <see cref="RootInstallationLocation"/>.
    /// </remarks>
    string Resources { get; }

    /// <summary>
    /// Gets the path to the directory containing the application's compiled assemblies.
    /// </summary>
    /// <remarks>
    /// Located within <see cref="RootInstallationLocation"/>.
    /// Used by the assembly resolver to load dependencies at runtime.
    /// </remarks>
    string VersionedAssemblies { get; }

    /// <summary>
    /// Gets the user-friendly display name of the application (e.g., "Rhino Inside AutoCAD").
    /// </summary>
    /// <remarks>
    /// Used for window titles, dialogs, and user-facing messages.
    /// </remarks>
    /// <seealso cref="ProductName"/>
    string ApplicationName { get; }

    /// <summary>
    /// Gets the product identifier using dot notation (e.g., "Rhino.Inside.AutoCAD").
    /// </summary>
    /// <remarks>
    /// Used for file paths, folder names, and registry entries where spaces are not permitted.
    /// Must match the Product name in Directory.Build.props.
    /// </remarks>
    /// <seealso cref="ApplicationName"/>
    string ProductName { get; }
}