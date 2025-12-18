using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IGrasshopperObjectModifiedEventArgs"/>
public class GrasshopperObjectModifiedEventArgs : IGrasshopperObjectModifiedEventArgs
{
    /// <inheritdoc/>
    public IGH_DocumentObject GrasshopperObject { get; }

    /// <summary>
    /// Constructs a new <see cref="IGrasshopperObjectModifiedEventArgs"/> instance.
    /// </summary>
    public GrasshopperObjectModifiedEventArgs(IGH_DocumentObject grasshopperObject)
    {
        this.GrasshopperObject = grasshopperObject;
    }
}