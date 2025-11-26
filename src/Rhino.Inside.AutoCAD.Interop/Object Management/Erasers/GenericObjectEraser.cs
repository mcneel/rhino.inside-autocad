using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Implements default object erasing in the <see cref="IDocument"/>.
/// </summary>
public class GenericObjectEraser : IObjectEraser
{
    /// <inheritdoc />
    public IDocument Document { get; }

    /// <summary>
    /// Constructs a new <see cref="GenericObjectEraser"/>.
    /// </summary>
    public GenericObjectEraser(IDocument document)
    {
        this.Document = document;
    }

    /// <inheritdoc />
    public void Erase(IDbObject dbObject)
    {
        var dbObjectUnwrapped = dbObject.Unwrap();

        dbObjectUnwrapped.Erase(true);
    }
}