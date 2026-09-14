using Autodesk.AutoCAD.DatabaseServices;
using Rhino.DocObjects;
using Rhino.FileIO;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using RhinoLayer = Rhino.DocObjects.Layer;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IBrepConverterRunner"/>
public class BrepConverterRunner : IBrepConverterRunner
{
    private const string _rhinoInsideAutocadFolder = InteropConstants.RhinoInsideAutocadFolder;
    private const string _convertersFolder = InteropConstants.ConvertersFolder;
    private const string _rhinoFileName = InteropConstants.RhinoFileName;
    private const string _importCommand = InteropConstants.ImportCommand;
    private const string _brepConversionErrorMessage = InteropConstants.BrepConversionErrorMessage;
    private const string _batchLayerPrefix = InteropConstants.BrepBatchLayerPrefix;

    /// <summary>
    /// The linetype scale a newly constructed AutoCAD entity has, restored on imported
    /// entities when the bake settings do not specify one.
    /// </summary>
    private const double DefaultLinetypeScale = 1.0;

    /// <summary>
    /// Queue of pending <see cref="IBrepConverterRequest"/> items awaiting conversion.
    /// </summary>
    private readonly Queue<IBrepConverterRequest> _requests = new Queue<IBrepConverterRequest>();

    /// <summary>
    /// Converts a batch of Rhino Brep geometries to AutoCAD <see cref="Solid3d"/> objects
    /// using a single export/import round-trip.
    /// </summary>
    /// <param name="requests">
    /// The conversion requests containing the Brep geometries and callback delegates.
    /// </param>
    /// <returns>
    /// One <see cref="IBrepConverterResult"/> per request, in request order. Requests
    /// whose Brep failed to convert receive an empty result.
    /// </returns>
    /// <remarks>
    /// All Breps are written to a single temporary .3dm file and brought into the active
    /// document with one AutoCAD IMPORT command. To keep track of which imported entity
    /// belongs to which request, each Brep is written to a uniquely named temporary layer
    /// encoding the request index. After the import the entities are matched back to their
    /// requests via their layer name, reassigned to their target layer (from the request's
    /// bake settings, or layer 0), and the temporary layers are purged from the document.
    /// A single Brep may produce multiple solids depending on the import processing.
    /// </remarks>
    /// <seealso cref="IBrepConverterRequest"/>
    /// <seealso cref="IBrepConverterResult"/>
    private IReadOnlyList<IBrepConverterResult> ConvertBatch(IReadOnlyList<IBrepConverterRequest> requests)
    {
        var activeDocument = Application.DocumentManager.MdiActiveDocument;

        var editor = activeDocument.Editor;

        var database = activeDocument.Database;

        var results = new IBrepConverterResult[requests.Count];

        for (var index = 0; index < results.Length; index++)
        {
            results[index] = new BrepConverterResult(new List<Solid3d>());
        }

        var batchId = Guid.NewGuid().ToString("N").Substring(0, 8);

        var layerPrefix = $"{_batchLayerPrefix}{batchId}_";

        var rhinoFilePath = string.Empty;

        // Collects every object the IMPORT command appends to the database. This is more
        // reliable than SelectLast, which does not guarantee returning all entities
        // created by a multi-object import.
        var appendedObjectIds = new List<ObjectId>();

        void OnObjectAppended(object sender, ObjectEventArgs eventArgs)
        {
            appendedObjectIds.Add(eventArgs.DBObject.ObjectId);
        }

        try
        {
            var tempDirectory = Path.GetTempPath();

            var converterDirectory = Path.Combine(tempDirectory, _rhinoInsideAutocadFolder, _convertersFolder);

            Directory.CreateDirectory(converterDirectory);

            var fileNameRoot = Path.GetFileNameWithoutExtension(_rhinoFileName);

            var fileExtension = Path.GetExtension(_rhinoFileName);

            rhinoFilePath = Path.Combine(converterDirectory, $"{fileNameRoot}_{batchId}{fileExtension}");

            var file3dm = new File3dm();

            for (var index = 0; index < requests.Count; index++)
            {
                var layer = new RhinoLayer { Name = $"{layerPrefix}{index}" };

                file3dm.AllLayers.Add(layer);

                var attributes = new ObjectAttributes { LayerIndex = index };

                file3dm.Objects.AddBrep(requests[index].BrepToConvert, attributes);
            }

            var writeSucceeded = file3dm.Write(rhinoFilePath, 0);

            if (!File.Exists(rhinoFilePath) || !writeSucceeded)
            {
                return results;
            }

            database.ObjectAppended += OnObjectAppended;

            try
            {
                editor.Command(_importCommand, rhinoFilePath, "");
            }
            finally
            {
                database.ObjectAppended -= OnObjectAppended;
            }

            using var transaction = database.TransactionManager.StartTransaction();

            var solidsPerRequest = new List<Solid3d>[requests.Count];

            // Every entity matched back to a request, solid or not. Requests that target a
            // block definition hand their whole import to that record, not just the solids.
            var entityIdsPerRequest = new ObjectIdCollection[requests.Count];

            for (var index = 0; index < solidsPerRequest.Length; index++)
            {
                solidsPerRequest[index] = new List<Solid3d>();

                entityIdsPerRequest[index] = new ObjectIdCollection();
            }

            foreach (var objectId in appendedObjectIds)
            {
                if (!objectId.IsValid || objectId.IsErased) continue;

                if (transaction.GetObject(objectId, OpenMode.ForRead) is not Entity entity) continue;

                var layerName = entity.Layer;

                if (!layerName.StartsWith(layerPrefix, StringComparison.Ordinal)) continue;

                var requestIndexText = layerName.Substring(layerPrefix.Length);

                if (!int.TryParse(requestIndexText, out var requestIndex)) continue;

                if (requestIndex < 0 || requestIndex >= requests.Count) continue;

                entity.UpgradeOpen();

                ApplySettings(requests[requestIndex].Settings, entity, database);

                entityIdsPerRequest[requestIndex].Add(objectId);

                // Non-solid imports (e.g. open Breps arriving as surfaces) remain in the
                // drawing on their target layer but are not reported back to the request.
                if (entity is Solid3d solid)
                {
                    solidsPerRequest[requestIndex].Add(solid);
                }
            }

            foreach (var message in TransferOwnershipToTargetRecords(transaction, requests, entityIdsPerRequest, database))
            {
                editor.WriteMessage(message);
            }

            PurgeBatchLayers(transaction, appendedObjectIds, layerPrefix);

            // Build the results while the entities are still open in the transaction,
            // as the wrappers capture entity state (e.g. layer name) on construction.
            for (var index = 0; index < requests.Count; index++)
            {
                results[index] = new BrepConverterResult(solidsPerRequest[index]);
            }

            transaction.Commit();
        }
        catch (System.Exception ex)
        {
            editor.WriteMessage($"{_brepConversionErrorMessage}{ex.Message}");
        }
        finally
        {
            TryDeleteFile(rhinoFilePath);
        }

        return results;
    }

    /// <summary>
    /// Hands the entities imported for each request over to the block table record that
    /// request targets, leaving requests without a target in model space.
    /// </summary>
    /// <remarks>
    /// Ownership is transferred rather than the entities being cloned and erased, so the
    /// ObjectIds reported back through <see cref="IBrepConverterResult"/> stay valid and
    /// the temporary layer purge that follows is unaffected. Any existing references to a
    /// target block are marked as modified so they redraw with the new geometry. A failure
    /// on one request is reported and skipped so it cannot abort the rest of the batch.
    /// </remarks>
    private static IReadOnlyList<string> TransferOwnershipToTargetRecords(Transaction transaction,
        IReadOnlyList<IBrepConverterRequest> requests, IReadOnlyList<ObjectIdCollection> entityIdsPerRequest,
        Database database)
    {
        var messages = new List<string>();

        BlockTableRecord? modelSpace = null;

        for (var index = 0; index < requests.Count; index++)
        {
            var targetId = requests[index].TargetBlockTableRecordId;

            if (targetId is null || targetId.IsValid == false) continue;

            var entityIds = entityIdsPerRequest[index];

            if (entityIds.Count == 0) continue;

            try
            {
                if (transaction.GetObject(targetId.Unwrap(), OpenMode.ForWrite) is not BlockTableRecord targetRecord)
                    continue;

                // The entities were appended to model space by the import, so their current
                // owner has to be open for write before ownership can be taken from it.
                modelSpace ??= (BlockTableRecord)transaction.GetObject(
                    SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite);

                targetRecord.AssumeOwnershipOf(entityIds);

                foreach (ObjectId blockReferenceId in targetRecord.GetBlockReferenceIds(true, false))
                {
                    if (transaction.GetObject(blockReferenceId, OpenMode.ForWrite) is BlockReference blockReference)
                        blockReference.RecordGraphicsModified(true);
                }
            }
            catch (System.Exception ex)
            {
                messages.Add($"{_brepConversionErrorMessage}{ex.Message}");
            }
        }

        return messages;
    }

    /// <summary>
    /// Applies the request's bake settings to a converted entity.
    /// </summary>
    /// <remarks>
    /// Every property is assigned, not just the ones the settings name. The other bakeable
    /// types build their entities from scratch, so anything the settings leave out starts
    /// out as AutoCAD's default - layer 0, ByLayer colour and linetype. An imported solid
    /// instead arrives carrying whatever the IMPORT stamped on it, usually an explicit
    /// colour brought across from Rhino, so the defaults have to be restored explicitly for
    /// it to match the geometry baked alongside it. This matters most inside a block
    /// definition, where ByLayer entities take on the block reference's layer and colour and
    /// an explicitly coloured one does not. Moving the entity off the temporary import layer
    /// is also what frees that layer for purging.
    /// </remarks>
    private static void ApplySettings(IBakeSettings? settings, Entity entity, Database database)
    {
        entity.LayerId = settings?.Layer != null
            ? settings.Layer.Id.Unwrap()
            : database.LayerZero;

        entity.LinetypeId = settings?.LineType != null
            ? settings.LineType.Id.Unwrap()
            : database.ByLayerLinetype;

        entity.Color = settings?.Color != null
            ? settings.Color.Unwrap()
            : AutocadColorWrapper.CreateByLayer().Unwrap();

        entity.LinetypeScale = settings?.LinetypeScale ?? DefaultLinetypeScale;
    }

    /// <summary>
    /// Erases the temporary layers created by the batched import. All imported entities
    /// have been reassigned to their target layers by this point, so the temporary
    /// layers are unreferenced and safe to erase. Only layer records appended to the
    /// database during the import are considered, so a pre-existing user layer can
    /// never be erased, even if its name happens to match a batch layer name.
    /// </summary>
    private static void PurgeBatchLayers(Transaction transaction, IReadOnlyList<ObjectId> appendedObjectIds, string layerPrefix)
    {
        foreach (var objectId in appendedObjectIds)
        {
            if (!objectId.IsValid || objectId.IsErased) continue;

            if (transaction.GetObject(objectId, OpenMode.ForRead) is not LayerTableRecord layerRecord) continue;

            if (!layerRecord.Name.StartsWith(layerPrefix, StringComparison.Ordinal)) continue;

            layerRecord.UpgradeOpen();

            layerRecord.Erase();
        }
    }

    /// <summary>
    /// Deletes the temporary conversion file, ignoring failures such as the file being
    /// locked or already removed.
    /// </summary>
    private static void TryDeleteFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;

        try
        {
            File.Delete(filePath);
        }
        catch (System.Exception)
        {
            // Best effort clean-up; a leftover temp file is harmless.
        }
    }

    /// <inheritdoc />
    public void Run()
    {
        if (_requests.Count == 0) return;

        var batchedRequests = new List<IBrepConverterRequest>(_requests.Count);

        while (_requests.Count > 0)
        {
            batchedRequests.Add(_requests.Dequeue());
        }

        var results = this.ConvertBatch(batchedRequests);

        for (var index = 0; index < batchedRequests.Count; index++)
        {
            _ = batchedRequests[index].Callback.Invoke(results[index]);
        }
    }

    /// <inheritdoc />
    public void EnqueueRequest(IBrepConverterRequest request)
    {
        _requests.Enqueue(request);
    }
}
