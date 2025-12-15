using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns the AutoCAD documents currently open in the AutoCAD session.
/// </summary>
public class GetAutocadLayerByNameComponent : GH_Component, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("e74496d3-c465-4676-8584-c6f277bfbf0e");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadLayersComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadLayersComponent"/> class.
    /// </summary>
    public GetAutocadLayerByNameComponent()
        : base("Get AutoCAD Layer By Name", "AC-Lyr",
            "Returns the the AutoCAD layer which matches the name",
            "AutoCAD", "Layers")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);
        pManager.AddTextParameter("Name", "N", "The name of the AutoCAD Layer to retrieve", GH_ParamAccess.item, string.Empty);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLayer(GH_ParamAccess.item), "Layer", "Layer",
            "The AutoCAD Layer matching the name, or the default layer if no matching layer is found",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        var name = string.Empty;

        if (!DA.GetData(0, ref autocadDocument)
            || autocadDocument is null) return;
        DA.GetData(1, ref name);

        var layersRepository = autocadDocument.LayerRepository;

        if (layersRepository.TryGetByName(name, out var layer) == false)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                $"No layer exists with name: {name}");
            return;
        }

        var gooLayer = new GH_AutocadLayer(layer);

        DA.SetData(0, gooLayer);
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var ghParam in this.Params.Output.OfType<IReferenceParam>())
        {
            if (ghParam.NeedsToBeExpired(change)) return true;
        }

        foreach (var changedObject in change)
        {
            if (changedObject.UnwrapObject() is CadLayer)
            {
                return true;
            }
        }

        return false;

    }
}
