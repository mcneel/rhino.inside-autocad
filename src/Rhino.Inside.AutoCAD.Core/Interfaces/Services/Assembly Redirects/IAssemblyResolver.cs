namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Provides assembly resolution services for handling version conflicts between
/// AutoCAD's bundled assemblies and those required by Rhino.Inside.AutoCAD.
/// </summary>
/// <remarks>
/// Implementations subscribe to <see cref="AppDomain.AssemblyResolve"/> events to intercept
/// assembly loading requests and redirect them to compatible versions from the versioned
/// assemblies directory. This is necessary because AutoCAD bundles older versions of
/// common .NET libraries that conflict with Rhino's requirements.
/// </remarks>
/// <seealso cref="IAssemblyRedirectsSet"/>
/// <seealso cref="IInstallationDirectories"/>
public interface IAssemblyResolver
{
    /// <summary>
    /// Terminates the assembly resolver service by unsubscribing from
    /// <see cref="AppDomain.AssemblyResolve"/> events.
    /// </summary>
    /// <remarks>
    /// Called during application shutdown to properly clean up the event subscription.
    /// This should be invoked before other services are terminated to ensure assembly
    /// resolution remains available during the shutdown sequence.
    /// </remarks>
    void Terminate();
}