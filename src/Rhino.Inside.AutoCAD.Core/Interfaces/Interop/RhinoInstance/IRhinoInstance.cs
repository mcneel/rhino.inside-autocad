using Rhino.Commands;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents an instance of the Rhino application within the Rhino.Inside.AutoCAD environment.
/// </summary>
/// <remarks>
/// This interface provides access to the core functionality of Rhino, including its core extension,
/// the active document, and methods for validating and interacting with Rhino commands.
/// It acts as the primary entry point for managing the Rhino instance.
/// </remarks>
public interface IRhinoInstance
{
    /// <summary>
    /// Event raised when a Rhino document changed, e.g. a new document is opened.
    /// </summary>
    event EventHandler? DocumentCreated;

    /// <summary>
    /// Event raised when the Rhino unit system is changed.
    /// </summary>
    event EventHandler? UnitsChanged;

    /// <summary>
    /// Event raised when a Rhino object is modified or appended.
    /// </summary>
    event EventHandler<IRhinoObjectModifiedEventArgs>? ObjectModifiedOrAppended;

    /// <summary>
    /// Event raised when a Rhino object is removed.
    /// </summary>
    event EventHandler<IRhinoObjectModifiedEventArgs>? ObjectRemoved;

    /// <summary>
    /// Event raised when all Rhino objects are deselected. This is used to clear the AutoCAD
    /// transient preview when all objects are deselected in Rhino.
    /// </summary>
    event EventHandler? DeselectAll;

    /// <summary>
    /// The instance of the Rhino core extension.
    /// </summary>
    IRhinoCoreExtension RhinoCore { get; }

    /// <summary>
    /// The current active Rhino document.
    /// </summary>
    RhinoDoc? ActiveDoc { get; }

    /// <summary>
    /// The version of the Rhino application.
    /// </summary>
    Version ApplicationVersion { get; }

    /// <summary>
    /// The unit system of the active Rhino document.
    /// </summary>
    UnitSystem UnitSystem { get; }

    /// <summary>
    /// Validates that the Rhino document is created and ready to use.
    /// </summary>
    /// <param name="mode">The mode the document is created for.</param>
    /// <param name="logger">The logger a creation failure is posted to.</param>
    /// <param name="autoCadUnitSystem">
    /// The unit system of the active AutoCAD document, applied to the Rhino document when it
    /// is first created. Pass <see cref="UnitSystem.Unset"/> when there is no active document
    /// or its units have no Rhino equivalent, and the template's units are kept.
    /// </param>
    /// <remarks>
    /// The units are applied on creation only. The document belongs to the user from that
    /// point on and is never pulled back to the AutoCAD units, however either side is changed
    /// later.
    /// </remarks>
    void ValidateRhinoDoc(RhinoInsideMode mode, IStartUpLogger logger, UnitSystem autoCadUnitSystem);

    /// <summary>
    /// Runs a Rhino command in the active Rhino document.
    /// </summary>
    Result RunRhinoCommand(string commandName);

    /// <summary>
    /// Runs a Rhino script command in the active Rhino document.
    /// </summary>
    bool RunRhinoScript(string commandName);

    /// <summary>
    /// The steps to taken to shutdown the rhino instance.
    /// </summary>
    void Shutdown();
}