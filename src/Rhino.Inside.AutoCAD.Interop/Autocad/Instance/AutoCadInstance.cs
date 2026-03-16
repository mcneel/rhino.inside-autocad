using Autodesk.AutoCAD.ApplicationServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IAutoCadInstance"/>
public class AutoCadInstance : IAutoCadInstance
{
    private readonly Dispatcher _dispatcher;

    private readonly DocumentCollection? _documentManager;

    private readonly string _readOnlyNotSupported = MessageConstants.ReadOnlyNotSupported;
    private readonly string _fileUnitsNotSupported = MessageConstants.FileUnitsNotSupported;

    /// <inheritdoc/>
    public event EventHandler? DocumentCreated;

    /// <inheritdoc/>
    public event EventHandler? UnitsChanged;

    /// <inheritdoc/>
    public event EventHandler<IAutocadDocumentChangeEventArgs>? DocumentChanged;

    /// <inheritdoc/>
    public IStartUpLogger StartUpLogger { get; }

    /// <inheritdoc/>
    public List<IAutocadDocument> Documents { get; }

    /// <inheritdoc/>
    public IAutocadDocument? ActiveDocument => this.GetActiveDocument();

    /// <inheritdoc/>
    public Version ApplicationVersion { get; }

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

            var documentFile = new AutocadDocument(document, _dispatcher);

            this.SubscribeToDocumentEvents(documentFile);

            documentFiles.Add(documentFile);
        }

        _documentManager.DocumentActivated += this.OnDocumentActivated;

        this.Documents = documentFiles;

        this.StartUpLogger = new StartUpLogger();

        this.ApplicationVersion = Application.Version;

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
    /// is raised. Raises the <see cref="DocumentCreated"/> event.
    /// </summary>
    protected void OnDocumentActivated(object sender, DocumentCollectionEventArgs e)
    {
        var document = e.Document;

        if (document != null)
        {
            var documentFile = new AutocadDocument(document, _dispatcher);

            document.BeginDocumentClose += this.OnDocumentClosing;

            this.Documents.Add(documentFile);

            this.SubscribeToDocumentEvents(documentFile);
        }

        this.DocumentCreated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event handler which fires when the <see cref="Document.BeginDocumentClose"/>
    /// event is raised.
    /// </summary>
    protected void OnDocumentClosing(object sender, DocumentBeginCloseEventArgs e)
    {
        var document = sender as Document;

        var autoCadDocument = this.Documents.FirstOrDefault(d => d.Unwrap() == document);

        if (autoCadDocument != null)
        {
            document!.BeginDocumentClose -= this.OnDocumentClosing;
            this.Documents.Remove(autoCadDocument);
        }
    }

    /// <summary>
    /// Validates this service by posting any known invalid states to the
    /// <see cref="StartUpLogger"/>.
    /// </summary>
    private void Validate(List<IAutocadDocument> autoCadDocuments)
    {
        foreach (var autoCadDocument in autoCadDocuments)
        {
            var validationLogger = this.StartUpLogger;

            var cadDocument = autoCadDocument.Unwrap();

            if (cadDocument.IsReadOnly)
            {
                validationLogger.AddError(_readOnlyNotSupported);
            }

            var unitSystem = autoCadDocument.UnitSystem;
            if (unitSystem == UnitSystem.Unset)
            {
                validationLogger.AddError(string.Format(_fileUnitsNotSupported,
                    unitSystem));

            }
        }
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        _documentManager!.DocumentActivated -= this.OnDocumentActivated;

        if (this.Documents.Any())
        {
            foreach (var autoCadDocument in this.Documents)
            {
                this.UnsubscribeToDocumentEvents(autoCadDocument);

                autoCadDocument.CloseDocument();
            }
        }
    }
}