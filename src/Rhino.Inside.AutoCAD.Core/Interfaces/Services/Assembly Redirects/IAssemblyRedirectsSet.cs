namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A set of assembly redirects, defined by their assembly names. This
/// set is used to determine which assemblies should be redirected in the
/// <see cref="IAssemblyManager"/>.
/// </summary>
public interface IAssemblyRedirectsSet : IEnumerable<string>
{
}