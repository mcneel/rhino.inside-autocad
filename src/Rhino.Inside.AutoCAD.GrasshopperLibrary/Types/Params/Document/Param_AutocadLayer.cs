using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper parameter for AutoCAD curves.
/// </summary>
public class Param_AutocadLayer : GH_Param<GH_AutocadLayer>, IReferenceParam
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("f7be9005-8755-42b9-93fc-ace2e19e736f");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_AutocadLayer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLayer"/> class with the
    /// specified instance description.
    /// </summary>
    public Param_AutocadLayer(IGH_InstanceDescription tag) : base(tag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadLayer"/> class with the
    /// specified instance description and parameter access type.
    /// </summary>
    public Param_AutocadLayer(IGH_InstanceDescription tag, GH_ParamAccess access)
        : base(tag, access)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Param_AutocadCurve"/> class.
    /// </summary>
    public Param_AutocadLayer(GH_ParamAccess access)
        : base("AutoCAD Layer", "Layer",
            "A Layer in AutoCAD", "Params", "AutoCAD", access)
    { }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {

        foreach (var autocadId in m_data.AllData(true).OfType<GH_AutocadLayer>())
        {
            if (change.DoesAffectObject(autocadId.Value.Id))
                return true;
        }

        return false;
    }
}