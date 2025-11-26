using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Implements layout object erasing in the <see cref="IDocument"/>.
/// </summary>
public class LayoutObjectEraser : IObjectEraser
{
    private readonly RXClass _layoutRxClass;

    /// <inheritdoc/>
    public IDocument Document { get; }

    /// <summary>
    /// Constructs a new <see cref="GenericObjectEraser"/>.
    /// </summary>
    public LayoutObjectEraser(IDocument document)
    {
        _layoutRxClass = RXObject.GetClass(typeof(Layout));

        this.Document = document;
    }

    /// <inheritdoc/>
    public void Erase(IDbObject dbObject)
    {
        using var layoutManager = LayoutManager.Current;

        var dbObjectUnwrapped = dbObject.Unwrap();

        if (dbObjectUnwrapped.GetRXClass().Equals(_layoutRxClass) == false)
            return;

        var layout = (Layout)dbObjectUnwrapped;

        var layoutName = layout.LayoutName;

        if (layoutManager.LayoutExists(layoutName)) layoutManager.DeleteLayout(layoutName);
    }
}