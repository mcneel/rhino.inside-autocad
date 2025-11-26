namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// An extensions class for <see cref="ObjectId"/>s.
/// </summary>
public static class DbObjectExtensions
{
    /// <summary>
    /// Returns the string of the <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId"/>
    /// value.
    /// </summary>
    public static string GetIdString(this Autodesk.AutoCAD.DatabaseServices.DBObject objectId) =>
        objectId.Handle.Value.ToString();
}