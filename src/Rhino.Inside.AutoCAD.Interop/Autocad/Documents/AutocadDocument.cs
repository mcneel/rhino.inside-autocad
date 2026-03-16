using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows.Threading;
using CadBlockTableRecord = Autodesk.AutoCAD.DatabaseServices.BlockTableRecord;
using CadLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;
using CadLayout = Autodesk.AutoCAD.DatabaseServices.Layout;
using CadLineType = Autodesk.AutoCAD.DatabaseServices.LinetypeTableRecord;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDocument"/>
public class AutocadDocument : AutocadWrapperBase<Document>, IAutocadDocument
{
    private readonly Document _document;

    private readonly Dispatcher _dispatcher;

    /// <summary>
    /// Accumulates document changes during command execution.
    /// </summary>
    /// <remarks>
    /// Defers <see cref="DocumentChanged"/> invocation until command completion to prevent
    /// forcing document updates mid-command, which causes viewport visibility issues.
    /// </remarks>
    private IAutocadDocumentChange _documentChange;

    /// <inheritdoc/>
    public event EventHandler<IAutocadDocumentChangeEventArgs>? DocumentChanged;

    /// <inheritdoc/>
    public ILayerRegister LayerRegister { get; }

    /// <inheritdoc/>
    public ILineTypeRegister LineTypeRegister { get; }

    /// <inheritdoc/>
    public ILayoutRegister LayoutRegister { get; }

    /// <inheritdoc/>
    public IBlockTableRecordRegister BlockTableRecordRegister { get; }

    /// <inheritdoc/>
    public IAutocadDocumentId DocumentId { get; }

    /// <inheritdoc/>
    public IDatabase Database { get; }

    /// <inheritdoc/>
    public IAutocadDocumentFileMetadata FileMetadata { get; }

    /// <inheritdoc/>
    public UnitSystem UnitSystem { get; private set; }

    /// <inheritdoc/>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AutocadDocument"/>.
    /// </summary>
    /// <param name="document">
    /// The AutoCAD <see cref="Document"/> to wrap.
    /// </param>
    /// <param name="dispatcher">
    /// The WPF <see cref="Dispatcher"/> for marshalling UI operations.
    /// </param>
    public AutocadDocument(Document document, Dispatcher dispatcher) : base(document)
    {
        _document = document;
        _document.CommandEnded += this.OnCommandEnded;
        _document.CommandCancelled += this.OnCommandEnded;

        _dispatcher = dispatcher;

        var database = document.Database;

        var documentUnits = this.ExtractUnitSystem(database.Insunits);

        var databaseWrapper = new AutocadDatabaseWrapper(database);

        this.Database = databaseWrapper;

        this.FileMetadata = new AutocadDocumentFileMetadata(document);

        this.UnitSystem = documentUnits;

        this.LayerRegister = new LayerRegister(this);

        this.LineTypeRegister = new LineTypeRegister(this);

        this.LayoutRegister = new LayoutRegister(this);

        this.BlockTableRecordRegister = new BlockTableRecordRegister(this);

        _documentChange = new AutocadDocumentChange(this);

        this.DocumentId = new AutocadDocumentId(this);

        //Delay Subscription to database events until after document Id
        // is created to ensure changes are tracked from the moment they occur.
        database.ObjectAppended += this.OnObjectAppended;
        database.ObjectModified += this.OnObjectModified;
        database.ObjectErased += this.OnObjectErased;

        this.IsReadOnly = document.IsReadOnly;
    }

    /// <summary>
    /// Converts an Autocad <see cref="UnitsValue"/> to a <see cref="UnitSystem"/>.
    /// </summary>
    private UnitSystem ExtractUnitSystem(UnitsValue unitsValue)
    {
        var unitSystemResult = Enum.TryParse(unitsValue.ToString(), out UnitSystem documentUnitSystem);

        return unitSystemResult ? documentUnitSystem : UnitSystem.Unset;

    }

    /// <summary>
    /// Handles command completion to process accumulated document changes.
    /// </summary>
    /// <remarks>
    /// Subscribed to both <see cref="Document.CommandEnded"/> and
    /// <see cref="Document.CommandCancelled"/> because modifications can occur
    /// even when a command is cancelled (e.g., copy then escape).
    /// Ignores startup application commands.
    /// </remarks>
    private void OnCommandEnded(object sender, CommandEventArgs e)
    {
        // On startup the first ending command is the application which is ignored.
        if (Enum.GetNames(typeof(ButtonApplicationId)).Any(appId => e.GlobalCommandName.Contains(appId)))
            return;

        if (_documentChange.HasChanges)
        {
            this.CheckUnits();

            this.CheckRepositories();

            this.TriggerDocumentChanged();
        }
    }

    /// <summary>
    /// Repopulates affected repositories based on accumulated changes.
    /// </summary>
    /// <remarks>
    /// Checks each register type against <see cref="_documentChange"/> and
    /// refreshes those that contain modified object types. Block changes
    /// additionally trigger a viewport regeneration.
    /// </remarks>
    private void CheckRepositories()
    {
        if (_documentChange.DoesEffectType(typeof(CadLayer)))
            this.LayerRegister.Repopulate();

        if (_documentChange.DoesEffectType(typeof(CadLayout)))
            this.LayoutRegister.Repopulate();

        if (_documentChange.DoesEffectType(typeof(CadLineType)))
            this.LineTypeRegister.Repopulate();

        if (_documentChange.DoesEffectType(typeof(CadBlockTableRecord)))
        {
            this.BlockTableRecordRegister.Repopulate();
            this.Regenerate();
        }
    }

    /// <summary>
    /// Detects and records unit system changes.
    /// </summary>
    /// <remarks>
    /// Compares current INSUNITS against cached <see cref="UnitSystem"/> and
    /// adds <see cref="ChangeType.UnitsChanged"/> to the change tracker if different.
    /// </remarks>
    private void CheckUnits()
    {
        var database = _document.Database;

        var documentUnits = this.ExtractUnitSystem(database.Insunits);

        if (this.UnitSystem != documentUnits)
        {
            this.UnitSystem = documentUnits;

            _documentChange.AddChange(ChangeType.UnitsChanged);
        }
    }

    /// <summary>
    /// Records object modification in the change tracker.
    /// </summary>
    private void OnObjectModified(object sender, ObjectEventArgs e)
    {
        _documentChange?.AddObjectChange(ChangeType.ObjectModified, new AutocadDbObjectWrapper(e.DBObject));
    }

    /// <summary>
    /// Records object creation in the change tracker.
    /// </summary>
    private void OnObjectAppended(object sender, ObjectEventArgs e)
    {
        _documentChange?.AddObjectChange(ChangeType.ObjectCreated, new AutocadDbObjectWrapper(e.DBObject));
    }

    /// <summary>
    /// Records object deletion in the change tracker.
    /// </summary>
    private void OnObjectErased(object sender, ObjectErasedEventArgs e)
    {
        _documentChange?.AddObjectChange(ChangeType.ObjectCreated, new AutocadDbObjectWrapper(e.DBObject));
    }

    /// <inheritdoc/>
    public T Transaction<T>(Func<ITransactionManager, T> function, bool abort = false)
    {
        using var documentLock = _document.LockDocument();

        var database = _document.Database;

        using var transactionManagerWrapper = new TransactionManagerWrapper(database);

        using var transaction = transactionManagerWrapper.Unwrap().StartTransaction();

        var result = function.Invoke(transactionManagerWrapper);

        if (abort)
        {
            transaction.Abort();
        }
        else
        {
            transaction.Commit();
        }
        return result;
    }

    /// <inheritdoc/>
    public async void UpdateEditorScreen()
    {
        await _dispatcher.InvokeAsync(_document.Editor.UpdateScreen, DispatcherPriority.ContextIdle);
    }

    /// <inheritdoc/>
    public async void Regenerate()
    {
        await _dispatcher.InvokeAsync(_document.Editor.Regen, DispatcherPriority.ContextIdle);
    }

    /// <summary>
    /// Raises <see cref="DocumentChanged"/> and resets the change tracker.
    /// </summary>
    private void TriggerDocumentChanged()
    {
        var eventArgs = new AutocadDocumentChangeEventArgs(_documentChange);

        DocumentChanged?.Invoke(this, eventArgs);

        _documentChange = new AutocadDocumentChange(this);
    }

    /// <inheritdoc/>
    public IAutocadDocument ShallowClone()
    {
        return new AutocadDocument(_document, _dispatcher);
    }

    /// <inheritdoc/>
    public IDbObject? GetObjectById(IObjectId objectId)
    {
        if (objectId.IsValid == false) return null;

        return this.Transaction((transactionManagerWrapper) =>
        {
            var cadObjectId = objectId.Unwrap();

            var transactionManager = transactionManagerWrapper.Unwrap();

            var dbObject = transactionManager.GetObject(cadObjectId, OpenMode.ForRead);

            return new AutocadDbObjectWrapper(dbObject);
        });
    }

    /// <inheritdoc/>
    public IDbObject? GetObjectByHandle(long handle)
    {
        return this.Database.Unwrap().TryGetObjectId(new Handle(handle), out var id) == false
            ? null
            : this.GetObjectById(new AutocadObjectIdWrapper(id));
    }

    /// <inheritdoc/>
    public void CloseDocument()
    {
        _document.CommandEnded -= this.OnCommandEnded;
        _document.CommandCancelled -= this.OnCommandEnded;
        var database = _document.Database;
        database.ObjectAppended -= this.OnObjectAppended;
        database.ObjectModified -= this.OnObjectModified;
        database.ObjectErased -= this.OnObjectErased;

        this.Database.Dispose();
        this.LayerRegister?.Dispose();

        if (_document.IsDisposed)
            return;
    }
}
