using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD AssocNetworks.
/// </summary>
public class GH_AutocadAssocNetwork : GH_AutocadObjectGoo<AssocNetworkWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadAssocNetwork"/> class with no value.
    /// </summary>
    public GH_AutocadAssocNetwork()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadAssocNetwork"/> class with the
    /// specified AutoCAD AssocNetwork.
    /// </summary>
    /// <param name="assocNetworkWrapper">The AutoCAD AssocNetwork to wrap.</param>
    public GH_AutocadAssocNetwork(AssocNetworkWrapper assocNetworkWrapper) : base(assocNetworkWrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadAssocNetwork"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadAssocNetwork(GH_AutocadAssocNetwork other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="GH_AutocadAssocNetwork"/> via the interface.
    /// </summary>
    public GH_AutocadAssocNetwork(IAssocNetwork assocNetwork)
        : base((assocNetwork as AssocNetworkWrapper)!)
    {
    }

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(AssocNetwork);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new AssocNetworkWrapper(unwrapped as AssocNetwork);

        return new GH_AutocadAssocNetwork(newWrapper);
    }
}