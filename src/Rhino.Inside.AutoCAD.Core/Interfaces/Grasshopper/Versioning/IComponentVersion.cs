using GH_IO.Serialization;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a versioning interface for a component, providing mechanisms
/// to read and write version information.
/// </summary>
public interface IComponentVersion
{
    /// <summary>
    /// Gets the current version of the component.
    /// </summary>
    Version CurrentVersion { get; }

    /// <summary>
    /// Reads the version information of the component from the specified reader.
    /// </summary>
    /// <param name="reader">
    /// The reader used to deserialize the version information.
    /// </param>
    /// <param name="componentName">
    /// The name of the component being read.
    /// </param>
    /// <returns>
    /// <c>true</c> if the version information was successfully read; otherwise, <c>false</c>.
    /// </returns>
    bool Read(GH_IReader reader, string componentName);

    /// <summary>
    /// Writes the version information of the component to the specified writer.
    /// </summary>
    /// <param name="writer">
    /// The writer used to serialize the version information.
    /// </param>
    /// <returns>
    /// <c>true</c> if the version information was successfully written; otherwise, <c>false</c>.
    /// </returns>
    bool Write(GH_IWriter writer);
}