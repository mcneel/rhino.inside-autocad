using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadGroup = Autodesk.AutoCAD.DatabaseServices.Group;
using OpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using RXObject = Autodesk.AutoCAD.Runtime.RXObject;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// The <see cref="IObjectEraser"/> for erasing <see cref="Group"/>s.
/// </summary>
public class GroupObjectEraser : IObjectEraser
{
    private readonly RXObject _groupRxClass;

    /// <inheritdoc />
    public IAutocadDocument AutocadDocument { get; }

    /// <summary>
    /// Constructs a new <see cref="GenericObjectEraser"/>.
    /// </summary>
    public GroupObjectEraser(IAutocadDocument autocadDocument)
    {
        _groupRxClass = RXObject.GetClass(typeof(CadGroup));

        this.AutocadDocument = autocadDocument;
    }

    /// <inheritdoc />
    public void Erase(IDbObject dbObject)
    {
        var dbObjectUnwrapped = dbObject.UnwrapObject();

        if (dbObjectUnwrapped.GetRXClass().Equals(_groupRxClass) == false)
            return;

        var group = (CadGroup)dbObjectUnwrapped;

        var groupedIds = group.GetAllEntityIds();

        foreach (var id in groupedIds)
        {
            var groupObject = id.GetObject(OpenMode.ForWrite);

            groupObject.Erase(true);
        }

        dbObjectUnwrapped.Erase(true);
    }
}