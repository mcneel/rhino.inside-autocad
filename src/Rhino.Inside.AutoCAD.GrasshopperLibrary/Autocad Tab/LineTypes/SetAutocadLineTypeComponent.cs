using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that sets properties for an AutoCAD linetype.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class SetAutocadLineTypeComponent : RhinoInsideAutocad_Component
{
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
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
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
        AutocadLinetypeTableRecord? lineType = null;

        if (!DA.GetData(0, ref lineType) || lineType is null) return;

        // Get current values as defaults
        var newPatternLength = lineType.PatternLength;
        var newNumberOfDashes = lineType.NumDashes;
        var newScaleToFit = lineType.IsScaledToFit;
        var newComments = lineType.Comments;

        // Override with user inputs if provided
        DA.GetData(1, ref newPatternLength);
        DA.GetData(2, ref newNumberOfDashes);
        DA.GetData(3, ref newScaleToFit);
        DA.GetData(4, ref newComments);

        // Validation
        if (newPatternLength <= 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "PatternLength must be positive");
            return;
        }

        if (newNumberOfDashes < 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "NumberOfDashes must be non-negative");
            return;
        }

        var cadLineType = lineType.Unwrap();
        cadLineType.PatternLength = newPatternLength;
        cadLineType.NumDashes = newNumberOfDashes;
        cadLineType.IsScaledToFit = newScaleToFit;
        cadLineType.Comments = newComments ?? string.Empty;

        // Note: When modifying dash pattern properties, the dash lengths would need to be recalculated
        // For now, we update the properties. A full implementation would regenerate the dash pattern.

        lineType = new AutocadLinetypeTableRecord(cadLineType);

        // Output updated values
        DA.SetData(0, lineType.Name);
        DA.SetData(1, lineType.Id);
        DA.SetData(2, lineType.PatternLength);
        DA.SetData(3, lineType.NumDashes);
        DA.SetData(4, lineType.IsScaledToFit);
        DA.SetData(5, lineType.Comments);
    }
}
