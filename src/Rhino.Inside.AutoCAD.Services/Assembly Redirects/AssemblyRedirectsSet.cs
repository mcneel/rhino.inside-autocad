using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IAssemblyRedirectsSet"/>
public class AssemblyRedirectsSet : IAssemblyRedirectsSet
{
    private readonly List<string> _assemblyNames = [];

    /// <summary>
    /// Constructs a new <see cref="AssemblyRedirectsSet"/> from a string enumerable,
    /// containing the names of the assemblies to redirect.
    /// </summary>
    public AssemblyRedirectsSet(IEnumerable<string> assemblyNames)
    {
        _assemblyNames.AddRange(assemblyNames);
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => _assemblyNames.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}