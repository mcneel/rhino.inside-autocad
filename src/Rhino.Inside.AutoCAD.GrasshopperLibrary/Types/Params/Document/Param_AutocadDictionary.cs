using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD dictionaries.
/// </summary>
public class Param_AutocadDictionary : GH_Param<GH_AutocadDictionary>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("a8c3e1f7-5d92-4b6a-9e84-7c1f3b2d5a98");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDictionary"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadDictionary(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDictionary"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadDictionary(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadDictionary"/> class.
    /// </summary>
    public Param_AutocadDictionary(GH_ParamAccess access)
        : base("AutoCAD Dictionary", "Dict",
            "A Dictionary in AutoCAD", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var dict in m_data.AllData(true).OfType<GH_AutocadDictionary>())
        {
            if (change.DoesEffectObject(dict.Value.Id))
                return true;
        }

        return false;
    }
}
