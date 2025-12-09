using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadDbObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using CadLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using Hatch = Autodesk.AutoCAD.DatabaseServices.Hatch;
using TransactionManager = Autodesk.AutoCAD.DatabaseServices.TransactionManager;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A converter class with extension methods to convert from
/// AutoCAD objects to internal wrapper types.
/// </summary>
public static class InteropConverter
{
    /// <summary>
    /// Unwraps the <see cref="IAutocadDocument"/> to the underlying AutoCAD
    /// <see cref="Document"/> object.
    /// </summary>
    public static Document Unwrap(this IAutocadDocument autocadDocument)
    {
        var documentWrapper = (WrapperBase<Document>)autocadDocument;

        return documentWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="IDatabase"/> to the underlying AutoCAD
    /// <see cref="Database"/> object.
    /// </summary>
    public static Database Unwrap(this IDatabase database)
    {
        var databaseWrapper = (WrapperDisposableBase<Database>)database;

        return databaseWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="IObjectId"/> and returns the <see cref="AutocadObjectId"/>.
    /// </summary>
    public static CadObjectId Unwrap(this IObjectId objectId)
    {
        var objectIdWrapper = (WrapperBase<CadObjectId>)objectId;

        return objectIdWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="IDbObject"/> and returns the <see cref="DbObjectWrapper"/>.
    /// </summary>
    public static CadDbObject UnwrapObject(this IDbObject dbObject)
    {
        var dbObjectWrapper = (WrapperDisposableBase<CadDbObject>)dbObject;

        return dbObjectWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="IAutocadLayerTableRecord"/> and returns the <see cref="CadLayer"/>.
    /// </summary>
    public static CadLayer Unwrap(this IAutocadLayerTableRecord layer)
    {
        var layerWrapper = (WrapperDisposableBase<CadLayer>)layer;

        return layerWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="ITransactionManager"/> and returns the
    /// <see cref="TransactionManager"/>.
    /// </summary>
    public static TransactionManager Unwrap(this ITransactionManager transactionManager)
    {
        var objectIdWrapper = (WrapperDisposableBase<TransactionManager>)transactionManager;

        return objectIdWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="IHatch"/> and returns the AutoCAD <see cref="Autodesk.AutoCAD.DatabaseServices.Hatch"/>.
    /// </summary>
    public static Hatch Unwrap(this IHatch hatch)
    {
        var hatchWrapper = (WrapperDisposableBase<CadDbObject>)hatch;

        return (Hatch)hatchWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="IHatchLoop"/> and returns the AutoCAD <see cref="HatchLoop"/>.
    /// </summary>
    public static HatchLoop Unwrap(this IHatchLoop hatchLoop)
    {
        var hatchLoopWrapper = (WrapperBase<HatchLoop>)hatchLoop;

        return hatchLoopWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the provided <see cref="IEntity"/> instance.
    /// </summary>
    public static CadEntity Unwrap(this IEntity entity)
    {
        var wrapper = (WrapperDisposableBase<CadDbObject>)entity;

        return (CadEntity)wrapper.Internal;
    }

    /// <summary>
    /// Unwraps the provided <see cref="IBlockTableRecord"/> instance.
    /// </summary>
    public static BlockTableRecord Unwrap(this IBlockTableRecord blockTableRecord)
    {
        var wrapper = (WrapperDisposableBase<CadDbObject>)blockTableRecord;

        return (BlockTableRecord)wrapper.Internal;
    }

    /// <summary>
    /// Unwraps the provided <see cref="IBlockReference"/> instance.
    /// </summary>
    public static BlockReference Unwrap(this IBlockReference blockReference)
    {
        var wrapper = (WrapperDisposableBase<CadDbObject>)blockReference;

        return (BlockReference)wrapper.Internal;
    }

    /// <summary>
    /// Unwraps the provided <see cref="IAutocadLayout"/> instance.
    /// </summary>
    public static Layout Unwrap(this IAutocadLayout layout)
    {
        var wrapper = (WrapperDisposableBase<CadDbObject>)layout;

        return (Layout)wrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="IPlotSettings"/> and returns the
    /// <see cref="Autodesk.AutoCAD.DatabaseServices.PlotSettings"/>.
    /// </summary>
    public static Autodesk.AutoCAD.DatabaseServices.PlotSettings Unwrap(this IPlotSettings plotSettings)
    {
        var plotSettingsWrapper = (WrapperDisposableBase<Autodesk.AutoCAD.DatabaseServices.PlotSettings>)plotSettings;

        return plotSettingsWrapper.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="INamedObjectsDictionary"/> and returns the
    /// <see cref="DBDictionary"/>.
    /// </summary>
    public static DBDictionary Unwrap(this INamedObjectsDictionary namedObjectsDictionary)
    {
        var dbDictionary = (WrapperDisposableBase<DBDictionary>)namedObjectsDictionary;

        return dbDictionary.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="INamedObjectsDictionary"/> and returns the
    /// <see cref="DBDictionary"/>.
    /// </summary>
    public static LinetypeTableRecord Unwrap(this IAutocadLinetypeTableRecord autocadLinetypeTableRecord)
    {
        var lineTypeTableRecord = (WrapperDisposableBase<LinetypeTableRecord>)autocadLinetypeTableRecord;

        return lineTypeTableRecord.Internal;
    }

    /// <summary>
    /// Unwraps the <see cref="ISelectionFilter"/> to the underlying AutoCAD
    /// <see cref="SelectionFilter"/> object.
    /// </summary>
    public static SelectionFilter Unwrap(this ISelectionFilter selectionFilter)
    {
        var selectionWrapper = (WrapperBase<SelectionFilter>)selectionFilter;

        return selectionWrapper.Internal;
    }
}