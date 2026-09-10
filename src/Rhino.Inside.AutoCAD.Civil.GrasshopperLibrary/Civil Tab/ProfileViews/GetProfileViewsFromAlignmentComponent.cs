using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Applications;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.GrasshopperLibrary;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.Civil.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that gets all ProfileViews from a Civil 3D Alignment.
/// </summary>
[ComponentVersion(introduced: "1.0.19")]
public class GetProfileViewsFromAlignmentComponent : RhinoInsideAutocad_ComponentBase, IReferenceComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("F6A7B8C9-D0E1-2345-F678-901234567890");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetProfileViewsFromAlignmentComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProfileViewsFromAlignmentComponent"/> class.
    /// </summary>
    public GetProfileViewsFromAlignmentComponent()
        : base("Get ProfileViews", "CVL-GetPV",
            "Gets all ProfileViews from a Civil 3D Alignment",
            "Civil3d", "ProfileViews")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilAlignment(), "Alignment",
            "Align", "A Civil3d Alignment to get ProfileViews from", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_CivilProfileView(), "ProfileViews", "PV",
            "The ProfileViews associated with this Alignment.", GH_ParamAccess.list);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        GH_CivilAlignment? alignmentGoo = null;

        if (!DA.GetData(0, ref alignmentGoo) || alignmentGoo is null) return;

        var alignmentId = alignmentGoo.Reference.ObjectId;

        var document = this.GetDocumentForObjectId(alignmentId);
        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
            return;
        }

        var transactionManager = document.CreateTransactionManager();

        var profileViews = transactionManager.PerformTask(() =>
        {
            var alignment = transactionManager.Unwrap()
                .GetObject(alignmentId.Unwrap(), OpenMode.ForRead) as Alignment;

            if (alignment == null)
                return new List<GH_CivilProfileView>();

            var profileViewIds = alignment.GetProfileViewIds();
            var result = new List<GH_CivilProfileView>();

            foreach (ObjectId profileViewId in profileViewIds)
            {
                if (profileViewId.IsNull || profileViewId.IsErased)
                    continue;

                var profileView = transactionManager.Unwrap()
                    .GetObject(profileViewId, OpenMode.ForRead) as ProfileView;

                if (profileView != null)
                {
                    result.Add(new GH_CivilProfileView(profileView));
                }
            }

            return result;
        });

        if (profileViews.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No ProfileViews found for this alignment");
            return;
        }

        DA.SetDataList(0, profileViews);
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
            if (changedObject.IsOfType<ProfileView>())
            {
                return true;
            }
        }

        return false;
    }
}
