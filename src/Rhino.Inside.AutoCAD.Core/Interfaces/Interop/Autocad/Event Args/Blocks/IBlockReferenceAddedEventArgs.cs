namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An event args interface created when a <see cref="IBlockReference"/> is added
/// to the <see cref="IBlockTableRecordRepository"/>.
/// </summary>
public interface IBlockReferenceAddedEventArgs
{
    /// <summary>
    /// The block reference added to the <see cref="IBlockTableRecordRepository"/>
    /// that raised this event args.
    /// </summary>
    IBlockReference BlockReference { get; }
}