using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD DBObjects.
/// </summary>
public class Param_AutocadObject : GH_Param<GH_AutocadObject>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("8f3a2b1c-4d5e-6f7a-8b9c-0d1e2f3a4b5c");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadObject"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadObject(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadObject"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadObject(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadObject"/> class.
    /// </summary>
    public Param_AutocadObject(GH_ParamAccess access)
        : base("AutoCAD Object", "Obj",
            "An AutoCAD DBObject", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var autocadObj in m_data.AllData(true).OfType<GH_AutocadObject>())
        {
            if (change.DoesEffectObject(autocadObj.Reference.ObjectId)) return true;
        }

        return false;
    }
}
