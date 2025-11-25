namespace Rhino.Inside.AutoCAD.Core.Enum.Application;

/// <summary>
/// Represents a unique ID and command ID in AutoCAD for each
/// Application. Any change to these names must be coordinated
/// with the PackageContents.xml, AWI_Toolbar.cuix file, and
/// the RhinoInsideAutoCadCommands.cs file.
/// </summary>
public enum RhinoInsideApplicationId
{
    /// <summary>
    /// The Rhino application.
    /// </summary>
    Rhino,

    /// <summary>
    /// The Grasshopper application.
    /// </summary>
    Grasshopper
}