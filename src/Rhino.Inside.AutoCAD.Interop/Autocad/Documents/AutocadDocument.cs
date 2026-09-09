using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows.Threading;
using Document = Autodesk.AutoCAD.ApplicationServices.Document;
using Handle = Autodesk.AutoCAD.DatabaseServices.Handle;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutocadDocument"/>
public class AutocadDocument : AutocadWrapperBase<Document>, IAutocadDocument
{
    private readonly Document _document;

    private readonly Dispatcher _dispatcher;

    private readonly IAutocadGuard _autocadGuard = new AutocadGuard();

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
    public IAutocadDocumentId DocumentId { get; }

    /// <inheritdoc/>
    public IAutocadDatabase AutocadDatabase { get; }

    /// <inheritdoc/>
    public IAutocadDocumentFileMetadata FileMetadata { get; }

    /// <inheritdoc/>
    public UnitSystem UnitSystem { get; private set; }

    /// <inheritdoc/>
    /// <remarks>
    /// Read live from the document rather than cached at construction, so that consumers
    /// running during construction cannot observe a stale <c>false</c>.
    /// </remarks>
    public bool IsReadOnly => _document.IsReadOnly;

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

        this.AutocadDatabase = databaseWrapper;

        this.FileMetadata = new AutocadDocumentFileMetadata(document);

        this.UnitSystem = documentUnits;

        _documentChange = new AutocadDocumentChange(this);

        this.DocumentId = new AutocadDocumentId(this);

        //Delay Subscription to database events until after document Id
        // is created to ensure changes are tracked from the moment they occur.
        database.ObjectAppended += this.OnObjectAppended;
        database.ObjectModified += this.OnObjectModified;
        database.ObjectErased += this.OnObjectErased;
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
        _autocadGuard.Run(() => this.HandleCommandEnded(e), nameof(this.OnCommandEnded));
    }

    /// <summary>
    /// Processes accumulated document changes once a command completes.
    /// </summary>
    private void HandleCommandEnded(CommandEventArgs e)
    {
        // On startup the first ending command is the application which is ignored.
        if (Enum.GetNames(typeof(ButtonApplicationId)).Any(appId => e.GlobalCommandName.Contains(appId)))
            return;

        if (_documentChange.HasChanges)
        {
            this.CheckUnits();

            this.TriggerDocumentChanged();
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
        _autocadGuard.Run(() => this.RecordObjectChange(ChangeType.ObjectModified, e.DBObject),
            nameof(this.OnObjectModified));
    }

    /// <summary>
    /// Records object creation in the change tracker.
    /// </summary>
    private void OnObjectAppended(object sender, ObjectEventArgs e)
    {
        _autocadGuard.Run(() => this.RecordObjectChange(ChangeType.ObjectCreated, e.DBObject),
            nameof(this.OnObjectAppended));
    }

    /// <summary>
    /// Records object deletion in the change tracker. The event is also raised when an
    /// object is un-erased (e.g. undo of a delete), in which case the object is recorded
    /// as created.
    /// </summary>
    private void OnObjectErased(object sender, ObjectErasedEventArgs e)
    {
 	var changeType = e.Erased ? ChangeType.ObjectErased : ChangeType.ObjectCreated;


        _autocadGuard.Run(() => this.RecordObjectChange(changeType, e.DBObject),
            nameof(this.OnObjectErased));
    }

    /// <summary>
    /// Adds a database object change to the change tracker.
    /// </summary>
    /// <remarks>
    /// Reached from AutoCAD database reactors, which fire for every entity in a drawing
    /// operation, so this is the highest-frequency managed/native boundary in the wrapper.
    /// <para>
    /// The change is recorded as a <see cref="DetachedDbObject"/> rather than as a wrapper
    /// around <paramref name="dbObject"/>: AutoCAD closes that object the moment this handler
    /// returns, while the change it belongs to is not read until the command ends.
    /// </para>
    /// </remarks>
    private void RecordObjectChange(ChangeType changeType, DBObject dbObject)
    {
        var detachedDbObject = new DetachedDbObject(dbObject);

        _documentChange?.AddObjectChange(changeType, detachedDbObject);
    }

    /// <inheritdoc/>
    public IAutocadTransactionManager CreateTransactionManager()
    {
        return new AutocadTransactionManagerWrapper(_document);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The dispatched work is guarded because this method is <c>async void</c>: it has no
    /// caller to receive an exception, so one would be rethrown on the dispatcher and
    /// terminate AutoCAD.
    /// </remarks>
    public async void UpdateEditorScreen()
    {
        await _dispatcher.InvokeAsync(
            () => _autocadGuard.Run(_document.Editor.UpdateScreen, nameof(this.UpdateEditorScreen)),
            DispatcherPriority.ContextIdle);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Guarded for the same reason as <see cref="UpdateEditorScreen"/>.
    /// </remarks>
    public async void Regenerate()
    {
        await _dispatcher.InvokeAsync(
            () => _autocadGuard.Run(_document.Editor.Regen, nameof(this.Regenerate)),
            DispatcherPriority.ContextIdle);
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
    public IDbObject? GetObjectById(IObjectId objectId)
    {
        if (objectId.IsValid == false) return null;

        var transactionManagerWrapper = this.CreateTransactionManager();

        return transactionManagerWrapper.PerformTask(() =>
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
        return this.AutocadDatabase.Unwrap().TryGetObjectId(new Handle(handle), out var id) == false
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

        // Nothing to dispose: the database belongs to the document, and AutoCAD destroys it
        // when it closes the document. See AutocadDatabaseWrapper.
    }
}
