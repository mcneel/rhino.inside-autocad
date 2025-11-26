using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IDocumentCloseAction"/>
public class DocumentCloseAction : IDocumentCloseAction
{
    private readonly Document _document;
    private readonly DocumentCollection _documentManager;
    private readonly DwgVersion _dwgVersion;
    
    /// <inheritdoc/>
    public CloseActionType CloseAction { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="DocumentCloseAction"/>
    /// </summary>
    public DocumentCloseAction(Document document, DocumentCollection documentManager)
    {
        _document = document;

        _documentManager = documentManager;

        _dwgVersion = DwgVersion.Current;

        var defaultCloseActionType = CloseActionType.Unmodified;

        this.CloseAction = defaultCloseActionType;
    }

    /// <summary>
    /// The default close action. No action is performed on the
    /// <see cref="IDocument"/> or underlying AutoCAD document.
    /// </summary>
    private void NoAction(IDocumentFileInfo fileInfo) { }

    /// <summary>
    /// The action to save the <see cref="IDocument"/>. The underlying AutoCAD
    /// document remains open.
    /// </summary>
    private void SaveDocument(IDocumentFileInfo fileInfo)
    {
        var filePath = fileInfo.FilePath;

        var database = _document.Database;

        var securityParameters = database.SecurityParameters;

        database.SaveAs(filePath, true, _dwgVersion, securityParameters);
    }

    /// <summary>
    /// The action to save a close the <see cref="IDocument"/> and the underlying
    /// AutoCAD document.
    /// </summary>
    private void SaveAndCloseDocument(IDocumentFileInfo fileInfo)
    {
        var filePath = fileInfo.FilePath;

        _document.CloseAndSave(filePath);
    }

    /// <summary>
    /// Returns the close action based on the <see cref="CloseAction"/> type.
    /// </summary>
    private Action<IDocumentFileInfo> GetCloseAction()
    {
        var closeActionType = this.CloseAction;

        switch (closeActionType)
        {
            case CloseActionType.Save:
                return this.SaveDocument;

            case CloseActionType.SaveAndClose:
                return this.SaveAndCloseDocument;

            default:
                return this.NoAction;
        }
    }

    /// <inheritdoc/>
    public void Invoke(IDocumentFileInfo fileInfo)
    {
        var documentUnwrapped = _document;

        var activeDocument = documentUnwrapped;
        if (documentUnwrapped.IsActive == false)
        {
            activeDocument = _documentManager.CurrentDocument;

            _documentManager.CurrentDocument = documentUnwrapped;
        }

        var closeAction = this.GetCloseAction();

        closeAction.Invoke(fileInfo);

        // Set the activeDocument as the current document. This is needed only if the
        // user has changed documents, otherwise has no effect. 
        _documentManager.CurrentDocument = activeDocument;
    }

    /// <inheritdoc/>
    public void SetCloseAction(CloseActionType closeActionType)
    {
        this.CloseAction = closeActionType;
    }
}