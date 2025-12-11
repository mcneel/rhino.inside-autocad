using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoConvertibleSet"/>
public class RhinoConvertibleSet : IRhinoConvertibleSet
{
    private readonly List<IRhinoConvertible> _set
        = new List<IRhinoConvertible>();

    /// <inheritdoc/>
    public bool Any => _set.Count > 0;

    /// <inheritdoc/>
    public void Add(IRhinoConvertible convertible)
    {
        _set.Add(convertible);
    }

    /// <inheritdoc/>
    public IEnumerator<IRhinoConvertible> GetEnumerator() => _set.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}