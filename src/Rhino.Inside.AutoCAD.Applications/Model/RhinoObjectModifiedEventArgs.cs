using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Applications;

/// <inheritdoc cref="IRhinoObjectModifiedEventArgs"/>
public class RhinoObjectModifiedEventArgs : IRhinoObjectModifiedEventArgs
{
    /// <inheritdoc/>
    public RhinoObject RhinoObject { get; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoObjectModifiedEventArgs"/> instance.
    /// </summary>
    public RhinoObjectModifiedEventArgs(RhinoObject rhinoObject)
    {
        this.RhinoObject = rhinoObject;
    }
}
