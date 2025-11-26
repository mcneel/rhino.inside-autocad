using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A class which provides a wrapped version of the AutoCAD
/// <see cref="Document"/>.
/// </summary>
public class DocumentFile : WrapperBase<Document>, IDocument
{
    private readonly string _applicationName = InteropConstants.ApplicationName;

    private readonly DwgVersion _dwgVersion = DwgVersion.Current;

    private readonly Document _document;
    private readonly Database _database;

    private readonly IDocumentCloseAction _documentCloseAction;
    private readonly Dispatcher _dispatcher;

    private const UnitSystem _internalUnitSystem = InteropConstants.InternalUnitSystem;
    private const GroupCodeValue _applicationNameKey = DataTagKeys.ApplicationNameKey;
    private const GroupCodeValue _documentIdKey = DataTagKeys.DocumentIdKey;

    /// <summary>
    /// The string representation of the <see cref="ButtonApplicationId"/> that is
    /// currently running in this <see cref="IDocument"/>.
    /// </summary>
    private readonly string _appId;

    /// <summary>
    /// Used as a flag for document changed events. Used to defer the invocation of the
    /// <see cref="DocumentChanged"/> event otherwise the application forces the
    /// underlying AutoCAD document to update while a command is executing which causes
    /// peculiar behavior such as losing visibility of all entities in the active
    /// viewport.
    /// </summary>
    private bool _documentChanged;

    /// <inheritdoc/>
    public event EventHandler? DocumentChanged;

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public IUnitSystemManager UnitSystemManager { get; }

    /// <inheritdoc/>
    public IDatabase Database { get; }

    /// <inheritdoc/>
    public IDocumentFileInfo FileInfo { get; }

    /*   /// <inheritdoc/>
       public ILinePatternCache LinePatternCache { get; }

       /// <inheritdoc/>
       public ILayerRepository LayerRepository { get; }

       /// <inheritdoc/>
       public ILayoutRepository LayoutRepository { get; }

       /// <inheritdoc/>
       public IBlockTableRecordRepository BlockTableRecordRepository { get; }

       /// <inheritdoc/>
       public IPlotSettingsRepository PlotSettingsRepository { get; }

       /// <inheritdoc/>
       public IDimensionStyleTableRecordRepository DimensionStyleTableRecordRepository { get; }

       /// <inheritdoc/>
       public ILeaderStyleObjectRepository LeaderStyleObjectRepository { get; }

       /// <inheritdoc/>
       public ITextStyleTableRecordRepository TextStyleTableRecordRepository { get; }*/

    /// <inheritdoc/>
    public UnitSystem UnitSystem { get; }

    /// <inheritdoc/>
    public bool IsSaveOnClose { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="DocumentFile"/>.
    /// </summary>
    public DocumentFile(Document document,
        IDocumentCloseAction documentCloseAction,
        Dispatcher dispatcher,
        ButtonApplicationId appId) : base(document)
    {
        _document = document;

        _database = document.Database;

        _dispatcher = dispatcher;

        _document.CommandEnded += this.OnDocumentCommandEnded;
        _document.CommandCancelled += this.OnDocumentCommandEnded;

        _database.ObjectAppended += this.OnDatabaseObjectModifiedOrAppended;
        _database.ObjectModified += this.OnDatabaseObjectModifiedOrAppended;
        _database.ObjectErased += this.OnDatabaseObjectErased;

        _documentCloseAction = documentCloseAction;

        _appId = appId.ToString();

        var id = this.RegisterApplication(document);

        var documentUnits = _database.Insunits.ToUnitSystem();

        var unitsSystemManager = new UnitSystemManager(documentUnits, _internalUnitSystem);

        //  RhinoGeometryConverter.Initialize(unitsSystemManager);
        //  InternalGeometryConverter.Initialize(unitsSystemManager);

        var database = new DatabaseWrapper(_database);

        //  var linePatternCache = new LinePatternCache(document, unitsSystemManager);

        this.Id = id;

        this.UnitSystemManager = unitsSystemManager;

        this.Database = database;

        this.FileInfo = new DocumentFileInfo(document, id);

        /*  this.LinePatternCache = linePatternCache;

          this.LayerRepository = new LayerRepository(document, linePatternCache);

          this.LayoutRepository = new LayoutRepository(document);

          this.BlockTableRecordRepository = new BlockTableRecordRepository(document);

          this.PlotSettingsRepository = new PlotSettingsRepository(document);

          this.DimensionStyleTableRecordRepository = new DimensionStyleTableRecordRepository(document);

          this.TextStyleTableRecordRepository = new TextStyleTableRecordRepository(document);

          this.LeaderStyleObjectRepository = new LeaderStyleObjectRepository(document);*/

        this.UnitSystem = documentUnits;
    }

    /// <summary>
    /// Saves the <see cref="Database"/>. The underlying AutoCAD
    /// document remains open. When a new document GUID is created,
    /// we force a save of the document to ensure the ID XData is
    /// preserved for all future interactions of the app and document.
    /// </summary>
    private void ForceSaveDatabase(Database database)
    {
        if (_document.IsReadOnly) return;

        var filePath = database.Filename;

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

        var blockModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(_database);

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

            xData.Add(new TypedValue((short)_applicationNameKey, _applicationName));
            xData.Add(new TypedValue(idKey, documentId.ToString()));

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
    /// <see cref="_documentChanged"/> flag is true, <see cref="DocumentChanged"/>
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
        if (e.GlobalCommandName.Contains(_appId))
            return;

        if (_documentChanged)
        {
            _documentChanged = false;

            this.OnDocumentChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Event handler which fires when an object is appended or modified in the database.
    /// Sets the <see cref="_documentChanged"/> to true. 
    /// </summary>
    private void OnDatabaseObjectModifiedOrAppended(object sender, ObjectEventArgs e)
    {
        _documentChanged = true;
    }

    /// <summary>
    /// Event handler which fires when an object is erased from the database.
    /// Sets the <see cref="_documentChanged"/> to true. 
    /// </summary>
    private void OnDatabaseObjectErased(object sender, ObjectErasedEventArgs e)
    {
        _documentChanged = true;
    }

    /// <summary>
    /// Sets the <see cref="CloseActionType"/> of this <see cref="IDocument"/>,
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

        using var transactionManagerWrapper = new TransactionManagerWrapper(_database);

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
    protected virtual void OnDocumentChanged(EventArgs e)
    {
        DocumentChanged?.Invoke(this, e);
    }

    /// <inheritdoc/>
    public void Close()
    {
        _document.CommandEnded -= this.OnDocumentCommandEnded;
        _document.CommandCancelled -= this.OnDocumentCommandEnded;
        _database.ObjectAppended -= this.OnDatabaseObjectModifiedOrAppended;
        _database.ObjectModified -= this.OnDatabaseObjectModifiedOrAppended;
        _database.ObjectErased -= this.OnDatabaseObjectErased;

        _documentCloseAction.Invoke(this.FileInfo);

        this.Database.Dispose();

        /**     this.LinePatternCache.Dispose();

             this.LayerRepository.Dispose();
             this.LayoutRepository.Dispose();
             this.BlockTableRecordRepository.Dispose();
             this.PlotSettingsRepository.Dispose();
             this.DimensionStyleTableRecordRepository.Dispose();
             this.LeaderStyleObjectRepository.Dispose();
             this.TextStyleTableRecordRepository.Dispose();*/
        if (_document.IsDisposed)
            return;

        _documentCloseAction.Invoke(this.FileInfo);
    }
}