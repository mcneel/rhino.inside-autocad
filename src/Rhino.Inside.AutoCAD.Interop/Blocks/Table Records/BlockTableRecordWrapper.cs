using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which wraps a <see cref="BlockTableRecord"/> from an <see cref="IAutoCadDocument"/>.
/// </summary>
/// <remarks>
/// Block table records are located in the <see cref="IBlockTableRecordRepository"/>
/// </remarks>
public class BlockTableRecordWrapper : DbObject, IBlockTableRecord
{
    private readonly BlockTableRecord _blockTableRecord;

    /// <inheritdoc />
    public string Name => _blockTableRecord.Name;

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockTableRecordWrapper() : base(new BlockTableRecord())
    {
        _blockTableRecord = (BlockTableRecord)_wrappedValue;
    }

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockTableRecordWrapper(BlockTableRecord blockTableRecord) : base(blockTableRecord)
    {
        _blockTableRecord = (BlockTableRecord)_wrappedValue;
    }

    /// <summary>
    /// Validates the <see cref="IBlockTableRecord"/> object.
    /// </summary>
    protected override bool Validate()
    {
        return base.Validate() && _blockTableRecord is { IsDisposed: false, IsUnloaded: false };
    }
}