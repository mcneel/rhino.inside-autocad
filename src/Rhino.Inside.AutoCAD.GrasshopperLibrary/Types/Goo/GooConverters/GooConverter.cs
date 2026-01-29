using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel.Types;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// Provides methods to convert objects to specific types or retrieve their identifiers.
/// </summary>
public class GooConverter
{
    /// <summary>
    /// Attempts to convert the specified source object to the target type
    /// <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">
    /// The type to which the source object should be converted.
    /// </typeparam>
    /// <param name="source">
    /// The object to be converted.
    /// </param>
    /// <param name="target">
    /// The converted object if the conversion is successful;
    /// otherwise, the default value of <typeparamref name="TTarget"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the conversion is successful; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool TryConvertFromGoo<TTarget>(object source, out TTarget? target)
    {
        if (source is TTarget isCast)
        {
            target = isCast;
            return true;
        }
        if (source is IGH_AutocadReferenceDatabaseObject ghAutocadReferenceObject)
        {
            var dbObject = ghAutocadReferenceObject.ObjectValue;
            if (dbObject is TTarget targetCast)
            {
                target = targetCast;
                return true;
            }
        }
        if (source is IDbObject autocadReferenceObject)
        {
            var dbObject = autocadReferenceObject;
            if (dbObject is TTarget targetCast)
            {
                target = targetCast;
                return true;
            }
        }
        target = default;
        return false;
    }

    /// <summary>
    /// Attempts to retrieve the <see cref="IObjectId"/> from the specified source object.
    /// </summary>
    /// <param name="source">
    /// The object from which to retrieve the identifier.
    /// </param>
    /// <param name="target">
    /// The retrieved <see cref="IObjectId"/> if successful; otherwise,
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the identifier is successfully retrieved; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool TryConvertGetId(object source, out IObjectId? target)
    {
        if (source is IGH_AutocadReferenceDatabaseObject ghAutocadReferenceObject)
        {
            target = ghAutocadReferenceObject.Reference.ObjectId;
            return true;
        }
        if (source is IDbObject autocadReferenceObject)
        {
            target = autocadReferenceObject.Id;
            return true;
        }
        target = null;
        return false;
    }

    /// <summary>
    /// Converts the specified <see cref="IDbObject"/> to the appropriate <see cref="IGH_Goo"/> type.
    /// </summary>
    public IGH_Goo? CreateGoo(IDbObject dbObject)
    {
        var cadObject = dbObject.UnwrapObject();

        switch (cadObject)
        {
            case CadEntity entity:
                {
                    var wrapper = new AutocadEntityWrapper(entity);

                    var geometricGoo = GooTypeRegistry.Instance?.CreateGeometryGoo(wrapper);

                    if (geometricGoo != null)
                    {
                        return geometricGoo;
                    }

                    if (entity is BlockReference blockReference)
                    {
                        var blockWrapper = new BlockReferenceWrapper(blockReference);

                        return new GH_AutocadBlockReference(blockWrapper);
                    }

                    break;
                }

            case LayerTableRecord layerTableRecord:
                var layerTableRecordWrapper = new AutocadLayerTableRecordWrapper(layerTableRecord);

                return new GH_AutocadLayer(layerTableRecordWrapper);

            case Layout layout:
                var layoutWrapper = new AutocadLayoutWrapper(layout);

                return new GH_AutocadLayout(layoutWrapper);

            case LinetypeTableRecord lineType:
                var lineTypeWrapper = new AutocadLinetypeTableRecord(lineType);

                return new GH_AutocadLineType(lineTypeWrapper);

            case BlockTableRecord blockTableRecord:

                var blockTableRecordWrapper = new BlockTableRecordWrapper(blockTableRecord);

                return new GH_AutocadBlockTableRecord(blockTableRecordWrapper);
        }

        return new GH_AutocadObject(dbObject);
    }
}