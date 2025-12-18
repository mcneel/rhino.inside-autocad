using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Represents a Grasshopper Goo object for AutoCAD block instances.
/// </summary>
public class GH_AutocadBlockReference : GH_AutocadObjectGoo<BlockReferenceWrapper>, IAutocadBakeable
{
    private readonly AutocadColorConverter _colorConverter = AutocadColorConverter.Instance;
    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockReference"/> class with no value.
    /// </summary>
    public GH_AutocadBlockReference()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GH_AutocadBlockReference"/> class with the
    /// specified AutoCAD block instance.
    /// </summary>
    /// <param name="blockRefWrapper">The AutoCAD block instance to wrap.</param>
    public GH_AutocadBlockReference(BlockReferenceWrapper blockRefWrapper) : base(blockRefWrapper)
    { }

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
    protected override Type GetCadType() => typeof(BlockReference);

    /// <inheritdoc />
    protected override IGH_Goo CreateInstance(IDbObject dbObject)
    {
        var unwrapped = dbObject.UnwrapObject();

        var newWrapper = new BlockReferenceWrapper(unwrapped as BlockReference);

        return new GH_AutocadBlockReference(newWrapper);
    }

    /// <summary>
    /// Applies the given settings to the block reference.
    /// </summary>
    private void ApplySettings(IBakeSettings? settings, BlockReference blockReference)
    {
        if (settings is null) return;

        if (settings.Layer != null)
            blockReference.LayerId = settings.Layer.Id.Unwrap();

        if (settings?.LineType != null)
            blockReference.LinetypeId = settings.LineType.Id.Unwrap();

        if (settings?.Color != null)
        {
            var color = settings.Color;
            blockReference.Color = _colorConverter.ToCadColor(color);
        }
    }

    /// <inheritdoc />
    public List<IObjectId> BakeToAutocad(ITransactionManager transactionManager, IBakingComponent bakingComponent, IBakeSettings? settings = null)
    {
        if (this.Value == null)
            throw new InvalidOperationException("Cannot bake a null block reference");

        var transaction = transactionManager.Unwrap();

        var modelSpace = transactionManager.GetModelSpaceBlockTableRecord(openForWrite: true);

        var modelSpaceRecord = modelSpace.Unwrap();

        var sourceBlockRef = this.Value.Unwrap();

        var blockReference = (BlockReference)sourceBlockRef.Clone();

        this.ApplySettings(settings, blockReference);

        var objectId = modelSpaceRecord.AppendEntity(blockReference);

        transaction.AddNewlyCreatedDBObject(blockReference, true);

        return [new AutocadObjectId(objectId)];
    }
}