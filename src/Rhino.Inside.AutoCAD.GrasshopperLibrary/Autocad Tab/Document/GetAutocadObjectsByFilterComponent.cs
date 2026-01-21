using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that returns AutoCAD elements matching a selection filter.
/// </summary>
[ComponentVersion(introduced: "1.0.9")]
public class GetAutocadObjectsByFilterComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("D6E8F0A2-B4C6-4D8E-9F0A-2B4C6D8E0F1A");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadElementsByFilterComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadObjectsByFilterComponent"/> class.
    /// </summary>
    public GetAutocadObjectsByFilterComponent()
        : base("Get AutoCAD Objects By Filter", "AC-GetByFilter",
            "Returns AutoCAD elements matching a selection filter",
            "AutoCAD", "Document")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document. If not provided, the active document will be used.", GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddParameter(new Param_AutocadFilter(GH_ParamAccess.item), "Filter",
            "F", "The selection filter to use for querying elements.", GH_ParamAccess.item);

        pManager.AddIntegerParameter("Limit", "L",
            "Maximum number of objects to return. Use 0 for unlimited. The default is 100.", GH_ParamAccess.item, 100);
        pManager[2].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGenericParameter("Objects", "O", "The AutoCAD objects matching the filter",
            GH_ParamAccess.list);
        pManager.AddIntegerParameter("Count", "C", "The number of objects found",
            GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDocument? autocadDocument = null;
        DA.GetData(0, ref autocadDocument);

        if (autocadDocument is null)
        {
            var activeDoc = RhinoInsideAutoCadExtension.Application?.RhinoInsideManager?.AutoCadInstance?.ActiveDocument;
            if (activeDoc is null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
                return;
            }
            autocadDocument = activeDoc as AutocadDocument;
        }

        if (autocadDocument is null)
            return;

        GH_AutocadFilter? filterGoo = null;
        if (!DA.GetData(1, ref filterGoo) || filterGoo?.Value == null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A valid filter is required");
            return;
        }

        var filter = filterGoo.Value;

        var limit = 100;
        DA.GetData(2, ref limit);

        var returnAllFilterObjects = limit <= 0;

        var selectionFilter = filter.GetSelectionFilter().Unwrap();

        var document = autocadDocument.Unwrap();
        var promptResult = document.Editor.SelectAll(selectionFilter);

        if (promptResult.Status != PromptStatus.OK)
        {
            DA.SetDataList(0, new List<IGH_Goo>());
            DA.SetData(1, 0);
            return;
        }

        var selectionSet = promptResult.Value;

        if (returnAllFilterObjects)
        {
            limit = selectionSet.Count;
        }

        var count = Math.Min(selectionSet.Count, limit);

        var elements = autocadDocument.Transaction((transactionManager) =>
           {
               var result = new List<IGH_Goo>();
               var transaction = transactionManager.Unwrap();
               var processedCount = 0;

               foreach (SelectedObject selectedObject in selectionSet)
               {
                   if (selectedObject == null) continue;
                   if (limit > 0 && processedCount >= limit) break;

                   var entity = transaction.GetObject(selectedObject.ObjectId, OpenMode.ForRead) as CadEntity;
                   if (entity == null) continue;

                   var wrappedEntity = new AutocadEntityWrapper(entity);

                   var goo = GooTypeRegistry.Instance?.CreateGoo(wrappedEntity);

                   if (goo != null)
                   {
                       result.Add(goo);
                   }
                   else
                   {
                       // Fall back to GH_AutocadObjectId for unrecognized types
                       var objectId = new AutocadObjectId(selectedObject.ObjectId);
                       result.Add(new GH_AutocadObjectId(objectId));
                   }

                   processedCount++;
               }
               return result;
           });

        DA.SetDataList(0, elements);
        DA.SetData(1, count);
    }

    /// <inheritdoc />
    public bool NeedsToBeExpired(IAutocadDocumentChange change)
    {
        foreach (var ghParam in this.Params.Output.OfType<IReferenceParam>())
        {
            if (ghParam.NeedsToBeExpired(change)) return true;
        }

        // Expire when entities are added, modified, or removed
        foreach (var changedObject in change)
        {
            if (changedObject.UnwrapObject() is CadEntity)
            {
                return true;
            }
        }

        return false;
    }
}
