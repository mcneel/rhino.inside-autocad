namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface storing directories local to a Rhino.Inside.AutoCAD.Core project (DWG) file.
/// </summary>
public interface IProjectDirectories
{
    /// <summary>
    /// The location on Rhino.Inside.AutoCAD.Core servers where the project template files are
    /// stored.
    /// </summary>
    string Templates { get; set; }
}