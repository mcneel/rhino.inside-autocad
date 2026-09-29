namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Decides which of the installed Rhino versions Rhino.Inside binds to for this session.
/// </summary>
/// <remarks>
/// The choice is made once per process, on the startup path, before any RhinoCommon type is
/// touched - the assembly resolvers which serve Rhino's assemblies bake in the paths of the
/// chosen installation and cannot be re-pointed afterwards. Implementations therefore must
/// not reference any Rhino type themselves.
/// </remarks>
/// <seealso cref="IRhinoInstallation"/>
/// <seealso cref="IRhinoInstallationLocator"/>
public interface IRhinoVersionSelection
{
    /// <summary>
    /// The installations passed over by the last <see cref="Resolve"/> for being older than
    /// the minimum RhinoCommon version, ordered newest first.
    /// </summary>
    /// <seealso cref="IRhinoInstallation.IsOutdated"/>
    IReadOnlyList<IRhinoInstallation> OutdatedInstallations { get; }

    /// <summary>
    /// Determines the Rhino installation to bind this session to, asking the user when the
    /// machine has more than one and they have not already settled on a version.
    /// </summary>
    /// <remarks>
    /// Outdated installations are never returned or offered to the user.
    /// </remarks>
    /// <param name="resolution">
    /// Why the call returned what it did. Tells apart the reasons for a null return - nothing
    /// installed, only outdated versions installed, or the user cancelling - which need
    /// different messages.
    /// </param>
    /// <returns>
    /// The installation to bind to, or null when <paramref name="resolution"/> is anything
    /// other than <see cref="RhinoVersionResolution.Resolved"/>.
    /// </returns>
    IRhinoInstallation? Resolve(out RhinoVersionResolution resolution);
}
