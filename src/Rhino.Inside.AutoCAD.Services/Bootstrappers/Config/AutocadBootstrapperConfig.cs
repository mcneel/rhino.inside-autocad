using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A configuration for the <see cref="Bootstrapper"/> specific to AutoCAD.
/// <inheritdoc cref="IBootstrapperConfig"/>.
/// </summary>
public class AutocadBootstrapperConfig : IBootstrapperConfig
{
    /// <inheritdoc />
    public IWindowConfig WindowConfig { get; }

    /// <inheritdoc />
    public IAssemblyRedirectsSet AssemblyRedirectsSet { get; }

    /// <inheritdoc />
    public IApplicationConfig ApplicationConfig { get; }

    /// <summary>
    /// The constructor for the <see cref="AutocadBootstrapperConfig"/>. A config for the <see
    /// cref="IBootstrapper"/> in AutoCAD applications.
    /// </summary>
    public AutocadBootstrapperConfig(IntPtr parentWindowHandle, IApplicationConfig applicationConfig)
    {
        this.WindowConfig = new AutocadWindowConfig(parentWindowHandle);

        this.ApplicationConfig = applicationConfig;

        this.AssemblyRedirectsSet = new AssemblyRedirectsSet([
            "System.Runtime.CompilerServices.Unsafe",
            "Microsoft.Bcl.AsyncInterfaces",
            "System.Threading.Tasks.Extensions",
            "System.Buffers",
            "System.Numerics.Vectors",
            "System.Memory",
            "System.Collections",
            "PresentationFramework",
            "System.Diagnostics.DiagnosticSource",
            "Serilog"
        ]);
    }
}
