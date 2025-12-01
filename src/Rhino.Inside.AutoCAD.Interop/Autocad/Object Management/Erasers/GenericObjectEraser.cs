using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Implements default object erasing in the <see cref="IAutoCadDocument"/>.
/// </summary>
public class GenericObjectEraser : IObjectEraser
{
    /// <inheritdoc />
    public IAutoCadDocument AutoCadDocument { get; }

    /// <summary>
    /// Constructs a new <see cref="GenericObjectEraser"/>.
    /// </summary>
    public GenericObjectEraser(IAutoCadDocument autoCadDocument)
    {
        this.AutoCadDocument = autoCadDocument;
    }

    /// <inheritdoc />
    public void Erase(IDbObject dbObject)
    {
        var dbObjectUnwrapped = dbObject.Unwrap();

        dbObjectUnwrapped.Erase(true);
    }
}