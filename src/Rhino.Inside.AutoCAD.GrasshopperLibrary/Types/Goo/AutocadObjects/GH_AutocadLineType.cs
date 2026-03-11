using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD line patterns.
/// </summary>
public class GH_AutocadLineType : GH_AutocadObjectGoo<AutocadLinetypeTableRecordWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLineType"/> class with no value.
    /// </summary>
    public GH_AutocadLineType()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLineType"/> class with the
    /// specified AutoCAD line pattern.
    /// </summary>
    /// <param name="linetypeTableRecordWrapper">The AutoCAD line pattern to wrap.</param>
    public GH_AutocadLineType(AutocadLinetypeTableRecordWrapper linetypeTableRecordWrapper) : base(linetypeTableRecordWrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadLineType"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadLineType(GH_AutocadLineType other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="IAutocadLinetypeTableRecord"/> via the interface.
    /// </summary>
    public GH_AutocadLineType(IAutocadLinetypeTableRecord linetypeTableRecord)
        : base((linetypeTableRecord as AutocadLinetypeTableRecordWrapper)!)
    {

    }

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(LinetypeTableRecord);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new AutocadLinetypeTableRecordWrapper(unwrapped as LinetypeTableRecord);

        return new GH_AutocadLineType(newWrapper);
    }
}