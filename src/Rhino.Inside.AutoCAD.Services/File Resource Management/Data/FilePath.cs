using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IFilepath"/>
public class FilePath : IFilepath
{
    /// <inheritdoc />
    public string FullFilePath { get; }

    /// <summary>
    /// Constructs a new <see cref="FilePath"/>.
    /// </summary>
    public FilePath(string fullFilePath)
    {
        this.FullFilePath = fullFilePath;
    }
}