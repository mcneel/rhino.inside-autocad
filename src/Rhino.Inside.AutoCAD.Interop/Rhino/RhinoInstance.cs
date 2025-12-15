using Rhino.ApplicationSettings;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Inside.AutoCAD.Core;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Rhino.UI;

namespace Rhino.Inside.AutoCAD.Interop;

/// <inheritdoc cref="IRhinoInstance"/>
public class RhinoInstance : IRhinoInstance
{
    private readonly IApplicationDirectories _applicationDirectories;

    /// <inheritdoc />
    public event EventHandler? DocumentCreated;

    /// <inheritdoc />
    public event EventHandler? UnitsChanged;

    /// <inheritdoc />
    public event EventHandler<IRhinoObjectModifiedEventArgs>? ObjectModifiedOrAppended;

    /// <inheritdoc />
    public event EventHandler<IRhinoObjectModifiedEventArgs>? ObjectRemoved;

    /// <inheritdoc />
    public IRhinoCoreExtension RhinoCore { get; }

    /// <inheritdoc />
    public RhinoDoc? ActiveDoc { get; private set; }

    /// <inheritdoc />
    public Version ApplicationVersion { get; }

    public UnitSystem UnitSystem { get; private set; }

    /// <summary>
    /// Constructs a new <see cref="IRhinoInstance"/> instance. Note that this does
    /// not mean that Rhino is running yet. This only constructs the instance to manage
    /// the Rhino Inside lifecycle and document. Use <see cref="IRhinoLauncher"/> to
    /// create a running Rhino instance.
    /// </summary>
    public RhinoInstance(IApplicationDirectories applicationDirectories)
    {
        _applicationDirectories = applicationDirectories;
        this.RhinoCore = RhinoCoreExtension.Instance;
        this.ApplicationVersion = Rhino.RhinoApp.Version;

    }

    /// <summary>
    /// Initializes the Rhino Inside instance and returns true if it has successfully
    /// launched otherwise false indicating a failure.
    /// </summary>
    private RhinoDoc CreateRhinoDoc(IValidationLogger validationLogger,
        RhinoInsideMode mode)
    {
        var template =
            $"{_applicationDirectories.Resources}Large Objects - Millimeters.3dm";

        try
        {

            var rhinoDoc = mode == RhinoInsideMode.Headless
                ? RhinoDoc.CreateHeadless(template)
                : RhinoDoc.Create(template);

            FileSettings.AutoSaveEnabled = false;

            this.DocumentCreated?.Invoke(this, EventArgs.Empty);

            this.UnitSystem = rhinoDoc.ModelUnitSystem;

            RhinoDoc.DocumentPropertiesChanged += this.OnDocumentPropertiesModified;
            RhinoDoc.AddRhinoObject += this.OnAddRhinoObject;
            RhinoDoc.ModifyObjectAttributes += this.OnModifyRhinoObject;
            RhinoDoc.DeleteRhinoObject += this.OnRemoveRhinoObject;

            return rhinoDoc;
        }
        catch
        {
            validationLogger.AddMessage("Failed to initialize Rhino Doc.");

            throw;
        }
    }

    /// <summary>
    /// Event handler which fires when a Rhino object is removed.
    /// </summary>
    private void OnRemoveRhinoObject(object sender, RhinoObjectEventArgs e)
    {
        this.ObjectRemoved?.Invoke(this, new RhinoObjectModifiedEventArgs(e.TheObject));
    }

    /// <summary>
    /// Event handler which fires when a Rhino object is modified.
    /// </summary>
    private void OnModifyRhinoObject(object sender, RhinoModifyObjectAttributesEventArgs e)
    {
        this.ObjectModifiedOrAppended?.Invoke(this, new RhinoObjectModifiedEventArgs(e.RhinoObject));
    }

    /// <summary>
    /// Event handler which fires when a Rhino object is added.
    /// </summary>
    private void OnAddRhinoObject(object sender, RhinoObjectEventArgs e)
    {
        this.ObjectModifiedOrAppended?.Invoke(this, new RhinoObjectModifiedEventArgs(e.TheObject));
    }

    /// <summary>
    /// An event handler which fires when the Rhino document properties are modified.
    /// It checks to see if the unit system has changed and raises the <see cref="UnitsChanged"/>
    /// event if it has.
    /// </summary>
    private void OnDocumentPropertiesModified(object sender, DocumentEventArgs e)
    {
        var currentUnits = e.Document.ModelUnitSystem;

        if (currentUnits == this.UnitSystem)
            return;

        this.UnitsChanged?.Invoke(this, EventArgs.Empty);

        this.UnitSystem = currentUnits;
    }

    /// <inheritdoc />
    public void ValidateRhinoDoc(RhinoInsideMode mode, IValidationLogger validationLogger)
    {
        if (this.ActiveDoc == null)
        {
            this.ActiveDoc = this.CreateRhinoDoc(validationLogger, mode);
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
        if (this.ActiveDoc != null && this.ActiveDoc.Modified)
        {
            if (Rhino.UI.Dialogs.ShowMessage("Save changes to Rhino Document",
                    "Rhino Save Changes", ShowMessageButton.YesNo,
                    ShowMessageIcon.Warning) == ShowMessageResult.Yes)
            {
                this.ActiveDoc.Save();
            }
        }

        RhinoDoc.DocumentPropertiesChanged -= this.OnDocumentPropertiesModified;

        RhinoDoc.AddRhinoObject -= this.OnAddRhinoObject;

        RhinoDoc.ModifyObjectAttributes -= this.OnModifyRhinoObject;

        RhinoDoc.DeleteRhinoObject -= this.OnRemoveRhinoObject;

        this.ActiveDoc?.Dispose();
    }
}