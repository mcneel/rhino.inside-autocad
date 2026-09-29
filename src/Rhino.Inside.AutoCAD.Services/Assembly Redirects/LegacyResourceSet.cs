using System.Resources;
using System.Resources.Extensions;

namespace Rhino.Inside.AutoCAD.Services;

/// <summary>
/// A <see cref="ResourceSet"/> which reads a .NET Framework assembly's resources without
/// BinaryFormatter.
/// </summary>
/// <remarks>
/// Reads through <see cref="DeserializingResourceReader"/>, which decodes the
/// BinaryFormatter blobs .NET Framework builds store Bitmaps as, on runtimes where
/// BinaryFormatter itself has been removed. At runtime the reader is the copy in the
/// Microsoft.WindowsDesktop.App shared framework, not the NuGet package this project
/// compiles against; from .NET 9 that copy decodes the blobs with System.Formats.Nrbf.
/// </remarks>
/// <seealso cref="LegacyResourceManager"/>
public sealed class LegacyResourceSet : ResourceSet
{
    /// <summary>
    /// Constructs a new <see cref="LegacyResourceSet"/> over a .resources stream.
    /// </summary>
    public LegacyResourceSet(Stream stream) : base(CreateReader(stream))
    {
    }

    /// <summary>
    /// Returns the <see cref="DeserializingResourceReader"/> over the stream.
    /// </summary>
    private static IResourceReader CreateReader(Stream stream)
    {
        var reader = new DeserializingResourceReader(stream);

        return reader;
    }

    /// <inheritdoc />
    public override Type GetDefaultReader() => typeof(DeserializingResourceReader);

    /// <inheritdoc />
    public override Type GetDefaultWriter() => typeof(PreserializedResourceWriter);
}
