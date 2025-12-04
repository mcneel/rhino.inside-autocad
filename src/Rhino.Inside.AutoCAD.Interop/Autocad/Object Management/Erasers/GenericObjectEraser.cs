using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Implements default object erasing in the <see cref="IAutocadDocument"/>.
/// </summary>
public class GenericObjectEraser : IObjectEraser
{
    /// <inheritdoc />
    public IAutocadDocument AutocadDocument { get; }

    /// <summary>
    /// Constructs a new <see cref="GenericObjectEraser"/>.
    /// </summary>
    public GenericObjectEraser(IAutocadDocument autocadDocument)
    {
        this.AutocadDocument = autocadDocument;
    }

    /// <inheritdoc />
    public void Erase(IDbObject dbObject)
    {
        var dbObjectUnwrapped = dbObject.Unwrap();

        dbObjectUnwrapped.Erase(true);
    }
}