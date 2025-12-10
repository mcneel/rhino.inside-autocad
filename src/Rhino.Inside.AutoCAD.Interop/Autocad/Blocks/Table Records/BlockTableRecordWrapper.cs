using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Geometry;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which wraps a <see cref="BlockTableRecord"/> from an <see cref="IAutocadDocument"/>.
/// </summary>
/// <remarks>
/// Block table records are located in the <see cref="IBlockTableRecordRepository"/>
/// </remarks>
public class BlockTableRecordWrapper : DbObjectWrapper, IBlockTableRecord
{
    private readonly GeometryConverter _geometryConverter = GeometryConverter.Instance!;
    private readonly BlockTableRecord _blockTableRecord;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public Point3d Origin { get; }

    /// <inheritdoc />
    public IObjectIdCollection ObjectIds { get; }

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockTableRecordWrapper() : base(new BlockTableRecord())
    {
        _blockTableRecord = (BlockTableRecord)_wrappedValue;

        this.Name = _blockTableRecord.Name;

        this.Origin = _geometryConverter.ToRhinoType(_blockTableRecord.Origin);

        this.ObjectIds = new AutocadObjectIdCollection();
    }

    /// <summary>
    /// Constructs a new <see cref="BlockReferenceWrapper"/>.
    /// </summary>
    public BlockTableRecordWrapper(BlockTableRecord blockTableRecord) : base(blockTableRecord)
    {
        _blockTableRecord = blockTableRecord;

        this.Name = blockTableRecord.Name;

        this.Origin = _geometryConverter.ToRhinoType(blockTableRecord.Origin);

        this.ObjectIds = new AutocadObjectIdCollection();
        foreach (var objectId in blockTableRecord)
        {
            this.ObjectIds.Add(new AutocadObjectId(objectId));
        }
    }

    /// <summary>
    /// Validates the <see cref="IBlockTableRecord"/> object.
    /// </summary>
    protected override bool Validate()
    {
        return base.Validate() && _blockTableRecord is { IsDisposed: false, IsUnloaded: false };
    }
}