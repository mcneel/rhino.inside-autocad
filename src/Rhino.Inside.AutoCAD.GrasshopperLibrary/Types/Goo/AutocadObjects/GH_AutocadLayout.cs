using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD layouts.
/// </summary>
public class GH_AutocadLayout : GH_AutocadObjectGoo<AutocadLayoutWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayout"/> class with no value.
    /// </summary>
    public GH_AutocadLayout()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayout"/> class with the
    /// specified AutoCAD layout.
    /// </summary>
    /// <param name="layoutWrapper">The AutoCAD layout to wrap.</param>
    public GH_AutocadLayout(AutocadLayoutWrapper layoutWrapper) : base(layoutWrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLayout"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadLayout(GH_AutocadLayout other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IAutocadLayout"/> via the interface.
    /// </summary>
    public GH_AutocadLayout(IAutocadLayout autocadLayout)
        : base((autocadLayout as AutocadLayoutWrapper)!)
    {

    }

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(Layout);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new AutocadLayoutWrapper(unwrapped as Layout);

        return new GH_AutocadLayout(newWrapper);
    }
}