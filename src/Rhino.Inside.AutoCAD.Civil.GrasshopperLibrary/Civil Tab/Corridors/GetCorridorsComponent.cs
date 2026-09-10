using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;
using CivilDocument = Autodesk.Civil.ApplicationServices.CivilDocument;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that gets all Corridors from the current Civil 3D document.
/// </summary>
[ComponentVersion(introduced: "1.1.19")]
public class GetCorridorsComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("C5D6E7F8-A9B0-1234-5678-901234567012");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetCorridorsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCorridorsComponent"/> class.
    /// </summary>
    public GetCorridorsComponent()
        : base("Get Civil3d Corridors", "CVL-GetCorrs",
            "Gets all Corridors from the current Civil 3D document",
            "Civil3d", "Corridors")
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
        pManager.AddParameter(new Param_CivilCorridor(), "Corridors", "Corrs",
            "All Corridors in the current document.", GH_ParamAccess.list);
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

        var corridors = transactionManager.PerformTask(() =>
        {
            var result = new List<GH_CivilCorridor>();

            try
            {
                var civilDoc = CivilDocument.GetCivilDocument(document.Unwrap().Database);

                var corridorIds = civilDoc.CorridorCollection;

                foreach (var corridorId in corridorIds)
                {
                    if (corridorId.IsNull || corridorId.IsErased)
                        continue;

                    var corridor = transactionManager.Unwrap()
                        .GetObject(corridorId, OpenMode.ForRead) as Corridor;

                    if (corridor != null)
                    {
                        result.Add(new GH_CivilCorridor(corridor));
                    }
                }
            }
            catch
            {
                // Return empty list if extraction fails
            }

            return result;
        });

        if (corridors.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No corridors found in the document");
            return;
        }

        DA.SetDataList(0, corridors);
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
                if (changedObject.IsOfType<Corridor>())
                    return true;
            }
        }

        return false;
    }
}
