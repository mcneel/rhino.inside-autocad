namespace Rhino.Inside.AutoCAD.Core;

/// <summary>
/// Represents a unique ID and command ID in AutoCAD for each
/// Application. Any change to these names must be coordinated
/// with the PackageContents.xml, Toolbar.cuix file, and
/// the RhinoInsideAutoCadCommands.cs file.
/// </summary>
public enum ButtonApplicationId
{
    /// <summary>
    /// The Rhino application.
    /// </summary>
    RHINO,

    /// <summary>
    /// The Grasshopper application.
    /// </summary>
    GRASSHOPPER
}