using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD dictionary entries.
/// </summary>
public class GH_AutocadDictionaryEntry : GH_Goo<DictionaryEntry>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => "Dictionary Entry";

    /// <inheritdoc />
    public override string TypeDescription => "Represents an entry in an AutoCAD dictionary";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDictionaryEntry"/> class with no value.
    /// </summary>
    public GH_AutocadDictionaryEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDictionaryEntry"/> class with the
    /// specified dictionary entry.
    /// </summary>
    /// <param name="entry">The dictionary entry to wrap.</param>
    public GH_AutocadDictionaryEntry(DictionaryEntry entry) : base(entry)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDictionaryEntry"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadDictionaryEntry(GH_AutocadDictionaryEntry other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDictionaryEntry"/> class from
    /// the interface.
    /// </summary>
    /// <param name="entry">The dictionary entry interface.</param>
    public GH_AutocadDictionaryEntry(IDictionaryEntry entry)
        : base((entry as DictionaryEntry)!)
    {
    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        return new GH_AutocadDictionaryEntry(this);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadDictionaryEntry goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is DictionaryEntry entry)
        {
            this.Value = entry;
            return true;
        }

        if (source is IDictionaryEntry iEntry)
        {
            this.Value = (DictionaryEntry)iEntry;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(DictionaryEntry)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(IDictionaryEntry)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadDictionaryEntry)))
        {
            target = (Q)(object)new GH_AutocadDictionaryEntry(this.Value);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null Dictionary Entry";

        return $"Entry [{this.Value.Key}]";
    }
}
