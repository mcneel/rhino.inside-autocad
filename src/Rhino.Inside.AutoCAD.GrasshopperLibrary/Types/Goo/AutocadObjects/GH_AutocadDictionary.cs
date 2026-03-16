using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD dictionaries.
/// </summary>
public class GH_AutocadDictionary : GH_AutocadObjectGoo<AutocadDictionaryWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDictionary"/> class with no value.
    /// </summary>
    public GH_AutocadDictionary()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDictionary"/> class with the
    /// specified AutoCAD dictionary.
    /// </summary>
    /// <param name="dictionaryWrapper">The AutoCAD dictionary to wrap.</param>
    public GH_AutocadDictionary(AutocadDictionaryWrapper dictionaryWrapper) : base(dictionaryWrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDictionary"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadDictionary(GH_AutocadDictionary other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="GH_AutocadDictionary"/> via the interface.
    /// </summary>
    public GH_AutocadDictionary(IAutocadDictionary autocadDictionary)
        : base((autocadDictionary as AutocadDictionaryWrapper)!)
    {
    }

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(DBDictionary);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new AutocadDictionaryWrapper(unwrapped as DBDictionary);

        return new GH_AutocadDictionary(newWrapper);
    }
}