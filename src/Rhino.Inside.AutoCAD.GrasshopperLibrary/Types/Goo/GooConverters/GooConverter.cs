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
                        var blockWrapper = new AutocadBlockReferenceWrapper(blockReference);

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
                var lineTypeWrapper = new AutocadLinetypeTableRecordWrapper(lineType);

                return new GH_AutocadLineType(lineTypeWrapper);

            case BlockTableRecord blockTableRecord:

                var blockTableRecordWrapper = new AutocadBlockTableRecordWrapper(blockTableRecord);

                return new GH_AutocadBlockTableRecord(blockTableRecordWrapper);

            case DBDictionary dbDictionary:
                var dictionaryWrapper = new AutocadDictionaryWrapper(dbDictionary);

                return new GH_AutocadDictionary(dictionaryWrapper);

            case Xrecord xrecord:
                var xrecordWrapper = new XRecordWrapper(xrecord);

                return new GH_AutocadXRecord(xrecordWrapper);

            case AssocNetwork assocNetwork:
                var assocNetworkWrapper = new AssocNetworkWrapper(assocNetwork);

                return new GH_AutocadAssocNetwork(assocNetworkWrapper);
        }

        return new GH_AutocadObject(dbObject);
    }

    /// <summary>
    /// Converts a typed value to an appropriate Grasshopper Goo type.
    /// </summary>
    public IGH_Goo ConvertToGoo(object value)
    {
        return value switch
        {
            string s => new GH_String(s),
            int i => new GH_Integer(i),
            short sh => new GH_Integer(sh),
            long l => new GH_Integer((int)l),
            double d => new GH_Number(d),
            float f => new GH_Number(f),
            bool b => new GH_Boolean(b),
            Autodesk.AutoCAD.Geometry.Point3d pt => new GH_Point(pt.ToRhinoPoint3d()),
            Autodesk.AutoCAD.Geometry.Point2d pt2 => new GH_Point(pt2.ToRhinoPoint3d()),
            Autodesk.AutoCAD.Geometry.Vector3d vec => new GH_Vector(vec.ToRhinoVector3d()),
            Autodesk.AutoCAD.DatabaseServices.ObjectId objId => new GH_AutocadObjectId(new AutocadObjectIdWrapper(objId)),
            _ => new GH_ObjectWrapper(value)
        };
    }
}