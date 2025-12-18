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
    /// Validates that the Rhino document is created and ready to use.
    /// </summary>
    void ValidateRhinoDoc(RhinoInsideMode mode, IValidationLogger validationLogger);

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