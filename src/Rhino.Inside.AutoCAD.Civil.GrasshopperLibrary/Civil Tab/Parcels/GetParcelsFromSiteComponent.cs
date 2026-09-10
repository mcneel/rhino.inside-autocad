using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns all Parcels from a Civil 3D Site.
/// </summary>
[ComponentVersion(introduced: "1.1.19")]
public class GetParcelsFromSiteComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("E1F2A3B4-C5D6-7890-1234-123456789012");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetParcelsFromSiteComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetParcelsFromSiteComponent"/> class.
    /// </summary>
    public GetParcelsFromSiteComponent()
        : base("Get Parcels from Site", "CVL-SiteParcels",
            "Returns all Parcels from a Civil 3D Site",
            "Civil3d", "Site/Parcels")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilSite(GH_ParamAccess.item), "Site",
            "Site", "A Civil3d Site to get parcels from", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilParcel(), "Parcels", "P",
            "The Parcels in the Site.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_CivilSite? siteGoo = null;

        if (!DA.GetData(0, ref siteGoo) || siteGoo?.Value is null) return;

        var site = siteGoo.Value;

        var document = this.GetDocumentForObjectId(site.Id);
        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
            return;
        }

        var transactionManager = document.CreateTransactionManager();

        var parcels = transactionManager.PerformTask(() =>
        {
            var parcelList = new List<GH_CivilParcel>();

            foreach (var parcelId in site.ParcelIds)
            {
                try
                {
                    var parcel = transactionManager.Unwrap()
                        .GetObject(parcelId.Unwrap(), OpenMode.ForRead) as Parcel;

                    if (parcel != null)
                    {
                        parcelList.Add(new GH_CivilParcel(parcel));
                    }
                }
                catch
                {
                    // Skip parcels that can't be read
                }
            }

            return parcelList;
        });

        DA.SetDataList(0, parcels);
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change, bool includeModified = true)
    {
        foreach (var ghParam in this.Params.Output.OfType<IReferenceParam>())
        {
            if (ghParam.NeedsToBeExpired(change, includeModified)) return true;
        }

        foreach (var changedObject in change)
        {
            if (changedObject.IsOfType<Parcel>())
            {
                return true;
            }
        }

        return false;
    }
}
