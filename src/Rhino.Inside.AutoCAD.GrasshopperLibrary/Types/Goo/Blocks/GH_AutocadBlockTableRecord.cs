using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD block definitions.
/// </summary>
public class GH_AutocadBlockTableRecord : GH_AutocadObjectGoo<AutocadBlockTableRecordWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockTableRecord"/> class with no value.
    /// </summary>
    public GH_AutocadBlockTableRecord()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockTableRecord"/> class with the
    /// specified AutoCAD block definition.
    /// </summary>
    /// <param name="autocadBlockWrapper">The AutoCAD block definition to wrap.</param>
    public GH_AutocadBlockTableRecord(AutocadBlockTableRecordWrapper autocadBlockWrapper) : base(autocadBlockWrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockTableRecord"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadBlockTableRecord(GH_AutocadBlockTableRecord other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="GH_AutocadBlockTableRecord"/> via the interface.
    /// </summary>
    public GH_AutocadBlockTableRecord(IAutocadBlockTableRecord autocadBlockTableRecord)
        : base((autocadBlockTableRecord as AutocadBlockTableRecordWrapper)!)
    {
    }

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(BlockTableRecord);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new AutocadBlockTableRecordWrapper(unwrapped as BlockTableRecord);

        return new GH_AutocadBlockTableRecord(newWrapper);
    }
}