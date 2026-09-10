using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that sets properties for an AutoCAD linetype.
/// </summary>
[ComponentVersion(introduced: "1.0.0", updated: "1.3.2")]
public class SetAutocadLineTypeComponent : RhinoInsideAutocad_ComponentBase
{
    private const double _zeroTolerance = GeometryConstants.ZeroTolerance;

    /// <inheritdoc />
    public override Guid ComponentGuid => new("b3e7d9f1-5c8a-4b6d-9e2f-7a4c3d6e8b1f");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SetAutocadLineTypeComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetAutocadLineTypeComponent"/> class.
    /// </summary>
    public SetAutocadLineTypeComponent()
        : base("Set AutoCAD Line Type", "AC-SetLT",
            "Sets properties for an AutoCAD LineType",
            "AutoCAD", "LineTypes")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLineType(GH_ParamAccess.item), "LineType",
            "LineType", "An AutoCAD LineType", GH_ParamAccess.item);

        pManager.AddTextParameter("NewName", "Name",
            "The name of the AutoCAD Layer.", GH_ParamAccess.item);

        pManager.AddNumberParameter("NewPatternLength", "Length",
            "New pattern length", GH_ParamAccess.item);

        pManager.AddIntegerParameter("NewNumberOfDashes", "Dashes",
            "New number of dashes", GH_ParamAccess.item);

        pManager.AddBooleanParameter("NewScaleToFit", "Scale",
            "New scale to fit setting", GH_ParamAccess.item);

        pManager.AddTextParameter("NewComments", "Comments",
            "New comments for the linetype", GH_ParamAccess.item);

        // Make all parameters optional except the first
        pManager[1].Optional = true;
        pManager[2].Optional = true;
        pManager[3].Optional = true;
        pManager[4].Optional = true;
        pManager[5].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadLineType(GH_ParamAccess.item), "LineType",
            "LineType", "The updated AutoCAD LineType", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD LineType", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD LineType", GH_ParamAccess.item);

        pManager.AddNumberParameter("PatternLength", "Length",
            "Total length of the linetype pattern", GH_ParamAccess.item);

        pManager.AddIntegerParameter("NumberOfDashes", "Dashes",
            "Number of dash segments in the pattern", GH_ParamAccess.item);

        pManager.AddBooleanParameter("ScaleToFit", "Scale",
            "Whether the pattern is scaled to fit geometry", GH_ParamAccess.item);

        pManager.AddTextParameter("Comments", "Comments",
            "Comments associated with the linetype", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadLinetypeTableRecordWrapper? lineType = null;

        if (!DA.GetData(0, ref lineType) || lineType is null) return;

        // Get current values as defaults
        var newName = lineType.Name;
        var newPatternLength = lineType.PatternLength;
        var newNumberOfDashes = lineType.NumDashes;
        var newScaleToFit = lineType.IsScaledToFit;
        var newComments = lineType.Comments;

        // Override with user inputs if provided
        DA.GetData(1, ref newName);
        DA.GetData(2, ref newPatternLength);
        DA.GetData(3, ref newNumberOfDashes);
        DA.GetData(4, ref newScaleToFit);
        DA.GetData(5, ref newComments);

        if (string.IsNullOrWhiteSpace(newName))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Line Type name cannot be empty");
            return;
        }

        if (newPatternLength < 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "PatternLength must be positive or zero");
            return;
        }

        if (newNumberOfDashes < 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "NumberOfDashes must be non-negative");
            return;
        }

        var change = newName != lineType.Name
                     || Math.Abs(newPatternLength - lineType.PatternLength) > _zeroTolerance
                     || newNumberOfDashes != lineType.NumDashes
                     || newScaleToFit != lineType.IsScaledToFit
                     || newComments != lineType.Comments;

        if (change)
        {
            var document = this.GetDocumentForObjectId(lineType.Id);
            if (document is null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No document available");
                return;
            }

            var transactionManager = document.CreateTransactionManager();

            lineType = transactionManager.PerformTask(() =>
            {
                var transaction = transactionManager.Unwrap();

                var cadLineType =
                    transaction.GetObject(lineType.Id.Unwrap(), OpenMode.ForWrite) as LinetypeTableRecord;

                cadLineType!.Name = newName;
                cadLineType.PatternLength = newPatternLength;
                cadLineType.NumDashes = newNumberOfDashes;
                cadLineType.IsScaledToFit = newScaleToFit;
                cadLineType.Comments = newComments ?? string.Empty;

                return new AutocadLinetypeTableRecordWrapper(cadLineType);
            });
        }

        // Output updated values
        var lineTypeGoo = new GH_AutocadLineType(lineType);

        DA.SetData(0, lineTypeGoo);
        DA.SetData(1, lineType.Name);
        DA.SetData(2, lineType.Id);
        DA.SetData(3, lineType.PatternLength);
        DA.SetData(4, lineType.NumDashes);
        DA.SetData(5, lineType.IsScaledToFit);
        DA.SetData(6, lineType.Comments);
    }
}
