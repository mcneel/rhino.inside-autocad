using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;
using CivilDocument = Autodesk.Civil.ApplicationServices.CivilDocument;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that gets standalone Feature Lines from the current Civil 3D document.
/// </summary>
[ComponentVersion(introduced: "1.3.12")]
public class GetFeatureLinesComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("4B8E1F2A-6C3D-4E5F-8A9B-0C1D2E3F4A5B");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetFeatureLinesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFeatureLinesComponent"/> class.
    /// </summary>
    public GetFeatureLinesComponent()
        : base("Get Civil3d Feature Lines", "CVL-GetFLs",
            "Gets standalone Feature Lines from the current Civil 3D document, from every site and the siteless collection, or from one site",
            "Civil3d", "Feature Lines")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddParameter(new Param_CivilSite(GH_ParamAccess.item), "Site", "Site",
            "Only return feature lines in this site. If not provided, feature lines from every site and siteless feature lines are returned.",
            GH_ParamAccess.item);
        pManager[1].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilFeatureLine(), "Feature Lines", "FLs",
            "The Feature Lines found.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        GH_CivilSite? siteGoo = null;

        DA.GetData(0, ref autocadDocument);
        DA.GetData(1, ref siteGoo);

        var document = this.GetDocumentOrDefault(autocadDocument);

        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
            return;
        }

        var siteId = siteGoo?.Value?.Id.Unwrap() ?? ObjectId.Null;

        var transactionManager = document.CreateTransactionManager();

        var featureLines = transactionManager.PerformTask(() =>
        {
            var result = new List<GH_CivilFeatureLine>();

            try
            {
                var transaction = transactionManager.Unwrap();
                var civilDoc = CivilDocument.GetCivilDocument(document.Unwrap().Database);

                var featureLineIds = siteId.IsNull
                    ? CivilFeatureLineCollector.GetAllFeatureLineIds(civilDoc, transaction)
                    : CivilFeatureLineCollector.GetSiteFeatureLineIds(siteId, transaction);

                foreach (var featureLineId in featureLineIds)
                {
                    if (transaction.GetObject(featureLineId, OpenMode.ForRead) is FeatureLine featureLine)
                    {
                        result.Add(new GH_CivilFeatureLine(featureLine));
                    }
                }
            }
            catch
            {
                // Return empty list if extraction fails
            }

            return result;
        });

        if (featureLines.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No feature lines found");
            return;
        }

        DA.SetDataList(0, featureLines);
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change, bool includeModified = true)
    {
        // Only expire if objects are created or erased (list changes)
        foreach (var ghParam in this.Params.Output.OfType<IReferenceParam>())
        {
            if (ghParam.NeedsToBeExpired(change, includeModified: false)) return true;
        }

        // Check for type created/erased only
        if (change.Contains(ChangeType.ObjectCreated) || change.Contains(ChangeType.ObjectErased))
        {
            foreach (var changedObject in change)
            {
                if (changedObject.IsOfType<FeatureLine>())
                    return true;
            }
        }

        return false;
    }
}
