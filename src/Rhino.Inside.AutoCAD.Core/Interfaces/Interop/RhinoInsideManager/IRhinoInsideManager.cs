namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents the manager responsible for coordinating the interaction between Rhino and AutoCAD.
/// </summary>
/// <remarks>
/// This interface provides access to the core instances of Rhino and AutoCAD, enabling seamless
/// integration between the two applications. It acts as the central point for managing the lifecycle
/// and interactions of these instances within the Rhino.Inside.AutoCAD environment.
/// </remarks>
public interface IRhinoInsideManager
{
    /// <summary>
    /// Gets the instance of Rhino that is being managed.
    /// </summary>
    /// <value>
    /// An <see cref="IRhinoInstance"/> representing the Rhino core extension and its associated
    /// functionality, such as the active document and command execution.
    /// </value>
    /// <remarks>
    /// This property provides access to the Rhino environment, including its core extension,
    /// active document, and methods for validating and interacting with Rhino commands.
    /// </remarks>
    IRhinoInstance RhinoInstance { get; }

    /// <summary>
    /// Gets the instance of AutoCAD that is being managed.
    /// </summary>
    /// <value>
    /// An <see cref="IAutoCadInstance"/> representing the AutoCAD application instance, including
    /// its document, validation logger, and various database managers.
    /// </value>
    /// <remarks>
    /// This property provides access to the AutoCAD environment, including its document lifecycle,
    /// event handling, and database management capabilities. It ensures that the AutoCAD instance
    /// is properly managed and synchronized with the Rhino instance.
    /// </remarks>
    IAutoCadInstance AutoCadInstance { get; }

    /// <summary>
    /// The <see cref="IUnitSystemManager"/> for managing unit systems between Rhino and AutoCAD.
    /// </summary>
    IUnitSystemManager UnitSystemManager { get; }

    /// <summary>
    /// The <see cref="IObjectRegister"/> for tracking objects between Rhino and AutoCAD. This
    /// is used to manage the transient objects previewed in the AutoCAD viewport.
    /// </summary>
    IObjectRegister ObjectRegister { get; }
}
