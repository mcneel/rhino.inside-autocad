namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// The host <see cref="IAutocadDocument"/> <see cref="ISatelliteService"/>.
/// The application is attached to this object and persists for its lifetime.
/// </summary>
public interface IAutoCadInstance
{
    /// <summary>
    /// Event raised when the Autocad document changes, e.g. a new document is opened.
    /// </summary>
    event EventHandler? OnDocumentCreated;

    /// <summary>
    /// Event raised when the units of the Autocad document change.
    /// </summary>
    event EventHandler? OnUnitsChanged;

    /// <summary>
    /// Event raised when the <see cref="IAutocadDocument"/> begins closing , or when the
    /// user changes the active document. If either event occurs, the application will
    /// close.
    /// </summary>
    /// <remarks>
    /// The event triggers the AWI application to shutdown. This ensures that only one
    /// instance of the AWI application is running per AutoCAD session, thereby resolving
    /// any complications that can arise if multiple instances were allowed. For example,
    /// singleton access to the <see cref="RhinoGeometryConverter"/> is easier to manage
    /// and reason about when this policy is enforced, plus it prevents cross-cutting
    /// concerns from impacting negatively on the quality of the codebase. Additionally,
    /// automated actions such as disabling the AutoCAD ribbon menu while the AWI
    /// application is running can be bypassed (reactivated) by launching another
    /// instance of the AWI application via a different document. The single-instance
    /// approach therefore simplifies interaction between AutoCAD and the AWI application.
    /// </remarks>
    event EventHandler? DocumentClosingOrActivated;

    /// <summary>
    /// The <see cref="IValidationLogger"/> for this <see cref="ISatelliteService"/>.
    /// </summary>
    IValidationLogger ValidationLogger { get; }

    /// <summary>
    /// The <see cref="IAutoCadDocument"/> instance.
    /// </summary>
    /// <remarks>
    /// This document is not necessarily the active document in the AutoCAD
    /// application. For example, if the user switches to another document
    /// this property will still return the document that was active when
    /// this <see cref="IInteropService"/> was initialized. This
    /// ensures that the AWI application is always working with the same
    /// <see cref="IAutoCadDocument"/>.
    /// </remarks>
   // IAutoCadDocument? Document { get; }

    /// <summary>
    /// The <see cref="IObjectIdTagDatabaseManager"/>.
    /// </summary>
  //  IObjectIdTagDatabaseManager? TagDatabaseManager { get; }

    /// <summary>
    /// The <see cref="IDataTagDatabaseManager"/>.
    /// </summary>
  //  IDataTagDatabaseManager? DataTagDatabaseManager { get; }

    List<IAutocadDocument> Documents { get; }

    /// <summary>
    /// The current active <see cref="IAutocadDocument"/> in the AutoCAD application.
    /// </summary>
    IAutocadDocument? ActiveDocument { get; }
}