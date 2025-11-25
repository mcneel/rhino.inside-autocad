using Bimorph.Core.Services.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface providing essential file resource directories and file names
/// for obtaining external resources.
/// </summary>
public interface IRhinoInsideAutoCadFileResourceManager : IFileResourceManager
{
    /// <summary>
    /// The <see cref="IFileNameLibrary"/>.
    /// </summary>
    IFileNameLibrary FileNameLibrary { get; }

    /// <summary>
    /// The <see cref="IJsonNameLibrary"/>.
    /// </summary>
    IJsonNameLibrary JsonNameLibrary { get; }
}