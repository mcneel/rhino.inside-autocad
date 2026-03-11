using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD XRecords.
/// </summary>
public class GH_AutocadXRecord : GH_AutocadObjectGoo<XRecordWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadXRecord"/> class with no value.
    /// </summary>
    public GH_AutocadXRecord()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadXRecord"/> class with the
    /// specified AutoCAD XRecord.
    /// </summary>
    /// <param name="xrecordWrapper">The AutoCAD XRecord to wrap.</param>
    public GH_AutocadXRecord(XRecordWrapper xrecordWrapper) : base(xrecordWrapper)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadXRecord"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadXRecord(GH_AutocadXRecord other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="GH_AutocadXRecord"/> via the interface.
    /// </summary>
    public GH_AutocadXRecord(IXRecord xrecord)
        : base((xrecord as XRecordWrapper)!)
    {
    }

    /// <inheritdoc />
    protected override Type GetCadType() => typeof(Xrecord);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new XRecordWrapper(unwrapped as Xrecord);

        return new GH_AutocadXRecord(newWrapper);
    }
}