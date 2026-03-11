using Grasshopper.Kernel;
using System.Collections;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that retrieves a specific entry from an AutoCAD Dictionary by key.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class GetAutocadDictionaryEntryComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("d6f7a8b9-0c12-4d3e-bf40-5e6f7a8b9c0d");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAutocadDictionaryEntryComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAutocadDictionaryEntryComponent"/> class.
    /// </summary>
    public GetAutocadDictionaryEntryComponent()
        : base("Get Dictionary Entry", "AC-GetEntry",
            "Gets a specific entry from an AutoCAD Dictionary by key",
            "AutoCAD", "Data")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDictionary(GH_ParamAccess.item), "Dictionary",
            "Dict", "An AutoCAD Dictionary", GH_ParamAccess.item);

        pManager.AddTextParameter("Key", "Key",
            "The key of the entry to retrieve", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDictionaryEntry(GH_ParamAccess.item), "Entry", "E",
            "The dictionary entry containing the key and value.", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        AutocadDictionaryWrapper? dictionary = null;
        var key = string.Empty;

        if (!DA.GetData(0, ref dictionary)
            || dictionary is null) return;

        if (!DA.GetData(1, ref key)
            || string.IsNullOrEmpty(key)) return;

        var found = dictionary.TryGetValue(key, out var value);

        GH_AutocadDictionaryEntry? entry = null;

        if (found && value != null)
        {
            var dictionaryEntry = new DictionaryEntry(key, value);
            entry = new GH_AutocadDictionaryEntry(dictionaryEntry);
        }

        DA.SetData(0, entry);
    }
}
