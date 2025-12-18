using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Reflection;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A services class which is responsible for handling assembly redirects and
/// resolving assemblies which the app depends on that conflict with AutoCAD.
/// </summary>
public class AssemblyManager : IAssemblyManager
{
    private readonly IApplicationDirectories _applicationDirectories;

    private readonly AppDomain _currentDomain;

    private readonly IList<string> _materialDesignAssemblyNames = ApplicationConstants.MaterialDesignAssemblyNames;

    private readonly IAssemblyRedirectsSet _assemblyNameRedirects;

    private readonly Dictionary<string, Assembly> _resolvedAssemblies = [];

    private const string _errorLoadingMaterialDesign = MessageConstants.ErrorLoadingMaterialDesign;

    /// <summary>
    /// Constructs a new <see cref="AssemblyManager"/>.
    /// </summary>
    public AssemblyManager(IApplicationDirectories applicationDirectories,
        IAssemblyRedirectsSet assemblyRedirectsSet)
    {
        _applicationDirectories = applicationDirectories;

        _currentDomain = AppDomain.CurrentDomain;

        _currentDomain.AssemblyResolve += this.OnAssemblyResolve;

        _assemblyNameRedirects = assemblyRedirectsSet;

        this.LoadMaterialDesign(applicationDirectories);
    }

    /// <summary>
    /// The Material Design library has to be force loaded into Revit to avoid runtime
    /// exceptions as it's not automatically loaded as the calls to the library are always
    /// from XAML. This method guarantees its loaded.
    /// </summary>
    private void LoadMaterialDesign(IApplicationDirectories applicationDirectories)
    {
        try
        {
            foreach (var names in _materialDesignAssemblyNames)
            {
                var assemblyPath = Path.Combine(applicationDirectories.Assemblies, names);
                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);

                Assembly.Load(assemblyName);
            }
        }
        catch (Exception e)
        {
            LoggerService.Instance.LogError(e, _errorLoadingMaterialDesign);
        }
    }

    /// <summary>
    /// The event handler which fires when an unresolved assembly event occurs in the
    /// app domain. This is the main method used for resolving assemblies which are
    /// shipped with the MFE software but conflict with older versions shipped with
    /// AutoCAD.
    /// </summary>
    private Assembly? OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyFullName = args.Name;

        var matchingAssemblyName = _assemblyNameRedirects.FirstOrDefault(assemblyFullName.Contains);
        if (matchingAssemblyName != null)
        {
            if (_resolvedAssemblies.TryGetValue(matchingAssemblyName, out var resolve))
                return resolve;

            var assemblyPath = Path.Combine(_applicationDirectories.Assemblies, $"{matchingAssemblyName}.dll");
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var assembly = Assembly.Load(assemblyName);

            _resolvedAssemblies[matchingAssemblyName] = assembly;

            return assembly;
        }

        // Must return null so the fallback assembly resolution occurs.
        return null;
    }

    /// <summary>
    /// Shuts down this service.
    /// </summary>
    public void ShutDown()
    {
        _currentDomain.AssemblyResolve -= this.OnAssemblyResolve;
    }
}