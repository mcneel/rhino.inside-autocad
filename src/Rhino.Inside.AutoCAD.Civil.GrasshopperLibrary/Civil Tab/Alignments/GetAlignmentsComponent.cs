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
/// A Grasshopper component that gets all Alignments from the current Civil 3D document.
/// </summary>
[ComponentVersion(introduced: "1.1.19")]
public class GetAlignmentsComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("f4a11a81-3f5e-4a38-8612-0e35e08ad1d9");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAlignmentsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAlignmentsComponent"/> class.
    /// </summary>
    public GetAlignmentsComponent()
        : base("Get Civil3d Alignments", "CVL-GetAlmnts",
            "Gets all Alignments from the current Civil 3D document",
            "Civil3d", "Alignments")
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
        pManager.AddParameter(new Param_CivilAlignment(), "Alignments", "Almnts",
            "All Alignments in the current document.", GH_ParamAccess.list);
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

        var alignments = transactionManager.PerformTask(() =>
        {
            var result = new List<GH_CivilAlignment>();

            try
            {
                var civilDoc = CivilDocument.GetCivilDocument(document.Unwrap().Database);

                var alignmentIds = civilDoc.GetAlignmentIds();

                foreach (ObjectId alignmentId in alignmentIds)
                {
                    if (alignmentId.IsNull || alignmentId.IsErased)
                        continue;

                    var alignment = transactionManager.Unwrap()
                        .GetObject(alignmentId, OpenMode.ForRead) as Alignment;

                    if (alignment != null)
                    {
                        result.Add(new GH_CivilAlignment(alignment));
                    }
                }
            }
            catch
            {
                // Return empty list if extraction fails
            }

            return result;
        });

        if (alignments.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No alignments found in the document");
            return;
        }

        DA.SetDataList(0, alignments);
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
                if (changedObject.IsOfType<Alignment>())
                    return true;
            }
        }

        return false;
    }
}
