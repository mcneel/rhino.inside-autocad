using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that extracts information from an AutoCAD Dictionary Entry.
/// </summary>
[ComponentVersion(introduced: "1.0.16")]
public class AutocadDictionaryEntryComponent : RhinoInsideAutocad_ComponentBase
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new("d168c8b2-046c-4aa8-91ce-91d8f3f39dd1");

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.AutocadDictionaryEntryComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutocadDictionaryEntryComponent"/> class.
    /// </summary>
    public AutocadDictionaryEntryComponent()
        : base("AutoCAD Dictionary Entry", "AC-DictEntry",
            "Gets the Key and Value from an AutoCAD Dictionary Entry",
            "AutoCAD", "Data")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDictionaryEntry(GH_ParamAccess.item), "Entry",
            "E", "An AutoCAD Dictionary Entry", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Key", "K",
            "The key of the dictionary entry.", GH_ParamAccess.item);

        pManager.AddGenericParameter("Value", "V",
            "The value of the dictionary entry (XRecord, Dictionary, or other object).", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        DictionaryEntry? entry = null;

        if (!DA.GetData(0, ref entry)
            || entry is null) return;

        var key = entry.Key;
        var value = entry.Value;

        // Convert the value to the appropriate Goo type
        var gooConverter = new GooConverter();
        var valueGoo = gooConverter.CreateGoo(value);

        DA.SetData(0, key);
        DA.SetData(1, valueGoo);
    }
}
