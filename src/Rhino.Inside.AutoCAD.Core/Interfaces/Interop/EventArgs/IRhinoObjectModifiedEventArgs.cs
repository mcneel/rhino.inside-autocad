using Rhino.DocObjects;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Event arguments for when a Rhino object is modified or appended.
/// </summary>
public interface IRhinoObjectModifiedEventArgs
{
    /// <summary>
    /// The Rhino object that was modified or appended.
    /// </summary>
    RhinoObject RhinoObject { get; }
}