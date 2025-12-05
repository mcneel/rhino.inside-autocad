using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="INamedObjectsDictionary"/>
public class NamedObjectsDictionary : WrapperDisposableBase<DBDictionary>, INamedObjectsDictionary
{
    /// <summary>
    /// Constructs a new <see cref="NamedObjectsDictionary"/>.
    /// </summary>
    public NamedObjectsDictionary(DBDictionary dbDictionary) : base(dbDictionary)
    {

    }

    /// <inheritdoc/>
    public bool TryGetValue(string key, out IObjectId? value)
    {
        if (!_wrappedValue.Contains(key))
        {
            value = null;

            return false;
        }

        value = new AutocadObjectId(_wrappedValue.GetAt(key));

        return true;
    }

    /// <inheritdoc/>
    public void UpgradeOpen()
    {
        _wrappedValue.UpgradeOpen();
    }
}