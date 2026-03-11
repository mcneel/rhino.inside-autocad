using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAssocNetwork"/>
public class AssocNetworkWrapper : AutocadDbObjectWrapper, IAssocNetwork
{
    private readonly AssocNetwork _assocNetwork;
    private readonly List<IObjectId> _actions;

    /// <inheritdoc/>
    public IReadOnlyList<IObjectId> Actions => _actions;

    /// <summary>
    /// Constructs a new <see cref="AssocNetworkWrapper"/>.
    /// </summary>
    public AssocNetworkWrapper(AssocNetwork assocNetwork) : base(assocNetwork)
    {
        _assocNetwork = assocNetwork;
        _actions = ExtractActions(assocNetwork);
    }

    /// <summary>
    /// Extracts the action ObjectIds from the AssocNetwork.
    /// </summary>
    private static List<IObjectId> ExtractActions(AssocNetwork assocNetwork)
    {
        var result = new List<IObjectId>();

        foreach (ObjectId actionId in assocNetwork.GetActions)
        {
            result.Add(new AutocadObjectIdWrapper(actionId));
        }

        return result;
    }

    /// <inheritdoc/>
    public new IAssocNetwork ShallowClone()
    {
        return new AssocNetworkWrapper(_assocNetwork);
    }
}