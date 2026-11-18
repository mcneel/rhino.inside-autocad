using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that gets all Gradings from the model space of the current Civil 3D document.
/// </summary>
/// <remarks>
/// The Civil 3D .NET API has no grading group collection, so gradings are found by scanning
/// model space for Grading entities.
/// </remarks>
[ComponentVersion(introduced: "1.3.10")]
public class GetGradingsComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("9A4D6B2C-7E1F-4385-A0C9-5B8E2D1F3A64");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetGradingsComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetGradingsComponent"/> class.
    /// </summary>
    public GetGradingsComponent()
        : base("Get Civil3d Gradings", "CVL-GetGrds",
            "Gets all Gradings in model space of the current Civil 3D document. Grading groups are not available through the Civil 3D .NET API; use the TIN Surface components for a grading group's surface.",
            "Civil3d", "Grading")
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
        pManager.AddParameter(new Param_CivilGrading(), "Gradings", "Grds",
            "All Gradings in model space.", GH_ParamAccess.list);
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

        var gradings = transactionManager.PerformTask(() =>
        {
            var result = new List<GH_CivilGrading>();

            try
            {
                var transaction = transactionManager.Unwrap();
                var database = document.Unwrap().Database;
                var gradingClass = RXObject.GetClass(typeof(Grading));

                var modelSpace = (BlockTableRecord)transaction.GetObject(
                    SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForRead);

                foreach (ObjectId entityId in modelSpace)
                {
                    if (entityId.IsErased || !entityId.ObjectClass.IsDerivedFrom(gradingClass))
                        continue;

                    if (transaction.GetObject(entityId, OpenMode.ForRead) is Grading grading)
                    {
                        result.Add(new GH_CivilGrading(grading));
                    }
                }
            }
            catch
            {
                // Return empty list if extraction fails
            }

            return result;
        });

        if (gradings.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No gradings found in model space");
            return;
        }

        DA.SetDataList(0, gradings);
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
                if (changedObject.IsOfType<Grading>())
                    return true;
            }
        }

        return false;
    }
}
