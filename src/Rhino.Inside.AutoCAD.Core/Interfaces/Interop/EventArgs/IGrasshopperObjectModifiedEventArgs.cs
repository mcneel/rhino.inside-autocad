using Grasshopper.Kernel;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Event arguments for when a Grasshopper object is modified or appended.
/// </summary>
public interface IGrasshopperObjectModifiedEventArgs
{
    /// <summary>
    /// The Grasshopper object that was modified or appended.
    /// </summary>
    IGH_DocumentObject GrasshopperObject { get; }
}