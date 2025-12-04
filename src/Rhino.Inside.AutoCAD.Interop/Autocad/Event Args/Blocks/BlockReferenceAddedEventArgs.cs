using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An event args class created when a <see cref="IBlockReference"/> is added
/// to the <see cref="IBlockReferenceRepository"/>.
/// </summary>
public class BlockReferenceAddedEventArgs : EventArgs, IBlockReferenceAddedEventArgs
{
    /// <summary>
    /// Constructs a new <see cref="BlockReferenceAddedEventArgs"/>.
    /// </summary>
    public BlockReferenceAddedEventArgs(IBlockReference blockReference)
    {
        this.BlockReference = blockReference;
    }
    /// <summary>
    /// The block reference added to the <see cref="IBlockReferenceRepository"/>
    /// that raised this event args.
    /// </summary>
    public IBlockReference BlockReference { get; }
}