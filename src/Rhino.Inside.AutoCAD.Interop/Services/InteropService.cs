using Autodesk.AutoCAD.ApplicationServices;
using Bimorph.Core.Services.Core;
using Bimorph.Core.Services.Core.Interfaces;
using Bimorph.Core.Services.Services;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Core.Interfaces.Applications.Applications;
using System.Windows.Threading;

namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// The host <see cref="IDocument"/> <see cref="ISatelliteService"/>.
/// The application is attached to this object and persists for its lifetime.
/// </summary>
public class InteropService : SatelliteServiceBase, IInteropService
{
    private readonly Dispatcher _dispatcher;

    private DocumentCollection? _documentManager;
    private Document? _activeDocument;

    private readonly string _unsavedNotSupported = MessageConstants.UnsavedNotSupported;
    private readonly string _readOnlyNotSupported = MessageConstants.ReadOnlyNotSupported;
    private readonly string _fileUnitsNotSupported = MessageConstants.FileUnitsNotSupported;

    private bool _documentClosing;

    private readonly ButtonApplicationId _appId;

    /// <inheritdoc/>
    public event EventHandler? DocumentClosingOrActivated;

    /// <inheritdoc/>
    public IDocument? Document { get; private set; }

    /// <inheritdoc/>
    public IObjectIdTagDatabaseManager? TagDatabaseManager { get; private set; }

    /// <inheritdoc/>
    public IDataTagDatabaseManager? DataTagDatabaseManager { get; private set; }

    /// <inheritdoc/>
    public ITransientManager? TransientManager { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="InteropService"/>.
    /// </summary>
    public InteropService(IRhinoInsideAutoCadApplication application, ButtonApplicationId appId)
        : base(nameof(InteropService))
    {
        _dispatcher = application.Bootstrapper.Dispatcher;

        _appId = appId;

    }

    /// <inheritdoc/>
    public override RunResult StartUp()
    {
        _documentManager = Application.DocumentManager;

        _activeDocument = _documentManager.MdiActiveDocument;

        var documentCloseAction = new DocumentCloseAction(_activeDocument, _documentManager);

        var document = new DocumentFile(_activeDocument, documentCloseAction, _dispatcher, _appId);

        _activeDocument.BeginDocumentClose += this.OnDocumentClosing;
        _documentManager.DocumentActivated += this.OnDocumentActivated;

        this.Document = document;

        this.TagDatabaseManager = new ObjectIdTagDatabaseManager(document);

        this.DataTagDatabaseManager = new DataTagDatabaseManager(document);

        this.TransientManager = new TransientManagerWrapper(Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager);

        return this.Validate(document);
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
    private RunResult Validate(IDocument document)
    {
        var validationLogger = this.ValidationLogger;

        var cadDocument = document.Unwrap();

        // If the file is not saved, the document is not named.
        if (cadDocument.IsNamedDrawing == false)
        {
            validationLogger.AddMessage(_unsavedNotSupported);
        }

        if (cadDocument.IsReadOnly)
        {
            validationLogger.AddMessage(_readOnlyNotSupported);
        }

        var unitSystem = document.UnitSystem;
        if (unitSystem == UnitSystem.Unset)
        {
            validationLogger.AddMessage(string.Format(_fileUnitsNotSupported, unitSystem));

        }

        return validationLogger.HasValidationErrors ? RunResult.Invalid : RunResult.Success;
    }

    /// <summary>
    /// Event handler which raises the <see cref="DocumentClosingOrActivated"/> event.
    /// </summary>
    protected virtual void OnDocumentClosingOrChanged(EventArgs e)
    {
        //   DocumentClosingOrActivated?.Invoke(this, e);
    }

    /// <inheritdoc/>
    protected override void RestartTasks()
    {
        _activeDocument!.BeginDocumentClose -= this.OnDocumentClosing;
        _documentManager!.DocumentActivated -= this.OnDocumentActivated;

        this.TagDatabaseManager!.CommitAll();
        this.DataTagDatabaseManager!.CommitAll();
    }

    /// <inheritdoc/>
    public override void Shutdown()
    {
        var document = this.Document;

        if (document != null)
        {
            _activeDocument!.BeginDocumentClose -= this.OnDocumentClosing;
            _documentManager!.DocumentActivated -= this.OnDocumentActivated;

            this.TagDatabaseManager?.CommitAll();
            this.DataTagDatabaseManager?.CommitAll();

            document.Close();
        }
    }
}