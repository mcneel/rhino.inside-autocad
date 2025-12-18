using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.GrasshopperLibrary.Autocad_Tab.Base;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates a new AutoCAD linetype.
/// </summary>
[ComponentVersion(introduced: "1.0.0")]
public class CreateAutocadLineTypeComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("a8f5c3d2-4e7b-4a9c-8d1f-6e3b2c5a8f9d");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SetAutocadLineTypeComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAutocadLineTypeComponent"/> class.
    /// </summary>
    public CreateAutocadLineTypeComponent()
        : base("Create AutoCAD Line Type", "AC-AddLT",
            "Creates a new AutoCAD LineType",
            "AutoCAD", "LineTypes")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "An AutoCAD Document", GH_ParamAccess.item);

        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD LineType", GH_ParamAccess.item);

        pManager.AddNumberParameter("PatternLength", "Length",
            "Total length of the linetype pattern", GH_ParamAccess.item, 1.0);

        pManager.AddIntegerParameter("NumberOfDashes", "Dashes",
            "Number of dash segments in the pattern", GH_ParamAccess.item, 2);

        pManager.AddBooleanParameter("ScaleToFit", "Scale",
            "Whether to scale pattern to fit geometry", GH_ParamAccess.item, false);
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
        AutocadDocument? autocadDocument = null;
        var name = string.Empty;

        if (!DA.GetData(0, ref autocadDocument) || autocadDocument is null) return;
        if (!DA.GetData(1, ref name) || string.IsNullOrEmpty(name)) return;

        var patternLength = 1.0;
        var numberOfDashes = 2;
        var scaleToFit = false;

        DA.GetData(2, ref patternLength);
        DA.GetData(3, ref numberOfDashes);
        DA.GetData(4, ref scaleToFit);

        // Validation
        if (patternLength <= 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "PatternLength must be positive");
            return;
        }

        if (numberOfDashes < 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "NumberOfDashes must be non-negative");
            return;
        }

        if (!autocadDocument.LineTypeRepository.TryAddLineType(name, patternLength, numberOfDashes, scaleToFit, out var lineType))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "LineType already exists");
            // Still output existing linetype info
        }

        DA.SetData(0, lineType.Name);
        DA.SetData(1, lineType.Id);
        DA.SetData(2, lineType.PatternLength);
        DA.SetData(3, lineType.NumDashes);
        DA.SetData(4, lineType.IsScaledToFit);
        DA.SetData(5, lineType.Comments);
    }
}
