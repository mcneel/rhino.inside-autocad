using Autodesk.AutoCAD.ApplicationServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// The host <see cref="IAutocadDocument"/> <see cref="ISatelliteService"/>.
/// The application is attached to this object and persists for its lifetime.
/// </summary>
public class AutoCadInstance : IAutoCadInstance
{
    private readonly Dispatcher _dispatcher;

    private readonly DocumentCollection? _documentManager;
    //  private Document? _activeDocument;

    private readonly string _unsavedNotSupported = MessageConstants.UnsavedNotSupported;
    private readonly string _readOnlyNotSupported = MessageConstants.ReadOnlyNotSupported;
    private readonly string _fileUnitsNotSupported = MessageConstants.FileUnitsNotSupported;

    private bool _documentClosing;

    /// <inheritdoc/>
    public event EventHandler? DocumentCreated;

    /// <inheritdoc/>
    public event EventHandler? UnitsChanged;

    /// <inheritdoc/>
    public event EventHandler<IAutocadDocumentChangeEventArgs>? DocumentChanged;

    /// <inheritdoc/>
    public event EventHandler? DocumentClosingOrActivated;

    /// <inheritdoc/>
    public IValidationLogger ValidationLogger { get; }

    /// <inheritdoc/>
    public List<IAutocadDocument> Documents { get; }

    /// <inheritdoc/>
    public IAutocadDocument? ActiveDocument => this.GetActiveDocument();

    /// <summary>
    /// Constructs a new <see cref="IAutoCadInstance"/>.
    /// </summary>
    public AutoCadInstance(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;

        _documentManager = Application.DocumentManager;

        var documentFiles = new List<IAutocadDocument>();
        foreach (var documentObject in _documentManager)
        {
            if (documentObject is Document document == false) continue;

            var documentCloseAction = new DocumentCloseAction(document, _documentManager);

            var documentFile = new AutocadDocument(document, documentCloseAction, _dispatcher);

            this.SubscribeToDocumentEvents(documentFile);

            documentFiles.Add(documentFile);
        }

        _documentManager.DocumentActivated += this.OnDocumentActivated;

        this.Documents = documentFiles;

        this.ValidationLogger = new ValidationLogger();

        this.Validate(documentFiles);
    }

    /// <summary>
    /// Subscribes to the relevant document events.
    /// </summary>
    private void SubscribeToDocumentEvents(IAutocadDocument autocadDocument)
    {
        var document = autocadDocument.Unwrap();
        document.BeginDocumentClose += this.OnDocumentClosing;
        autocadDocument.DocumentChanged += this.OnDocumentChanged;
    }

    /// <summary>
    /// Unsubscribes to the relevant document events.
    /// </summary>
    private void UnsubscribeToDocumentEvents(IAutocadDocument autocadDocument)
    {
        var document = autocadDocument.Unwrap();
        document.BeginDocumentClose -= this.OnDocumentClosing;
        autocadDocument.DocumentChanged -= this.OnDocumentChanged;
    }

    /// <summary>
    /// Internal event handler which bubbles up the document modified event.
    /// </summary>
    private void OnDocumentChanged(object sender, IAutocadDocumentChangeEventArgs e)
    {
        if (e.Change.Contains(ChangeType.UnitsChanged))
        {
            this.UnitsChanged?.Invoke(this, EventArgs.Empty);

            if (e.Change.Count() == 1)
                return;
        }

        this.DocumentChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Returns the active document in the AutoCAD application.
    /// </summary>
    private IAutocadDocument? GetActiveDocument()
    {
        foreach (var autoCadDocument in this.Documents)
        {
            if (autoCadDocument.Unwrap().IsActive)
            {
                return autoCadDocument;
            }
        }
        return null;
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

        var document = e.Document;

        if (document != null)
        {
            var documentCloseAction = new DocumentCloseAction(document, _documentManager!);

            var documentFile = new AutocadDocument(document, documentCloseAction, _dispatcher);

            document.BeginDocumentClose += this.OnDocumentClosing;

            this.Documents.Add(documentFile);

            this.SubscribeToDocumentEvents(documentFile);
        }

        this.DocumentCreated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event handler which fires when the <see cref="Document.BeginDocumentClose"/>
    /// event is raised. Veto's the Raises the <see cref="DocumentClosingOrActivated"/> event.
    /// </summary>
    protected void OnDocumentClosing(object sender, DocumentBeginCloseEventArgs e)
    {
        //TODO: I dont think is needed as we have the Terminate method in the extension
        //  e.Veto();

        _documentClosing = true;

        var document = sender as Document;
        var autoCadDocument = this.Documents.FirstOrDefault(d => d.Unwrap() == document);
        if (autoCadDocument != null)
        {
            document!.BeginDocumentClose -= this.OnDocumentClosing;
            this.Documents.Remove(autoCadDocument);
        }

        this.OnDocumentClosingOrChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Validates this service by posting any known invalid states to the
    /// <see cref="ValidationLogger"/>.
    /// </summary>
    private void Validate(List<IAutocadDocument> autoCadDocuments)
    {
        foreach (var autoCadDocument in autoCadDocuments)
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
                validationLogger.AddMessage(string.Format(_fileUnitsNotSupported,
                    unitSystem));

            }
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
        _documentManager!.DocumentActivated -= this.OnDocumentActivated;

        foreach (var autoCadDocument in this.Documents)
        {
            this.UnsubscribeToDocumentEvents(autoCadDocument);

        }

        //  this.TagDatabaseManager!.CommitAll();
        //   this.DataTagDatabaseManager!.CommitAll();
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        if (this.Documents.Any())
        {
            _documentManager!.DocumentActivated -= this.OnDocumentActivated;

            foreach (var autoCadDocument in this.Documents)
            {
                this.UnsubscribeToDocumentEvents(autoCadDocument);

                autoCadDocument.Close();
            }

            // this.TagDatabaseManager?.CommitAll();
            // this.DataTagDatabaseManager?.CommitAll();

        }
    }
}