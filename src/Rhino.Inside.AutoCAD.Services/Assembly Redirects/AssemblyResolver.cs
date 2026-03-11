using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A services class which is responsible for handling assembly redirects and
/// resolving assemblies which the app depends on that conflict with AutoCAD.
/// </summary>
public class AssemblyResolver : IAssemblyResolver
{
    private readonly IInstallationDirectories _installationDirectories;

    private readonly AppDomain _currentDomain;

    private readonly IList<string> _materialDesignAssemblyNames = ApplicationConstants.MaterialDesignAssemblyNames;

    private readonly IAssemblyRedirectsSet _assemblyNameRedirects;

    private readonly Dictionary<string, Assembly> _resolvedAssemblies = [];

    private const string _errorLoadingMaterialDesign = MessageConstants.ErrorLoadingMaterialDesign;

    /// <summary>
    /// Constructs a new <see cref="AssemblyResolver"/>.
    /// </summary>
    public AssemblyResolver(IInstallationDirectories installationDirectories,
        IAssemblyRedirectsSet assemblyRedirectsSet)
    {
        _installationDirectories = installationDirectories;

        _currentDomain = AppDomain.CurrentDomain;

        _currentDomain.AssemblyResolve += this.ResolveAssembly;

        _assemblyNameRedirects = assemblyRedirectsSet;

        this.LoadMaterialDesign(installationDirectories);
    }

    /// <summary>
    /// The Material Design library has to be force loaded into Revit to avoid runtime
    /// exceptions as it's not automatically loaded as the calls to the library are always
    /// from XAML. This method guarantees its loaded.
    /// </summary>
    private void LoadMaterialDesign(IInstallationDirectories installationDirectories)
    {
        try
        {
            foreach (var names in _materialDesignAssemblyNames)
            {
                var assemblyPath = Path.Combine(installationDirectories.VersionedAssemblies, names);

                Assembly.LoadFrom(assemblyPath);
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(_errorLoadingMaterialDesign, e);
        }
    }

    /// <summary>
    /// The event handler which fires when an unresolved assembly event occurs in the
    /// app domain. This is the main method used for resolving assemblies which are
    /// shipped with the Rhino Iniside Autocad software but conflict with older versions
    /// shipped with AutoCAD. If no match this method must return null so the fallback
    /// assembly resolution occurs.
    /// </summary>
    private Assembly? ResolveAssembly(object sender, ResolveEventArgs args)
    {
        var assemblyFullName = args.Name;

        var matchingAssemblyName = _assemblyNameRedirects.FirstOrDefault(assemblyFullName.Contains);
        if (matchingAssemblyName != null)
        {
            if (_resolvedAssemblies.TryGetValue(matchingAssemblyName, out var resolve))
                return resolve;

            var assemblyPath = Path.Combine(_installationDirectories.VersionedAssemblies, $"{matchingAssemblyName}.dll");

            if (!File.Exists(assemblyPath))
                return null;

            var assembly = Assembly.LoadFrom(assemblyPath);

            _resolvedAssemblies[matchingAssemblyName] = assembly;

            return assembly;
        }

        return null;
    }

    /// <summary>
    /// Shuts down this service.
    /// </summary>
    public void Terminate()
    {
        _currentDomain.AssemblyResolve -= this.ResolveAssembly;
    }
}