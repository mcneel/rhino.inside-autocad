using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using CadDbObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using CadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using CadLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;
using CadObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using TransactionManager = Autodesk.AutoCAD.DatabaseServices.TransactionManager;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// Provides extension methods for unwrapping interface wrappers to their underlying AutoCAD API types.
/// </summary>
/// <remarks>
/// This converter enables direct access to native AutoCAD objects when the abstraction layer
/// needs to be bypassed for advanced operations or AutoCAD API interop. All methods cast the
/// wrapper to its base type and return the internal AutoCAD object.
/// Usage: <c>var nativeDoc = myDocument.Unwrap();</c>
/// </remarks>
/// <seealso cref="WrapperBase{T}"/>
/// <seealso cref="AutocadWrapperDisposableBase{T}"/>
public static class InteropConverter
{
    /// <summary>
    /// Unwraps an <see cref="IAutocadDocument"/> to its underlying AutoCAD <see cref="Document"/>.
    /// </summary>
    /// <param name="autocadDocument">
    /// The document wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="Document"/> instance.
    /// </returns>
    public static Document Unwrap(this IAutocadDocument autocadDocument)
    {
        var documentWrapper = (AutocadWrapperBase<Document>)autocadDocument;

        return documentWrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IDatabase"/> to its underlying AutoCAD <see cref="Database"/>.
    /// </summary>
    /// <param name="database">
    /// The database wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="Database"/> instance.
    /// </returns>
    public static Database Unwrap(this IDatabase database)
    {
        var databaseWrapper = (AutocadWrapperDisposableBase<Database>)database;

        return databaseWrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IObjectId"/> to its underlying AutoCAD <see cref="CadObjectId"/>.
    /// </summary>
    /// <param name="objectId">
    /// The object ID wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="CadObjectId"/> structure.
    /// </returns>
    public static CadObjectId Unwrap(this IObjectId objectId)
    {
        var objectIdWrapper = (AutocadWrapperBase<CadObjectId>)objectId;

        return objectIdWrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IDbObject"/> to its underlying AutoCAD <see cref="CadDbObject"/>.
    /// </summary>
    /// <param name="dbObject">
    /// The database object wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="CadDbObject"/> instance.
    /// </returns>
    /// <remarks>
    /// Named <c>UnwrapObject</c> to avoid ambiguity with more specific unwrap methods
    /// for derived types like <see cref="IEntity"/> or <see cref="IAutocadLayerTableRecord"/>.
    /// </remarks>
    public static CadDbObject UnwrapObject(this IDbObject dbObject)
    {
        var dbObjectWrapper = (AutocadWrapperDisposableBase<CadDbObject>)dbObject;

        return dbObjectWrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="ITransactionManager"/> to its underlying AutoCAD <see cref="TransactionManager"/>.
    /// </summary>
    /// <param name="transactionManager">
    /// The transaction manager wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="TransactionManager"/> instance.
    /// </returns>
    public static TransactionManager Unwrap(this ITransactionManager transactionManager)
    {
        var objectIdWrapper = (AutocadWrapperDisposableBase<TransactionManager>)transactionManager;

        return objectIdWrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IEntity"/> to its underlying AutoCAD <see cref="CadEntity"/>.
    /// </summary>
    /// <param name="entity">
    /// The entity wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="CadEntity"/> instance.
    /// </returns>
    public static CadEntity Unwrap(this IEntity entity)
    {
        var wrapper = (AutocadWrapperDisposableBase<CadDbObject>)entity;

        return (CadEntity)wrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IAutocadBlockTableRecord"/> to its underlying AutoCAD <see cref="BlockTableRecord"/>.
    /// </summary>
    /// <param name="autocadBlockTableRecord">
    /// The block table record wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="BlockTableRecord"/> instance.
    /// </returns>
    public static BlockTableRecord Unwrap(this IAutocadBlockTableRecord autocadBlockTableRecord)
    {
        var wrapper = (AutocadWrapperDisposableBase<CadDbObject>)autocadBlockTableRecord;

        return (BlockTableRecord)wrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IAutocadBlockReference"/> to its underlying AutoCAD <see cref="BlockReference"/>.
    /// </summary>
    /// <param name="autocadBlockReference">
    /// The block reference wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="BlockReference"/> instance.
    /// </returns>
    public static BlockReference Unwrap(this IAutocadBlockReference autocadBlockReference)
    {
        var wrapper = (AutocadWrapperDisposableBase<CadDbObject>)autocadBlockReference;

        return (BlockReference)wrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IAutocadLayout"/> to its underlying AutoCAD <see cref="Layout"/>.
    /// </summary>
    /// <param name="layout">
    /// The layout wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="Layout"/> instance.
    /// </returns>
    public static Layout Unwrap(this IAutocadLayout layout)
    {
        var wrapper = (AutocadWrapperDisposableBase<CadDbObject>)layout;

        return (Layout)wrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IAutocadLayerTableRecord"/> to its underlying AutoCAD <see cref="CadLayer"/>.
    /// </summary>
    /// <param name="layer">
    /// The layer wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="CadLayer"/> instance.
    /// </returns>
    public static CadLayer Unwrap(this IAutocadLayerTableRecord layer)
    {
        var layerWrapper = (AutocadWrapperDisposableBase<CadDbObject>)layer;

        return (CadLayer)layerWrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IAutocadLinetypeTableRecord"/> to its underlying AutoCAD <see cref="LinetypeTableRecord"/>.
    /// </summary>
    /// <param name="autocadLinetypeTableRecord">
    /// The linetype table record wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="LinetypeTableRecord"/> instance.
    /// </returns>
    public static LinetypeTableRecord Unwrap(this IAutocadLinetypeTableRecord autocadLinetypeTableRecord)
    {
        var lineTypeTableRecord = (AutocadWrapperDisposableBase<CadDbObject>)autocadLinetypeTableRecord;

        return (LinetypeTableRecord)lineTypeTableRecord.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="IAutocadSelectionFilterWrapper"/> to its underlying AutoCAD <see cref="SelectionFilter"/>.
    /// </summary>
    /// <param name="autocadSelectionFilterWrapper">
    /// The selection filter wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="SelectionFilter"/> instance.
    /// </returns>
    public static SelectionFilter Unwrap(this IAutocadSelectionFilterWrapper autocadSelectionFilterWrapper)
    {
        var selectionWrapper = (AutocadWrapperBase<SelectionFilter>)autocadSelectionFilterWrapper;

        return selectionWrapper.AutocadObject;
    }

    /// <summary>
    /// Unwraps an <see cref="ITypedValueWrapper"/> to its underlying AutoCAD <see cref="TypedValue"/>.
    /// </summary>
    /// <param name="typedValue">
    /// The typed value wrapper to unwrap.
    /// </param>
    /// <returns>
    /// The native AutoCAD <see cref="TypedValue"/> structure.
    /// </returns>
    public static TypedValue Unwrap(this ITypedValueWrapper typedValue)
    {
        var wrapper = (AutocadWrapperBase<TypedValue>)typedValue;

        return wrapper.AutocadObject;
    }
}
