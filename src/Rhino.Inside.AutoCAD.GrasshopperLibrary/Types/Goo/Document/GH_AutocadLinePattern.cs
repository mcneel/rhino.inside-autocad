using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD line patterns.
/// </summary>
public class GH_AutocadLinePattern : GH_Goo<AutocadLinePattern>
{
    /// <inheritdoc />
    public override bool IsValid => this.Value != null;

    /// <inheritdoc />
    public override string TypeName => nameof(AutocadLinePattern);

    /// <inheritdoc />
    public override string TypeDescription => "Represents an AutoCAD line pattern object";

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLinePattern"/> class with no value.
    /// </summary>
    public GH_AutocadLinePattern()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLinePattern"/> class with the
    /// specified AutoCAD line pattern.
    /// </summary>
    /// <param name="linePattern">The AutoCAD line pattern to wrap.</param>
    public GH_AutocadLinePattern(AutocadLinePattern linePattern) : base(linePattern)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLinePattern"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadLinePattern(GH_AutocadLinePattern other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="ILinePattern"/> via the interface.
    /// </summary>
    public GH_AutocadLinePattern(IAutocadLinePattern linePattern)
        : base((linePattern as AutocadLinePattern)!)
    {

    }

    /// <inheritdoc />
    public override IGH_Goo Duplicate()
    {
        var clone = this.Value.ShallowClone();

        return new GH_AutocadLinePattern(clone);
    }

    /// <inheritdoc />
    public override bool CastFrom(object source)
    {
        if (source is GH_AutocadLinePattern goo)
        {
            this.Value = goo.Value;
            return true;
        }

        if (source is AutocadLinePattern linePattern)
        {
            this.Value = linePattern;
            return true;
        }

        if (source is GH_AutocadObject objectGoo
            && objectGoo.Value.Unwrap() is LinetypeTableRecord lineTypeFromObjectGoo)
        {
            this.Value = new AutocadLinePattern(lineTypeFromObjectGoo);
            return true;

        }

        if (source is DbObjectWrapper dbObject
            && dbObject.Unwrap() is LinetypeTableRecord lineTypeFromObject)
        {
            this.Value = new AutocadLinePattern(lineTypeFromObject);
            return true;

        }

        return false;
    }

    /// <inheritdoc />
    public override bool CastTo<Q>(ref Q target)
    {
        if (typeof(Q).IsAssignableFrom(typeof(AutocadLinePattern)))
        {
            target = (Q)(object)this.Value;
            return true;
        }

        if (typeof(Q).IsAssignableFrom(typeof(GH_AutocadLinePattern)))
        {
            target = (Q)(object)new GH_AutocadLinePattern(this.Value);
            return true;
        }
        return false;
    }
    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Value == null)
            return "Null AutocadLinePattern";

        return $"AutocadLinePattern [Name: {this.Value.Name}, Id: {this.Value.Id.Value} ]";
    }
}

