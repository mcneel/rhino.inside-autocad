namespace Rhino.Inside.AutoCAD.Core.Interfaces;
/// <summary>
/// An event args interface created when a <see cref="IBlockReference"/> is added
/// to the <see cref="IBlockReferenceRepository"/>.
/// </summary>
public interface IBlockReferenceAddedEventArgs
{
    /// <summary>
    /// The block reference added to the <see cref="IBlockReferenceRepository"/>
    /// that raised this event args.
    /// </summary>
    IBlockReference BlockReference { get; }
}