using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoObjectModifiedEventArgs"/>
public class RhinoObjectModifiedEventArgs : IRhinoObjectModifiedEventArgs
{
    /// <inheritdoc/>
    public RhinoObject RhinoObject { get; }

    /// <inheritdoc/>
    public uint DocumentSerialNumber { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoObjectModifiedEventArgs"/> instance.
    /// </summary>
    /// <param name="rhinoObject">The Rhino object that was modified or appended.</param>
    /// <param name="document">
    /// The document the event was raised for, or null when Rhino did not report one.
    /// </param>
    public RhinoObjectModifiedEventArgs(RhinoObject rhinoObject, RhinoDoc? document)
    {
        this.RhinoObject = rhinoObject;
        this.DocumentSerialNumber = document?.RuntimeSerialNumber ?? 0;
    }
}
