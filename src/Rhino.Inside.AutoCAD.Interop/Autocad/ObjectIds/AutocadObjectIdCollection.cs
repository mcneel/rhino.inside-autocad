using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Collections;

namespace Rhino.Inside.AutoCAD.Interop;

///<inheritdoc cref="IObjectIdCollection"/>
public class AutocadObjectIdCollection : IObjectIdCollection
{
    private readonly List<IObjectId> _ids = new();
    /// <summary>
    /// Constructs a new instance of <see cref="IObjectIdCollection"/>.
    /// </summary>
    public AutocadObjectIdCollection() { }

    ///<inheritdoc/>
    public void Add(IObjectId objectId)
    {
        _ids.Add(objectId);
    }

    ///<inheritdoc/>
    public void Add(IObjectIdCollection objectIdCollection)
    {
        foreach (var objectId in objectIdCollection) this.Add(objectId);
    }

    ///<inheritdoc/>
    public IEnumerator<IObjectId> GetEnumerator() => _ids.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}