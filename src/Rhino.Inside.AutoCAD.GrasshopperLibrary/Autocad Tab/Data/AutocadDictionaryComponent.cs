using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from an AutoCAD Dictionary.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class AutocadDictionaryComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("c5e6f7a8-9b01-4c2d-ae3f-4d5e6f7a8b9c");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadDictionaryComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadDictionaryComponent"/> class.
    /// </summary>
    public AutocadDictionaryComponent()
        : base("AutoCAD Dictionary", "AC-Dict",
            "Gets Information from an AutoCAD Dictionary",
            "AutoCAD", "Data")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDictionary(GH_ParamAccess.item), "Dictionary",
            "Dict", "An AutoCAD Dictionary", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "Name",
            "The name of the AutoCAD Dictionary.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadObjectId(GH_ParamAccess.item), "Id", "Id",
            "The Id of the AutoCAD Dictionary.", GH_ParamAccess.item);

        pManager.AddParameter(new Param_AutocadDictionaryEntry(GH_ParamAccess.list), "Entries", "E",
            "The entries in the Dictionary as key-value pairs.", GH_ParamAccess.list);

        pManager.AddIntegerParameter("Count", "Count",
            "The number of entries in the Dictionary.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDictionaryWrapper? dictionary = null;

        if (!DA.GetData(0, ref dictionary)
            || dictionary is null) return;

        var name = dictionary.Name ?? string.Empty;
        var id = dictionary.Id;
        var count = dictionary.Count;

        // Get all entries and convert to DictionaryEntry objects
        var entries = new List<GH_AutocadDictionaryEntry>();
        var allEntries = dictionary.GetAllEntries();

        foreach (var entry in allEntries)
        {
            var dictionaryEntry = new DictionaryEntry(entry.Key, entry.Value);
            entries.Add(new GH_AutocadDictionaryEntry(dictionaryEntry));
        }

        DA.SetData(0, name);
        DA.SetData(1, id);
        DA.SetDataList(2, entries);
        DA.SetData(3, count);
    }
}
