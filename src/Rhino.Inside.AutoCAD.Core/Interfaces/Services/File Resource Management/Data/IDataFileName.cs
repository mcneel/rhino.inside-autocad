namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the name of a data file resource, e.g. a JSON file.
/// The file contents provides data for the application.
/// </summary>
public interface IDataFileName
{
    /// <summary>
    /// The name of the data file resource. This includes the file extension
    /// name, e.g. "data.json".
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// The unique identifier of the data file resource.
    /// </summary>
    /// <remarks>
    /// Used for external purposes, e.g. to identify the file and provide a
    /// file name in short format.
    /// </remarks>
    int Id { get; }
}