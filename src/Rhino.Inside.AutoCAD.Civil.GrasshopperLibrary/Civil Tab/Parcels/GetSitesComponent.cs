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
/// A Grasshopper component that returns all Civil 3D Sites in the document.
/// </summary>
[ComponentVersion(introduced: "1.1.19")]
public class GetSitesComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("D0E1F2A3-B4C5-6789-0123-012345678901");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetSitesComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSitesComponent"/> class.
    /// </summary>
    public GetSitesComponent()
        : base("Get Civil3d Sites", "CVL-Sites",
            "Returns the list of all Civil 3D Sites in the document",
            "Civil3d", "Site/Parcels")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilSite(GH_ParamAccess.list), "Sites", "Sites",
            "The Civil 3D Sites in the document.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        DA.GetData(0, ref autocadDocument);

        var document = this.GetDocumentOrDefault(autocadDocument);

        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
            return;
        }

        var transactionManager = document.CreateTransactionManager();

        var sites = transactionManager.PerformTask(() =>
        {
            var siteList = new List<GH_CivilSite>();

            try
            {
                var database = document.AutocadDatabase.Unwrap();
                var civilDoc = CivilDocument.GetCivilDocument(database);

                if (civilDoc == null)
                {
                    return siteList;
                }

                var siteIds = civilDoc.GetSiteIds();

                foreach (ObjectId siteId in siteIds)
                {
                    if (siteId.IsNull || siteId.IsErased)
                        continue;

                    try
                    {
                        var site = transactionManager.Unwrap()
                            .GetObject(siteId, OpenMode.ForRead) as Site;

                        if (site != null)
                        {
                            var wrapper = new CivilSiteWrapper(site);
                            siteList.Add(new GH_CivilSite(wrapper));
                        }
                    }
                    catch
                    {
                        // Skip sites that can't be read
                    }
                }
            }
            catch
            {
                // Return empty list if site enumeration fails
            }

            return siteList;
        });

        DA.SetDataList(0, sites);
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
                if (changedObject.IsOfType<Site>())
                    return true;
            }
        }

        return false;
    }
}
