using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD block instances.
/// </summary>
public class GH_AutocadBlockReference : GH_AutocadObjectGoo<BlockReferenceWrapper>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockReference"/> class with no value.
    /// </summary>
    public GH_AutocadBlockReference()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockReference"/> class with the
    /// specified AutoCAD block instance.
    /// </summary>
    /// <param name="blockRefWrapper">The AutoCAD block instance to wrap.</param>
    public GH_AutocadBlockReference(BlockReferenceWrapper blockRefWrapper) : base(blockRefWrapper)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockReference"/> class by copying
    /// another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public GH_AutocadBlockReference(GH_AutocadBlockReference other)
    {
        this.Value = other.Value;
    }

    /// <summary>
    /// Constructs a new <see cref="GH_AutocadBlockReference"/> via the interface.
    /// </summary>
    public GH_AutocadBlockReference(IBlockReference blockReference)
        : base((blockReference as BlockReferenceWrapper)!)
    {
    }

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        return new GH_AutocadObject(dbObject);
    }
}
