using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A class that bakes Rhino convertible objects to AutoCAD.
/// </summary>
public class BakableRhinoConverter : IAutocadBakeable
{
    private readonly AutocadColorConverter _colorConverter = AutocadColorConverter.Instance!;

    private readonly IRhinoConvertible rhinoConvertible;

    /// <summary>
    /// Constructs a new <see cref="BakableRhinoConverter"/> instance.
    /// </summary>
    /// <param name="rhinoConvertible"></param>
    public BakableRhinoConverter(IRhinoConvertible rhinoConvertible)
    {
        this.rhinoConvertible = rhinoConvertible;
    }

    /// <summary>
    /// Applies the given settings to the block reference.
    /// </summary>
    protected void ApplySettings(IBakeSettings? settings, Entity entity)
    {
        if (settings is null) return;

        if (settings.Layer != null)
            entity.LayerId = settings.Layer.Id.Unwrap();

        if (settings?.LineType != null)
            entity.LinetypeId = settings.LineType.Id.Unwrap();

        if (settings?.Color != null)
        {
            var color = settings.Color;
            entity.Color = _colorConverter.ToCadColor(color);
        }
    }

    /// <inheritdoc />
    public List<IObjectId> BakeToAutocad(ITransactionManager transactionManager,
        IBakingComponent bakingComponent, IBakeSettings? settings = null)
    {
        var convert = rhinoConvertible.Convert(transactionManager);

        var transaction = transactionManager.Unwrap();

        var modelSpace = transactionManager.GetModelSpaceBlockTableRecord(openForWrite: true);

        var modelSpaceRecord = modelSpace.Unwrap();

        var idList = new List<IObjectId>();
        foreach (var entity in convert)
        {

            var cadEntity = entity.Unwrap();

            this.ApplySettings(settings, cadEntity);

            var objectId = modelSpaceRecord.AppendEntity(cadEntity);

            idList.Add(new AutocadObjectId(objectId));

            transaction.AddNewlyCreatedDBObject(cadEntity, true);
        }

        return idList;
    }
}