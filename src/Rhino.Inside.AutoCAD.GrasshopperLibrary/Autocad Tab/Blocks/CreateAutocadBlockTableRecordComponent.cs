using Autodesk.AutoCAD.DatabaseServices;
using Grasshopper.Kernel;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using System.Collections;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Exception = System.Exception;
using RhinoPoint3d = Rhino.Geometry.Point3d;

namespace Rhino.Inside.AutoCAD.GrasshopperLibrary;

/// <summary>
/// A Grasshopper component that creates an AutoCAD Block Table Record (a block definition)
/// from Rhino and AutoCAD geometry. The definition is written straight into the document's
/// Block Table and can be fed to the Create AutoCAD Block Reference component to place
/// instances of it.
/// </summary>
/// <remarks>
/// Geometry is accepted through the same pipeline the Bake components use, so Rhino
/// geometry and native AutoCAD entities can be mixed freely on the one input. The component
/// owns exactly one block definition: while its inputs change it redefines that same record
/// in place (renaming it if the Name input changes), so existing Block References survive
/// and pick up the new geometry rather than being orphaned.
/// </remarks>
[ComponentVersion(introduced: "1.3.3")]
public class CreateAutocadBlockTableRecordComponent : RhinoInsideAutocad_CreateComponentBase, IBakingComponent
{
    /// <inheritdoc />
    public override Guid ComponentGuid => new Guid("3F6B1C24-7A58-4E0D-9B33-5C8E2A17D94B");

    /// <inheritdoc />
    public override GH_Exposure Exposure => GH_Exposure.primary;

    /// <inheritdoc />
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateAutocadBlockTableRecordComponent;

    /// <inheritdoc />
    public int OutputParamTargetIndex => 0;

    // Number of asynchronous brep conversions still in flight from the last solve. Breps
    // are imported by the BrepConverterRunner after this component's transaction has
    // committed, so their solids arrive in the definition later; this suppresses duplicate
    // enqueues from event driven re-solves while they are outstanding.
    private int _pendingAsyncBrepCount;
    private string? _lastBuiltSignature;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAutocadBlockTableRecordComponent"/> class.
    /// </summary>
    public CreateAutocadBlockTableRecordComponent()
        : base("Create AutoCAD Block Table Record", "AC-BlkDef",
            "Creates an AutoCAD Block Table Record (a block definition) from Rhino or AutoCAD geometry. " +
            "The definition is added to the document's Block Table and can be placed with the Create AutoCAD Block Reference component. " +
            "Changing the inputs redefines the same block in place, so existing Block References update rather than being lost.",
            "AutoCAD", "Blocks")
    {
    }

    /// <inheritdoc />
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadDocument(GH_ParamAccess.item), "Document",
            "Doc", "The AutoCAD Document to create the Block Definition in. If not provided, the active document will be used.",
            GH_ParamAccess.item);
        pManager[0].Optional = true;

        pManager.AddTextParameter("Name", "N",
            "The name of the Block Definition", GH_ParamAccess.item);

        pManager.AddGenericParameter("Geometry", "G",
            "The geometry to place inside the Block Definition. Accepts both Rhino geometry (curves, points, meshes, SubDs, breps, hatches, text) and native AutoCAD entities",
            GH_ParamAccess.list);

        pManager.AddPointParameter("BasePoint", "Pt",
            "The base point of the Block Definition, as a Rhino Point. Block References align this point with their insertion point. Defaults to the origin",
            GH_ParamAccess.item, RhinoPoint3d.Origin);
        pManager[3].Optional = true;

        pManager.AddParameter(new Param_BakeSettings(GH_ParamAccess.item), "Settings",
            "S", "Optional bake settings (layer, linetype, color) applied to the geometry inside the Block Definition",
            GH_ParamAccess.item);
        pManager[4].Optional = true;

        pManager.AddTextParameter("Description", "D",
            "An optional description for the Block Definition", GH_ParamAccess.item);
        pManager[5].Optional = true;
    }

    /// <inheritdoc />
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new Param_AutocadBlockTableRecord(GH_ParamAccess.item), "BlockDefinition",
            "BlockDef", "The created AutoCAD Block Definition", GH_ParamAccess.item);
    }

    /// <inheritdoc />
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        // Skip solve if undo/redo deferral is active (see base class documentation)
        if (this.ShouldSkipSolve())
            return;

        AutocadDocument? autocadDocument = null;
        DA.GetData(0, ref autocadDocument);

        var document = this.GetDocumentOrDefault(autocadDocument);

        if (document is null)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No active AutoCAD document available");
            return;
        }

        var name = string.Empty;
        if (!DA.GetData(1, ref name))
            return;

        if (this.TryValidateBlockName(ref name) == false)
            return;

        var objects = new List<object>();
        if (!DA.GetDataList(2, objects) || objects.Count == 0)
            return;

        var basePoint = RhinoPoint3d.Origin;
        DA.GetData(3, ref basePoint);

        GH_BakeSettings? settingsGoo = null;
        DA.GetData(4, ref settingsGoo);

        var settings = settingsGoo?.Value;

        var description = string.Empty;
        DA.GetData(5, ref description);

        var bakeables = this.ExtractBakeables(objects);

        if (bakeables.Count == 0)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No bakeable geometry provided");
            return;
        }

        var signature = this.BuildInputSignature(name, bakeables, basePoint, settings, description, document);

        // Check for reuse to prevent infinite loops (creation -> AutoCAD change events ->
        // downstream Get components expire -> this component expires again)
        if (this.TryReuseLastCreated(signature))
        {
            var trackedRecords = this.RetrieveAllTrackedObjects<BlockTableRecord>(document);

            if (trackedRecords.Count > 0)
            {
                DA.SetData(0, new GH_AutocadBlockTableRecord(new AutocadBlockTableRecordWrapper(trackedRecords[0])));
                return;
            }

            // Fall through to create if retrieval failed
        }

        // Asynchronous brep conversions from the last solve may still be in flight; don't
        // enqueue duplicates, and don't erase the definition's contents out from under them.
        if (_pendingAsyncBrepCount > 0 && signature == _lastBuiltSignature)
            return;

        GH_AutocadBlockTableRecord? blockDefinition = null;

        var redefined = false;

        var transactionManagerWrapper = document.CreateTransactionManager();

        _ = transactionManagerWrapper.PerformTask(() =>
        {
            var transaction = transactionManagerWrapper.Unwrap();

            var blockTable = (BlockTable)transaction.GetObject(
                transactionManagerWrapper.BlockTableId.Unwrap(), OpenMode.ForRead);

            var trackedRecordId = this.GetLiveTrackedRecordId();

            var hasTrackedRecord = trackedRecordId.IsNull == false;

            if (blockTable.Has(name) && (hasTrackedRecord == false || blockTable[name] != trackedRecordId))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    $"A Block Definition named '{name}' already exists in this drawing and was not created by this component");

                return false;
            }

            _lastBuiltSignature = signature;
            _pendingAsyncBrepCount = bakeables.Count(bakeable => bakeable is GH_AutocadBrepProxy);

            BlockTableRecord blockTableRecord;

            if (hasTrackedRecord)
            {
                // Redefine the record this component already owns, so its existing Block
                // References keep working and simply pick up the new geometry.
                blockTableRecord = (BlockTableRecord)transaction.GetObject(trackedRecordId, OpenMode.ForWrite);

                if (blockTableRecord.Name != name)
                    blockTableRecord.Name = name;

                EraseContents(transaction, blockTableRecord);

                redefined = true;
            }
            else
            {
                blockTableRecord = new BlockTableRecord { Name = name };

                blockTable.UpgradeOpen();

                blockTable.Add(blockTableRecord);

                transaction.AddNewlyCreatedDBObject(blockTableRecord, true);

                // Track created object for redefine-on-recompute functionality
                this.TrackCreatedObject(blockTableRecord.ObjectId, document);
            }

            // Origin is AutoCAD's own base point: the insertion point of a Block Reference
            // is aligned to it, so the geometry does not need re-basing to the world origin.
            blockTableRecord.Origin = basePoint.ToAutocadPoint3d();

            blockTableRecord.Comments = description ?? string.Empty;

            var target = new AutocadBlockTableRecordWrapper(blockTableRecord);

            foreach (var bakeable in bakeables)
            {
                try
                {
                    bakeable.BakeToAutocad(transactionManagerWrapper, this, settings, target);
                }
                catch (Exception ex)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                        $"Failed to add geometry to the Block Definition: {ex.Message}");
                }
            }

            if (redefined)
                MarkReferencesModified(transaction, blockTableRecord);

            // Wrapped after the geometry has been appended: the wrapper snapshots the
            // record's contents on construction.
            blockDefinition = new GH_AutocadBlockTableRecord(new AutocadBlockTableRecordWrapper(blockTableRecord));

            return true;
        });

        if (blockDefinition is null)
            return;

        if (redefined)
            TryRegen();

        DA.SetData(0, blockDefinition);
    }

    /// <summary>
    /// Returns the ObjectId of the block definition this component owns, or
    /// <see cref="ObjectId.Null"/> when it owns none. Tracked ids that no longer resolve
    /// are skipped: a definition erased in AutoCAD leaves its id behind in the tracking
    /// list, and treating that as the owned record would collide with the replacement.
    /// </summary>
    private ObjectId GetLiveTrackedRecordId()
    {
        foreach (var trackedId in this.GetTrackedObjectIds())
        {
            var objectId = trackedId.Unwrap();

            if (objectId.IsNull == false && objectId.IsEffectivelyErased == false)
                return objectId;
        }

        return ObjectId.Null;
    }

    /// <summary>
    /// Trims the block name and checks it against AutoCAD's symbol name rules, reporting
    /// an error on the component when it is unusable.
    /// </summary>
    /// <returns><c>true</c> when the name is valid; otherwise <c>false</c>.</returns>
    private bool TryValidateBlockName(ref string name)
    {
        name = name?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(name))
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A Block Definition name is required");
            return false;
        }

        try
        {
            SymbolUtilityServices.ValidateSymbolName(name, false);
        }
        catch (Exception)
        {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                $"'{name}' is not a valid Block Definition name");

            return false;
        }

        return true;
    }

    /// <summary>
    /// Normalizes the raw Grasshopper inputs into bakeables, warning about anything that
    /// cannot be converted. Shared with the bake components via <see cref="BakeableExtractor"/>,
    /// which is what lets this component accept Rhino and AutoCAD geometry on one input.
    /// </summary>
    private List<IAutocadBakeable> ExtractBakeables(List<object> objects)
    {
        var converterFactory = new RhinoConvertibleFactory();

        var bakeableExtractor = new BakeableExtractor(converterFactory);

        var bakeables = new List<IAutocadBakeable>();

        foreach (var obj in objects)
        {
            var bakeable = bakeableExtractor.ExtractBakeable(obj);

            if (bakeable != null)
            {
                bakeables.Add(bakeable);
                continue;
            }

            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                $"Object of type {obj?.GetType().Name ?? "null"} cannot be added to a Block Definition");
        }

        return bakeables;
    }

    /// <summary>
    /// Builds a signature from the name, geometry, base point, settings, description and
    /// target document, used to detect input changes between solves. Each bakeable
    /// fingerprints the same geometry it bakes, so anything affecting the definition's
    /// contents contributes to the signature.
    /// </summary>
    private string BuildInputSignature(string name, List<IAutocadBakeable> bakeables, RhinoPoint3d basePoint,
        IBakeSettings? settings, string? description, IAutocadDocument document)
    {
        var builder = new InputSignatureBuilder();

        builder.Add(name);

        foreach (var bakeable in bakeables)
        {
            bakeable.AppendInputSignature(builder);
        }

        builder.AddPoint(basePoint);

        builder.Add(settings?.Layer?.Id);
        builder.Add(settings?.LineType?.Id);
        builder.AddColor(settings?.Color);

        var linetypeScaleText = settings?.LinetypeScale?.ToString("F6");

        builder.Add(linetypeScaleText);
        builder.Add(description);
        builder.Add(document.FileMetadata.FileName);

        return builder.Build();
    }

    /// <summary>
    /// Erases every entity currently held by the block definition, ready for it to be
    /// redefined. The ids are read out first, as erasing while enumerating the record
    /// invalidates the enumerator.
    /// </summary>
    private static void EraseContents(TransactionManager transaction,
        BlockTableRecord blockTableRecord)
    {
        var entityIds = new List<ObjectId>();

        foreach (ObjectId entityId in blockTableRecord)
        {
            entityIds.Add(entityId);
        }

        foreach (var entityId in entityIds)
        {
            if (entityId.IsNull || entityId.IsEffectivelyErased)
                continue;

            var dbObject = transaction.GetObject(entityId, OpenMode.ForWrite);

            dbObject.Erase(true);
        }
    }

    /// <summary>
    /// Marks every Block Reference to the redefined block as modified so it redraws with
    /// the new geometry.
    /// </summary>
    private static void MarkReferencesModified(TransactionManager transaction,
        BlockTableRecord blockTableRecord)
    {
        foreach (ObjectId referenceId in blockTableRecord.GetBlockReferenceIds(true, false))
        {
            if (referenceId.IsNull || referenceId.IsEffectivelyErased)
                continue;

            if (transaction.GetObject(referenceId, OpenMode.ForWrite) is BlockReference blockReference)
                blockReference.RecordGraphicsModified(true);
        }
    }

    /// <summary>
    /// Regenerates the active document so redefined blocks redraw. Best effort: a regen is
    /// only a display refresh, and the editor refuses one in some contexts.
    /// </summary>
    private static void TryRegen()
    {
        try
        {
            Application.DocumentManager.MdiActiveDocument?.Editor.Regen();
        }
        catch (Exception)
        {
            // The references are already marked as modified; a failed regen only delays
            // the redraw until AutoCAD next refreshes the view.
        }
    }

    /// <inheritdoc />
    public void AddWarningMessage(string message)
    {
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, message);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Called by the brep conversion once its solids have been imported and handed to the
    /// block definition. Unlike the bake components this does not append the ids to the
    /// output, which carries the definition itself rather than a list of ObjectIds. Instead
    /// it triggers a re-solve so the output is rebuilt from the database, picking up the
    /// solids that have just arrived.
    /// </remarks>
    public bool AppendDataList(IEnumerable list)
    {
        if (_pendingAsyncBrepCount > 0)
        {
            _pendingAsyncBrepCount--;
        }

        // Only refresh the output once the last outstanding conversion has landed,
        // otherwise a multi-brep definition re-solves once per brep.
        if (_pendingAsyncBrepCount > 0)
            return true;

        this.ExpireSolution(false);

        this.OnPingDocument()?.NewSolution(false);

        return true;
    }
}
