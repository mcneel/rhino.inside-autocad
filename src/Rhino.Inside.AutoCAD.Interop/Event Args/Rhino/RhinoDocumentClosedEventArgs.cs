using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoDocumentClosedEventArgs"/>
public class RhinoDocumentClosedEventArgs : IRhinoDocumentClosedEventArgs
{
    /// <inheritdoc/>
    public uint DocumentSerialNumber { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoDocumentClosedEventArgs"/> instance.
    /// </summary>
    /// <param name="documentSerialNumber">
    /// The runtime serial number of the document whose objects are gone.
    /// </param>
    public RhinoDocumentClosedEventArgs(uint documentSerialNumber)
    {
        this.DocumentSerialNumber = documentSerialNumber;
    }
}
