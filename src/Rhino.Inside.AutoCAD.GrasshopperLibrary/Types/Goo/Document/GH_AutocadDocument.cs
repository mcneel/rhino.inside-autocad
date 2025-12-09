using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD documents.
/// </summary>
public class GH_AutocadDocument : GH_Goo<AutocadDocument>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => nameof(AutocadDocument);

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD document object";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDocument"/> class with no value.
    /// </summary>
    public GH_AutocadDocument()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDocument"/> class with the
    /// specified AutoCAD document.
    /// </summary>
    /// <param name="document">The AutoCAD document to wrap.</param>
    public GH_AutocadDocument(AutocadDocument document) : base(document)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadDocument"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadDocument(GH_AutocadDocument other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IAutocadDocument"/> via the interface.
    /// </summary>
    public GH_AutocadDocument(IAutocadDocument autocadDocument)
        : base((autocadDocument as AutocadDocument)!)
    { }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return new GH_AutocadDocument(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadDocument goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadDocument document)
        {
            this.Value = document;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadDocument)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadDocument)))
        {
            target = (Q)(object)new GH_AutocadDocument(this.Value);
            return true;
        }
        return false;
    }
    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadDocument";

        return $"AutocadDocument [Name: {this.Value.FileInfo.FileName}, Id: {this.Value.Id} ]";
    }
}