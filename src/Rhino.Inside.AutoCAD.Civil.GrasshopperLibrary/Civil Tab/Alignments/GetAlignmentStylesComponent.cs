using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Civil.Interop;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;
using CivilDocument = Autodesk.Civil.ApplicationServices.CivilDocument;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that gets all Alignment Styles from the current Civil 3D document.
/// </summary>
[ComponentVersion(introduced: "1.1.19")]
public class GetAlignmentStylesComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("8b7c6d5e-4f3a-2b1c-0d9e-8f7a6b5c4d3e");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Param_CivilAlignmentStyle;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAlignmentStylesComponent"/> class.
    /// </summary>
    public GetAlignmentStylesComponent()
        : base("Get Civil3d Alignment Styles", "CVL-GetAlnStyles",
            "Gets all Alignment Styles from the current Civil 3D document",
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
        pManager.AddParameter(new Param_CivilAlignmentStyle(GH_ParamAccess.list), "Styles", "S",
            "All Alignment Styles in the document.", GH_ParamAccess.list);
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

        var styles = transactionManager.PerformTask(() =>
        {
            var result = new List<GH_CivilAlignmentStyle>();

            try
            {
                var civilDoc = CivilDocument.GetCivilDocument(document.Unwrap().Database);

                var alignmentStyles = civilDoc.Styles.AlignmentStyles;

                foreach (ObjectId styleId in alignmentStyles)
                {
                    if (styleId.IsNull || styleId.IsErased)
                        continue;

                    var style = transactionManager.Unwrap()
                        .GetObject(styleId, OpenMode.ForRead) as AlignmentStyle;

                    if (style != null)
                    {
                        result.Add(new GH_CivilAlignmentStyle(new CivilAlignmentStyleWrapper(style)));
                    }
                }
            }
            catch
            {
                // Return empty list if extraction fails
            }

            return result;
        });

        if (styles.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No alignment styles found in the document");
            return;
        }

        DA.SetDataList(0, styles);
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
                if (changedObject.IsOfType<AlignmentStyle>())
                    return true;
            }
        }

        return false;
    }
}
