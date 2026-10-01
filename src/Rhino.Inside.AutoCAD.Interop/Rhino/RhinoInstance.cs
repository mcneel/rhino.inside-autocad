using Rhino.ApplicationSettings;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.Inside.AutoCAD.Services;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoInstance"/>
public class RhinoInstance : IRhinoInstance
{
    private readonly IInstallationDirectories _installationDirectories;
    private const string _defaultTemplate = ApplicationConstants.DefaultTemplateFormat;
    private const string _failedToLoadRhinoDoc = ApplicationConstants.FailedToLoadRhinoDoc;
    private const string _failedToMatchAutoCadUnits = ApplicationConstants.FailedToMatchAutoCadUnits;

    /// <inheritdoc />
    public event EventHandler? DocumentCreated;

    /// <inheritdoc />
    public event EventHandler? UnitsChanged;

    /// <inheritdoc />
    public event EventHandler<IRhinoObjectModifiedEventArgs>? ObjectModifiedOrAppended;

    /// <inheritdoc />
    public event EventHandler<IRhinoObjectModifiedEventArgs>? ObjectRemoved;

    /// <inheritdoc />
    public event EventHandler? DeselectAll;

    /// <inheritdoc />
    public event EventHandler<IRhinoDocumentClosedEventArgs>? DocumentClosed;

    /// <inheritdoc />
    public event EventHandler? WindowDisplayStateChanged;

    /// <inheritdoc />
    public IRhinoCoreExtension RhinoCore { get; }

    /// <inheritdoc />
    public RhinoDoc? ActiveDoc { get; private set; }

    /// <inheritdoc />
    public Version ApplicationVersion { get; }

    /// <inheritdoc />
    public UnitSystem UnitSystem { get; private set; }

    /// <inheritdoc />
    public bool IsWindowMinimised => this.RhinoCore.WindowManager.IsMinimised;

    /// <inheritdoc />
    public bool IsWindowHidden => this.RhinoCore.WindowManager.IsHidden;

    /// <summary>
    /// Constructs a new <see cref="RhinoInstance"/> for managing the Rhino Inside lifecycle.
    /// </summary>
    /// <param name="installationDirectories">
    /// The installation directories containing paths to resources such as templates.
    /// </param>
    /// <remarks>
    /// This constructor only initializes the management instance; Rhino is not yet running.
    /// Use <see cref="IRhinoLauncher"/> to start a running Rhino instance, then call
    /// <see cref="ValidateRhinoDoc"/> to create or verify the active document.
    /// </remarks>
    /// <seealso cref="IRhinoLauncher"/>
    /// <seealso cref="ValidateRhinoDoc"/>
    public RhinoInstance(IInstallationDirectories installationDirectories)
    {
        _installationDirectories = installationDirectories;
        this.RhinoCore = RhinoCoreExtension.Instance;
        this.ApplicationVersion = Rhino.RhinoApp.Version;

        // The window manager watches the main window for being minimised or hidden through
        // the activation hook it installs when the core is created.
        this.RhinoCore.WindowManager.DisplayStateChanged += this.OnWindowDisplayStateChanged;
    }

    /// <summary>
    /// Creates and initializes a new <see cref="RhinoDoc"/> based on the specified mode.
    /// </summary>
    /// <param name="logger">
    /// The startup logger used to record errors if document creation fails.
    /// </param>
    /// <param name="mode">
    /// The mode determining whether to create a headless or interactive document.
    /// </param>
    /// <param name="autoCadUnitSystem">
    /// The unit system of the active AutoCAD document, applied to the new document.
    /// </param>
    /// <returns>
    /// The newly created <see cref="RhinoDoc"/> instance.
    /// </returns>
    /// <remarks>
    /// This method performs the following initialization steps:
    /// <list type="bullet">
    ///   <item>Creates a headless or interactive document based on <paramref name="mode"/></item>
    ///   <item>Disables auto-save to prevent unwanted file operations</item>
    ///   <item>Matches the document's units to <paramref name="autoCadUnitSystem"/></item>
    ///   <item>Raises the <see cref="DocumentCreated"/> event</item>
    ///   <item>Subscribes to Rhino document events for object tracking</item>
    /// </list>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when document creation fails. The error is logged before re-throwing.
    /// </exception>
    /// <seealso cref="ValidateRhinoDoc"/>
    private RhinoDoc CreateRhinoDoc(IStartUpLogger logger,
        RhinoInsideMode mode,
        UnitSystem autoCadUnitSystem)
    {
        var template = string.Format(_defaultTemplate, _installationDirectories.Resources);

        try
        {

            var rhinoDoc = mode == RhinoInsideMode.Headless
                ? RhinoDoc.CreateHeadless(template)
                : RhinoDoc.Create(template);

            FileSettings.AutoSaveEnabled = false;

            // Assigned here rather than by ValidateRhinoDoc: handlers of DocumentCreated read
            // ActiveDoc, and this method has not returned by the time the event is raised.
            this.ActiveDoc = rhinoDoc;

            this.MatchAutoCadUnits(rhinoDoc, autoCadUnitSystem);

            this.DocumentCreated?.Invoke(this, EventArgs.Empty);

            // Read after the match, so the cache holds the units the document ended up with and
            // OnDocumentPropertiesModified does not report the match itself as a user change.
            this.UnitSystem = rhinoDoc.ModelUnitSystem;

            RhinoDoc.DocumentPropertiesChanged += this.OnDocumentPropertiesModified;
            RhinoDoc.AddRhinoObject += this.OnAddRhinoObject;
            RhinoDoc.ModifyObjectAttributes += this.OnModifyRhinoObject;
            RhinoDoc.DeleteRhinoObject += this.OnRemoveRhinoObject;
            RhinoDoc.SelectObjects += this.OnSelectedObject;
            RhinoDoc.DeselectObjects += this.OnSelectedObject;
            RhinoDoc.DeselectAllObjects += this.OnDeselectObjects;
            RhinoDoc.CloseDocument += this.OnCloseDocument;
            RhinoDoc.BeginOpenDocument += this.OnBeginOpenDocument;
            RhinoDoc.ActiveDocumentChanged += this.OnActiveDocumentChanged;

            return rhinoDoc;
        }
        catch
        {
            // Cleared so the next launch retries creation, rather than adopting a document that
            // never reached its event subscriptions.
            this.ActiveDoc = null;

            logger.AddError(_failedToLoadRhinoDoc);

            throw;
        }
    }

    /// <summary>
    /// Sets the model unit system of a newly created document to the units of the AutoCAD
    /// document that launched it.
    /// </summary>
    /// <param name="rhinoDoc">The document just created from the template.</param>
    /// <param name="autoCadUnitSystem">The unit system of the active AutoCAD document.</param>
    /// <remarks>
    /// The template is fixed at millimeters, so without this a drawing in feet opens a Rhino
    /// document whose numbers bear no relation to the ones the user is working in. Run once,
    /// against a document that holds no geometry yet: nothing re-applies it, so the user is
    /// free to change the units afterwards and they stay changed.
    /// <para>
    /// <see cref="RhinoDoc.AdjustModelUnitSystem"/> is used in preference to the
    /// <see cref="RhinoDoc.ModelUnitSystem"/> setter because it states in its signature that no
    /// geometry is scaled, and it carries the template's tolerances over to the new unit system
    /// instead of leaving a millimeter tolerance behind as a foot one.
    /// </para>
    /// <para>
    /// <see cref="UnitSystem.Unset"/> is what an AutoCAD document reports when it is unitless,
    /// when its INSUNITS value has no Rhino equivalent, and when there is no active document at
    /// all; the template's units are kept in each case, and the start up logger already warns
    /// the user about the first two. A failure is contained rather than thrown: the document is
    /// usable in the template's units, and the launch must not be lost over its unit tag.
    /// </para>
    /// </remarks>
    private void MatchAutoCadUnits(RhinoDoc rhinoDoc, UnitSystem autoCadUnitSystem)
    {
        if (autoCadUnitSystem is UnitSystem.Unset or UnitSystem.None or UnitSystem.CustomUnits)
            return;

        try
        {
            rhinoDoc.AdjustModelUnitSystem(autoCadUnitSystem, scale: false);
        }
        catch (Exception exception)
        {
            // Tested rather than null-conditioned: the Instance getter itself throws before the
            // logger is initialised, and losing the launch is the one outcome to avoid here.
            if (LoggerService.IsInitialized)
            {
                LoggerService.Instance.LogError(exception,
                    string.Format(_failedToMatchAutoCadUnits, autoCadUnitSystem));
            }
        }
    }

    /// <summary>
    /// Returns true when <paramref name="document"/> is a headless document other than
    /// <see cref="ActiveDoc"/>, such as the one the Brep converter imports into and disposes.
    /// </summary>
    /// <remarks>
    /// Rhino raises its object events for every document, so without this the objects of a
    /// temporary document are previewed in AutoCAD and, as disposing the document removes
    /// nothing, never cleared. Only headless documents are excluded: the user's own
    /// documents are never headless, and a document Rhino did not report is let through, as
    /// it was before.
    /// </remarks>
    /// <param name="document">The document an event was raised for.</param>
    private bool IsTemporaryDocument(RhinoDoc? document)
    {
        if (document == null || document.IsHeadless == false) return false;

        return document.RuntimeSerialNumber != this.ActiveDoc?.RuntimeSerialNumber;
    }

    /// <summary>
    /// Raises <see cref="DocumentClosed"/> for the document with the given serial number.
    /// </summary>
    /// <param name="documentSerialNumber">The serial number of the document that went away.</param>
    private void RaiseDocumentClosed(uint documentSerialNumber)
    {
        var eventArgs = new RhinoDocumentClosedEventArgs(documentSerialNumber);

        this.DocumentClosed?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.CloseDocument"/> event by raising
    /// <see cref="DocumentClosed"/> for the closed document.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the closed document.</param>
    /// <remarks>
    /// Rhino raises no <see cref="RhinoDoc.DeleteRhinoObject"/> event for the objects a
    /// closed document held, so their previews would otherwise outlive the document. The
    /// event is raised whether or not the document is <see cref="ActiveDoc"/>: Rhino may
    /// already have made the next document active by the time the previous one closes.
    /// The temporary headless documents created and disposed by this plugin are ignored.
    /// </remarks>
    private void OnCloseDocument(object sender, DocumentEventArgs e)
    {
        var document = e.Document;

        if (this.IsTemporaryDocument(document)) return;

        this.RaiseDocumentClosed(e.DocumentSerialNumber);
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.BeginOpenDocument"/> event by raising
    /// <see cref="DocumentClosed"/> when the file replaces the document's contents.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the document being opened into.</param>
    /// <remarks>
    /// Rhino can open a file into the document it already has rather than closing it, in
    /// which case no <see cref="RhinoDoc.CloseDocument"/> event is raised for the objects it
    /// is about to discard. Imports and reference (worksession) opens add to the document
    /// instead of replacing it, so they are ignored, as are temporary headless documents.
    /// </remarks>
    private void OnBeginOpenDocument(object sender, DocumentOpenEventArgs e)
    {
        var document = e.Document;

        if (e.Merge || e.Reference || this.IsTemporaryDocument(document)) return;

        this.RaiseDocumentClosed(e.DocumentSerialNumber);
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.ActiveDocumentChanged"/> event by adopting the newly
    /// active document as <see cref="ActiveDoc"/>.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the newly active document.</param>
    /// <remarks>
    /// Opening or creating a document in Rhino replaces the one this instance created, and
    /// commands, scripts and units follow the document the user is working in. Previews do
    /// not depend on it: <see cref="DocumentClosed"/> names the document it is raised for.
    /// Headless documents never become the user's document, and a headless
    /// <see cref="ActiveDoc"/> belongs to a headless session which has no other document
    /// to follow. Raises <see cref="UnitsChanged"/> when the new document's units differ.
    /// </remarks>
    private void OnActiveDocumentChanged(object sender, DocumentEventArgs e)
    {
        var document = e.Document;

        if (document == null || document.IsHeadless) return;

        var activeDoc = this.ActiveDoc;

        if (activeDoc == null || activeDoc.IsHeadless ||
            activeDoc.RuntimeSerialNumber == document.RuntimeSerialNumber)
            return;

        this.ActiveDoc = document;

        var currentUnits = document.ModelUnitSystem;

        if (currentUnits == this.UnitSystem)
            return;

        this.UnitSystem = currentUnits;

        this.UnitsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the main window being minimised, restored, hidden or shown by raising
    /// <see cref="WindowDisplayStateChanged"/>.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments.</param>
    private void OnWindowDisplayStateChanged(object? sender, EventArgs e)
    {
        this.WindowDisplayStateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.DeselectAllObjects"/> event by raising <see cref="DeselectAll"/>.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing deselection details.</param>
    private void OnDeselectObjects(object? sender, RhinoDeselectAllObjectsEventArgs e)
    {
        if (this.IsTemporaryDocument(e.Document)) return;

        this.DeselectAll?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles <see cref="RhinoDoc.SelectObjects"/> and <see cref="RhinoDoc.DeselectObjects"/> events
    /// by raising <see cref="ObjectModifiedOrAppended"/> for each affected object.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the selected or deselected objects.</param>
    /// <remarks>
    /// Rhino does not raise a modify event when an object's selection state changes, so this method
    /// bridges that gap by treating selection changes as modifications. This ensures preview geometry
    /// is updated to reflect the current selection state.
    /// </remarks>
    private void OnSelectedObject(object? sender, RhinoObjectSelectionEventArgs e)
    {
        if (this.IsTemporaryDocument(e.Document)) return;

        for (var index = 0; index < e.RhinoObjects.Length; index++)
        {
            var rhinoObject = e.RhinoObjects[index];

            var eventArgs = new RhinoObjectModifiedEventArgs(rhinoObject, e.Document);

            this.ObjectModifiedOrAppended?.Invoke(this, eventArgs);
        }
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.DeleteRhinoObject"/> event by raising <see cref="ObjectRemoved"/>.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the removed object.</param>
    private void OnRemoveRhinoObject(object sender, RhinoObjectEventArgs e)
    {
        var document = e.TheObject.Document;

        if (this.IsTemporaryDocument(document)) return;

        var eventArgs = new RhinoObjectModifiedEventArgs(e.TheObject, document);

        this.ObjectRemoved?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.ModifyObjectAttributes"/> event by raising <see cref="ObjectModifiedOrAppended"/>.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the modified object.</param>
    private void OnModifyRhinoObject(object sender, RhinoModifyObjectAttributesEventArgs e)
    {
        if (this.IsTemporaryDocument(e.Document)) return;

        var eventArgs = new RhinoObjectModifiedEventArgs(e.RhinoObject, e.Document);

        this.ObjectModifiedOrAppended?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.AddRhinoObject"/> event by raising <see cref="ObjectModifiedOrAppended"/>.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the added object.</param>
    private void OnAddRhinoObject(object sender, RhinoObjectEventArgs e)
    {
        var document = e.TheObject.Document;

        if (this.IsTemporaryDocument(document)) return;

        var eventArgs = new RhinoObjectModifiedEventArgs(e.TheObject, document);

        this.ObjectModifiedOrAppended?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Handles the <see cref="RhinoDoc.DocumentPropertiesChanged"/> event and raises <see cref="UnitsChanged"/>
    /// when the model unit system has changed.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments containing the document.</param>
    /// <remarks>
    /// This method compares the current <see cref="UnitSystem"/> with the document's model unit system.
    /// If they differ, it raises the <see cref="UnitsChanged"/> event and updates the cached value.
    /// </remarks>
    private void OnDocumentPropertiesModified(object sender, DocumentEventArgs e)
    {
        var currentUnits = e.Document.ModelUnitSystem;

        if (currentUnits == this.UnitSystem)
            return;

        this.UnitsChanged?.Invoke(this, EventArgs.Empty);

        this.UnitSystem = currentUnits;
    }

    /// <inheritdoc />
    public void ValidateRhinoDoc(RhinoInsideMode mode, IStartUpLogger logger,
        UnitSystem autoCadUnitSystem)
    {
        if (this.ActiveDoc == null)
        {
            this.ActiveDoc = this.CreateRhinoDoc(logger, mode, autoCadUnitSystem);
        }
    }

    /// <inheritdoc />
    public Result RunRhinoCommand(string commandName)
    {
        return this.ActiveDoc == null
            ? Result.Failure
            : RhinoApp.ExecuteCommand(this.ActiveDoc, commandName);
    }

    /// <inheritdoc />
    public bool RunRhinoScript(string commandName)
    {
        return this.ActiveDoc != null
               && RhinoApp.RunScript(this.ActiveDoc.RuntimeSerialNumber, commandName, true);
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        RhinoDoc.DocumentPropertiesChanged -= this.OnDocumentPropertiesModified;

        RhinoDoc.AddRhinoObject -= this.OnAddRhinoObject;

        RhinoDoc.ModifyObjectAttributes -= this.OnModifyRhinoObject;

        RhinoDoc.DeleteRhinoObject -= this.OnRemoveRhinoObject;

        RhinoDoc.SelectObjects -= this.OnSelectedObject;

        RhinoDoc.DeselectObjects -= this.OnSelectedObject;

        RhinoDoc.DeselectAllObjects -= this.OnDeselectObjects;

        RhinoDoc.CloseDocument -= this.OnCloseDocument;

        RhinoDoc.BeginOpenDocument -= this.OnBeginOpenDocument;

        RhinoDoc.ActiveDocumentChanged -= this.OnActiveDocumentChanged;

        // Before the core shuts down, which restores the window and then destroys it.
        this.RhinoCore.WindowManager.DisplayStateChanged -= this.OnWindowDisplayStateChanged;

        this.RhinoCore.Shutdown();
    }
}