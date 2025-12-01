namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a type responsible for performing <see cref="CloseActionType"/>s
/// on the <see cref="IAutoCadDocument"/>.
/// </summary>
public interface IDocumentCloseAction
{
    /// <summary>
    /// The <see cref="CloseActionType"/> to apply to the <see cref="IAutoCadDocument"/>.
    /// </summary>
    CloseActionType CloseAction { get; }

    /// <summary>
    /// Invokes the <see cref="IAutoCadDocument"/> <see cref="CloseActionType"/>.
    /// </summary>
    /// <remarks>
    /// If the user has changed DWG files (which triggers document close) the
    /// <see cref="IAutoCadDocument"/> will not be the active document. If save or close is
    /// performed an exception is thrown. Therefore, <see cref="IAutoCadDocument"/> is set as
    /// the active document to perform the <see cref="CloseActionType"/>. Once completed
    /// the active document is set back to the document activated by the user.
    /// </remarks>
    void Invoke(IAutocadDocumentFileInfo fileInfo);

    /// <summary>
    /// Sets the <see cref="CloseActionType"/> to apply to the <see cref="IAutoCadDocument"/>,
    /// determining the action to take when <see cref="Invoke"/> is called.
    /// </summary>
    void SetCloseAction(CloseActionType closeActionType);
}