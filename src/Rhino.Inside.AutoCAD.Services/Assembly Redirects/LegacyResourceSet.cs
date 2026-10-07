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
/// <para>
/// A <see cref="ResourceSet"/> reads every value once and hands out that same instance on
/// each lookup, whereas the stock reader deserializes a new Bitmap per lookup. Callers rely
/// on that: Grasshopper's own GH_FilePanel disposes the Bitmap it gets from Res_FileIcons,
/// which would leave GH_FileEntry's static copy of it invalid and crash the recent files
/// menu with "Parameter is not valid". Cloneable values are therefore returned as copies.
/// </para>
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

    /// <summary>
    /// Returns a copy of the value if it is cloneable, such as a Bitmap, so that a caller
    /// disposing it does not invalidate the cached instance. Strings are returned as they are.
    /// </summary>
    private static object? CopyValue(object? value)
    {
        if (value is ICloneable cloneable and not string)
            return cloneable.Clone();

        return value;
    }

    /// <inheritdoc />
    public override object? GetObject(string name) => CopyValue(base.GetObject(name));

    /// <inheritdoc />
    public override object? GetObject(string name, bool ignoreCase) =>
        CopyValue(base.GetObject(name, ignoreCase));

    /// <inheritdoc />
    public override Type GetDefaultReader() => typeof(DeserializingResourceReader);

    /// <inheritdoc />
    public override Type GetDefaultWriter() => typeof(PreserializedResourceWriter);
}
