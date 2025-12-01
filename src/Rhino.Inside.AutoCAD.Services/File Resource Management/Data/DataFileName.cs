using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IDataFileName"/>
public class DataFileName : IDataFileName
{
    /// <inheritdoc />
    public string FileName { get; set; } = string.Empty;

    /// <inheritdoc />
    public int Id { get; set; }
}