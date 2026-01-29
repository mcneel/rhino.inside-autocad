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

/// <summary>
/// A class which provides a wrapped version of the AutoCAD
/// <see cref="Document"/>.
/// </summary>
public class AutocadDocument : WrapperBase<Document>, IAutocadDocument
{
    private readonly string _applicationName = InteropConstants.ApplicationName;

    private readonly DwgVersion _dwgVersion = DwgVersion.Current;

    private readonly Document _document;

    private readonly IDocumentCloseAction _documentCloseAction;
    private readonly Dispatcher _dispatcher;

    private const short _applicationNameKey = XRecordKeys.ApplicationNameKey;
    private const short _documentIdKey = XRecordKeys.DocumentIdKey;

    /// <summary>
    /// Used as a flag for document changed events. Used to defer the invocation of the
    /// <see cref="DocumentChanged"/> event otherwise the application forces the
    /// underlying AutoCAD document to update while a command is executing which causes
    /// peculiar behavior such as losing visibility of all entities in the active
    /// viewport.
    /// </summary>
    private IAutocadDocumentChange _documentChange;

    /// <inheritdoc/>
    public event EventHandler<IAutocadDocumentChangeEventArgs>? DocumentChanged;

    /// <inheritdoc/>
    public event EventHandler? OnUnitsChanged;

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public IDatabase Database { get; }

    /// <inheritdoc/>
    public IAutocadDocumentFileInfo FileInfo { get; }

    /// <inheritdoc/>
    public ILayerRepository LayerRepository { get; }

    /// <inheritdoc/>
    public ILineTypeRepository LineTypeRepository { get; }

    /// <inheritdoc/>
    public ILayoutRepository LayoutRepository { get; }

    /// <inheritdoc/>
    public IBlockTableRecordRepository BlockTableRecordRepository { get; }

    /// <inheritdoc/>
    public UnitSystem UnitSystem { get; private set; }

    /// <inheritdoc/>
    public bool IsSaveOnClose { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="AutocadDocument"/>.
    /// </summary>
    public AutocadDocument(Document document,
        IDocumentCloseAction documentCloseAction,
        Dispatcher dispatcher) : base(document)
    {
        _document = document;

        var database = document.Database;

        _dispatcher = dispatcher;

        _document.CommandEnded += this.OnDocumentCommandEnded;
        _document.CommandCancelled += this.OnDocumentCommandEnded;

        database.ObjectAppended += this.OnDatabaseObjectAppended;
        database.ObjectModified += this.OnDatabaseObjectModified;
        database.ObjectErased += this.OnDatabaseObjectErased;

        _documentCloseAction = documentCloseAction;

        var id = this.RegisterApplication(document);

        var documentUnits = database.Insunits.ToUnitSystem();

        var databaseWrapper = new DatabaseWrapper(database);

        this.Id = id;

        this.Database = databaseWrapper;

        this.FileInfo = new AutocadDocumentFileInfo(document, id);

        this.UnitSystem = documentUnits;

        this.LayerRepository = new LayerRepository(this);

        this.LineTypeRepository = new LineTypeRepository(this);

        this.LayoutRepository = new LayoutRepository(this);

        this.BlockTableRecordRepository = new BlockTableRecordRepository(this);

        _documentChange = new AutocadDocumentChange(this);
    }

    /// <summary>
    /// Saves the <see cref="Database"/>. The underlying AutoCAD
    /// document remains open. When a new document GUID is created,
    /// we force a save of the document to ensure the ID XData is
    /// preserved for all future interactions of the app and document.
    /// If there is no file path (i.e. the document has never been saved
    /// and is not a template), no save is performed.
    /// </summary>
    private void ForceSaveDatabase(Database database)
    {
        var filePath = database.Filename;

        if (_document.IsReadOnly || string.IsNullOrEmpty(filePath)) return;

        var securityParameters = database.SecurityParameters;

        database.SaveAs(filePath, true, _dwgVersion, securityParameters);
    }

    /// <summary>
    /// Returns the document ID from the model space XData. If the document ID
    /// does not exist, a new one is created and stored in the model space XData.
    /// </summary>
    /// <remarks>
    /// Only called after <see cref="RegisterApplication"/> has been called.
    /// </remarks>
    private Guid GetId(Database database)
    {
        var transactionManager = database.TransactionManager;

        using var transaction = transactionManager.StartTransaction();

        var blockModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(database);

        using var modelSpace = (BlockTableRecord)blockModelSpaceId.GetObject(OpenMode.ForRead);

        using var xData = modelSpace.XData == null ? new ResultBuffer() : modelSpace.XData;

        var idKey = (short)_documentIdKey;

        var typedValues = xData.AsArray().Where(v => v.TypeCode == idKey);

        var documentId = Guid.Empty;
        foreach (var typedValue in typedValues)
        {
            if (Guid.TryParse(typedValue.Value.ToString(), out documentId))
                break;
        }

        if (documentId == Guid.Empty)
        {
            documentId = Guid.NewGuid();

            xData.Add(new Autodesk.AutoCAD.DatabaseServices.TypedValue((short)_applicationNameKey, _applicationName));
            xData.Add(new Autodesk.AutoCAD.DatabaseServices.TypedValue(idKey, documentId.ToString()));

            modelSpace.UpgradeOpen();

            modelSpace.XData = xData;

            this.ForceSaveDatabase(database);
        }

        transaction.Commit();

        return documentId;
    }

    /// <summary>
    /// Registers this application with the <see cref="RegAppTable"/>.
    /// Required for functions such as writing XData to the database.
    /// </summary>
    private Guid RegisterApplication(Document document)
    {
        var database = document.Database;

        using var documentLock = _document.LockDocument();

        using var transaction = document.TransactionManager.StartTransaction();

        using var regAppTable = (RegAppTable)transaction.GetObject(database.RegAppTableId, OpenMode.ForRead);

        if (regAppTable.Has(_applicationName) == false)
        {
            regAppTable.UpgradeOpen();

            using var regAppTableRecord = new RegAppTableRecord();

            regAppTableRecord.Name = _applicationName;

            regAppTable.Add(regAppTableRecord);

            transaction.AddNewlyCreatedDBObject(regAppTableRecord, true);
        }

        transaction.Commit();

        return this.GetId(database);
    }

    /// <summary>
    /// Event handler which fires when a command is ended or cancelled. If the
    /// <see cref="_documentChange"/> flag is true, <see cref="DocumentChanged"/>
    /// is invoked and the flag is set back to false ready for any future changes.
    /// </summary>
    /// <remarks>
    /// It is possible to modify the document and cancel the command. For example,
    /// if an entity is copied and the escape key is pressed, the command is cancelled
    /// yet the document has changed. For this reason, both the
    /// <see cref="Document.CommandEnded"/> and <see cref="Document.CommandCancelled"/>
    /// events are subscribed to.
    /// </remarks>
    private void OnDocumentCommandEnded(object sender, CommandEventArgs e)
    {
        // On startup the first ending command is the application which is ignored.
        if (Enum.GetNames(typeof(ButtonApplicationId)).Any(appId => e.GlobalCommandName.Contains(appId)))
            return;

        this.CheckUnits();

        if (_documentChange.HasChanges)
        {
            this.CheckRepositories();

            this.TriggerDocumentChanged();
        }
    }

    /// <summary>
    /// Checks the repositories to see if they need to be updated based on the
    /// current <see cref="_documentChange"/>.
    /// </summary>
    private void CheckRepositories()
    {
        if (_documentChange.DoesEffectType(typeof(CadLayer)))
            this.LayerRepository.Repopulate();

        if (_documentChange.DoesEffectType(typeof(CadLayout)))
            this.LayoutRepository.Repopulate();

        if (_documentChange.DoesEffectType(typeof(CadLineType)))
            this.LineTypeRepository.Repopulate();

        if (_documentChange.DoesEffectType(typeof(CadBlockTableRecord)))
            this.BlockTableRecordRepository.Repopulate();
    }

    /// <summary>
    /// Checks the document units against the stored units to see if they have changed.
    /// If they have, the <see cref="OnUnitsChanged"/> event is invoked.
    /// </summary>
    private void CheckUnits()
    {
        var database = _document.Database;

        var documentUnits = database.Insunits.ToUnitSystem();

        if (this.UnitSystem != documentUnits)
        {

            this.UnitSystem = documentUnits;

            _documentChange.AddChange(ChangeType.UnitsChanged);

            OnUnitsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Event handler which fires when an object is modified in the database.
    /// Sets the <see cref="_documentChange"/> to true. 
    /// </summary>
    private void OnDatabaseObjectModified(object sender, ObjectEventArgs e)
    {
        _documentChange?.AddObjectChange(ChangeType.ObjectModified, new DbObjectWrapper(e.DBObject));
    }

    /// <summary>
    /// Event handler which fires when an object is appended  in the database.
    /// Sets the <see cref="_documentChange"/> to true. 
    /// </summary>
    private void OnDatabaseObjectAppended(object sender, ObjectEventArgs e)
    {
        _documentChange?.AddObjectChange(ChangeType.ObjectCreated, new DbObjectWrapper(e.DBObject));
    }

    /// <summary>
    /// Event handler which fires when an object is erased from the database.
    /// Sets the <see cref="_documentChange"/> to true. 
    /// </summary>
    private void OnDatabaseObjectErased(object sender, ObjectErasedEventArgs e)
    {
        _documentChange?.AddObjectChange(ChangeType.ObjectCreated, new DbObjectWrapper(e.DBObject));
    }

    /// <summary>
    /// Sets the <see cref="CloseActionType"/> of this <see cref="IAutocadDocument"/>,
    /// to <see cref="CloseActionType.Save"/> to ensure any changes made to the
    /// document are saved in the underlying AutoCAD document.
    /// </summary>
    private void SaveChanges()
    {
        this.IsSaveOnClose = true;

        _documentCloseAction.SetCloseAction(CloseActionType.Save);
    }

    /// <inheritdoc/>
    public T Transaction<T>(Func<ITransactionManager, T> function, bool saveChanges = false, bool abort = false)
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

        if (saveChanges)
            this.SaveChanges();

        return result;
    }

    /// <inheritdoc/>
    public async void UpdateScreen()
    {
        await _dispatcher.InvokeAsync(_document.Editor.UpdateScreen, DispatcherPriority.ContextIdle);
    }

    /// <inheritdoc/>
    public async void Regenerate()
    {
        await _dispatcher.InvokeAsync(_document.Editor.Regen, DispatcherPriority.ContextIdle);
    }

    /// <summary>
    /// Raises the <see cref="DocumentChanged"/> event.
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
        return new AutocadDocument(_document, _documentCloseAction, _dispatcher);
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

            return new DbObjectWrapper(dbObject);
        });
    }

    /// <inheritdoc/>
    public IDbObject? GetObjectByHandle(long handle)
    {
        return this.Database.Unwrap().TryGetObjectId(new Handle(handle), out var id) == false
            ? null
            : this.GetObjectById(new AutocadObjectId(id));
    }

    /// <inheritdoc/>
    public void Close()
    {
        _document.CommandEnded -= this.OnDocumentCommandEnded;
        _document.CommandCancelled -= this.OnDocumentCommandEnded;
        var database = _document.Database;
        database.ObjectAppended -= this.OnDatabaseObjectAppended;
        database.ObjectModified -= this.OnDatabaseObjectModified;
        database.ObjectErased -= this.OnDatabaseObjectErased;

        _documentCloseAction.Invoke(this.FileInfo);

        this.Database.Dispose();
        this.LayerRepository?.Dispose();

        if (_document.IsDisposed)
            return;

        _documentCloseAction.Invoke(this.FileInfo);
    }
}

