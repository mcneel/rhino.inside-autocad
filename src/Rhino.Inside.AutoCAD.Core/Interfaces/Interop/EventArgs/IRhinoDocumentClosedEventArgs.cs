namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Event arguments for when a Rhino document is closed, or its contents are about to be
/// replaced by a file being opened into it.
/// </summary>
public interface IRhinoDocumentClosedEventArgs
{
    /// <summary>
    /// The <see cref="RhinoDoc.RuntimeSerialNumber"/> of the document whose objects are gone.
    /// </summary>
    uint DocumentSerialNumber { get; }
}