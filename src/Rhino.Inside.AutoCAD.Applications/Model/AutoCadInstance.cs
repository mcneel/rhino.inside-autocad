using Autodesk.AutoCAD.ApplicationServices;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Interop;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Applications;

/// <summary>
/// The host <see cref="IAutoCadDocument"/> <see cref="ISatelliteService"/>.
/// The application is attached to this object and persists for its lifetime.
/// </summary>
public class AutoCadInstance : IAutoCadInstance
{
    private readonly Dispatcher _dispatcher;

    private DocumentCollection? _documentManager;
    private Document? _activeDocument;

    private readonly string _unsavedNotSupported = MessageConstants.UnsavedNotSupported;
    private readonly string _readOnlyNotSupported = MessageConstants.ReadOnlyNotSupported;
    private readonly string _fileUnitsNotSupported = MessageConstants.FileUnitsNotSupported;

    private bool _documentClosing;

    /// <inheritdoc/>
    public event EventHandler? OnDocumentCreated;

    /// <inheritdoc/>
    public event EventHandler? OnUnitsChanged;

    /// <inheritdoc/>
    public event EventHandler? DocumentClosingOrActivated;

    /// <inheritdoc/>
    public IValidationLogger ValidationLogger { get; }

    /// <inheritdoc/>
    public bool IsValid => this.ValidationLogger.HasValidationErrors == false;

    /// <inheritdoc/>
    public IAutoCadDocument Document { get; }

    /// <inheritdoc/>
    public IObjectIdTagDatabaseManager TagDatabaseManager { get; }

    /// <inheritdoc/>
    public IDataTagDatabaseManager DataTagDatabaseManager { get; }

    /// <inheritdoc/>
    public ITransientManager TransientManager { get; }

    /// <summary>
    /// Constructs a new <see cref="InteropService"/>.
    /// </summary>
    public AutoCadInstance(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;

        _documentManager = Application.DocumentManager;

        _activeDocument = _documentManager.MdiActiveDocument;

        var documentCloseAction = new DocumentCloseAction(_activeDocument, _documentManager);

        var document = new AutocadDocumentFile(_activeDocument, documentCloseAction, _dispatcher);

        _activeDocument.BeginDocumentClose += this.OnDocumentClosing;
        _documentManager.DocumentActivated += this.OnDocumentActivated;
        document.OnUnitsChanged += this.DocumentUnitsChanged;

        this.Document = document;

        this.TagDatabaseManager = new ObjectIdTagDatabaseManager(document);

        this.DataTagDatabaseManager = new DataTagDatabaseManager(document);

        this.TransientManager = new TransientManagerWrapper(Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager);

        this.ValidationLogger = new ValidationLogger();

        this.Validate(document);
    }

    /// <summary>
    /// Bubble up the units changed event from the document.
    /// </summary>
    private void DocumentUnitsChanged(object sender, EventArgs e)
    {
        this.OnUnitsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event handler which fires when the <see cref=" DocumentCollection.DocumentActivated"/>
    /// is raised. Raises the <see cref="DocumentClosingOrActivated"/> event. If the document
    /// is closing, the event is not raised.
    /// </summary>
    protected void OnDocumentActivated(object sender, DocumentCollectionEventArgs e)
    {
        if (_documentClosing == false)
            this.OnDocumentClosingOrChanged(EventArgs.Empty);

        this.OnDocumentCreated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event handler which fires when the <see cref="Document.BeginDocumentClose"/>
    /// event is raised. Veto's the Raises the <see cref="DocumentClosingOrActivated"/> event.
    /// </summary>
    protected void OnDocumentClosing(object sender, DocumentBeginCloseEventArgs e)
    {
        e.Veto();

        _documentClosing = true;

        this.OnDocumentClosingOrChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Validates this service by posting any known invalid states to the
    /// <see cref="ValidationLogger"/>.
    /// </summary>
    private void Validate(IAutoCadDocument autoCadDocument)
    {
        var validationLogger = this.ValidationLogger;

        var cadDocument = autoCadDocument.Unwrap();

        // If the file is not saved, the document is not named.
        if (cadDocument.IsNamedDrawing == false)
        {
            validationLogger.AddMessage(_unsavedNotSupported);
        }

        if (cadDocument.IsReadOnly)
        {
            validationLogger.AddMessage(_readOnlyNotSupported);
        }

        var unitSystem = autoCadDocument.UnitSystem;
        if (unitSystem == UnitSystem.Unset)
        {
            validationLogger.AddMessage(string.Format(_fileUnitsNotSupported, unitSystem));

        }
    }

    /// <summary>
    /// Event handler which raises the <see cref="DocumentClosingOrActivated"/> event.
    /// </summary>
    protected virtual void OnDocumentClosingOrChanged(EventArgs e)
    {
        DocumentClosingOrActivated?.Invoke(this, e);
    }

    /// <inheritdoc/>
    protected void RestartTasks()
    {
        _activeDocument!.BeginDocumentClose -= this.OnDocumentClosing;
        _documentManager!.DocumentActivated -= this.OnDocumentActivated;
        this.Document.OnUnitsChanged -= this.DocumentUnitsChanged;

        this.TagDatabaseManager!.CommitAll();
        this.DataTagDatabaseManager!.CommitAll();
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        var document = this.Document;

        if (document != null)
        {
            _activeDocument!.BeginDocumentClose -= this.OnDocumentClosing;
            _documentManager!.DocumentActivated -= this.OnDocumentActivated;
            this.Document.OnUnitsChanged -= this.DocumentUnitsChanged;

            this.TagDatabaseManager?.CommitAll();
            this.DataTagDatabaseManager?.CommitAll();

            document.Close();
        }
    }
}