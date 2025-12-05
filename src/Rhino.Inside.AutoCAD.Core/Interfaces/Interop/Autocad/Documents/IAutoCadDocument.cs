namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// Represents a document object from AutoCAD.
/// </summary>
public interface IAutocadDocument
{
    /// <summary>
    /// The unique Id of this document. 
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Event handler triggered when a command within this document has successfully
    /// completed.
    /// </summary>
    /// <remarks>
    /// This event only triggers when a command concludes successfully. If a command
    /// is cancelled, this event will not fire. For example, when a user is creating
    /// a polyline by adding vertices, if they hit the "esc" key after the final vertex, 
    /// the command is cancelled, and this event won't be triggered. However, if they
    /// press "Enter", the command completes successfully and this event will be
    /// triggered.
    /// </remarks>
    event EventHandler<IAutocadDocumentChangeEventArgs>? DocumentChanged;

    /// <summary>
    /// Event raised when the units of the Autocad document change.
    /// </summary>
    event EventHandler? OnUnitsChanged;

    /// <summary>
    /// The <see cref="IDatabase"/> of the
    /// <see cref="IAutocadDocument"/>.
    /// </summary>
    IDatabase Database { get; }

    /// <summary>
    /// Provides file information about this <see cref="IAutocadDocument"/>.
    /// </summary>
    IAutocadDocumentFileInfo FileInfo { get; }

    /// <summary>
    /// The <see cref="ILayerRepository"/> of this <see cref="IAutocadDocument"/>.
    /// </summary>
    ILayerRepository LayerRepository { get; }

    /*/// <summary>
  /// The <see cref="ILinePatternCache"/> of this <see cref="IAutocadDocument"/>.
  /// </summary>
  ILinePatternCache LinePatternCache { get; }


  /// <summary>
  /// The <see cref="ILayoutRepository"/> of this <see cref="IAutocadDocument"/>.
  /// </summary>
  ILayoutRepository LayoutRepository { get; }

 /// <summary>
 /// The <see cref="IBlockTableRecordRepository"/> of this <see cref="IAutocadDocument"/>.
 /// </summary>
 IBlockTableRecordRepository BlockTableRecordRepository { get; }

 /// <summary>
 /// The <see cref="IPlotSettingsRepository"/> of this <see cref="IAutocadDocument"/>.
 /// </summary>
 IPlotSettingsRepository PlotSettingsRepository { get; }

 /// <summary>
 /// The <see cref="IDimensionStyleTableRecordRepository"/> of this <see
 /// cref="IAutocadDocument"/>.
 /// </summary>
 IDimensionStyleTableRecordRepository DimensionStyleTableRecordRepository { get; }

 /// <summary>
 /// The <see cref="ILeaderStyleObjectRepository"/> of this <see cref=
 /// "IAutocadDocument"/>.
 /// </summary>
 ILeaderStyleObjectRepository LeaderStyleObjectRepository { get; }

 /// <summary>
 /// The <see cref="ITextStyleTableRecordRepository"/> of this <see cref=
 /// "IDocument"/>.
 /// </summary>
 ITextStyleTableRecordRepository TextStyleTableRecordRepository { get; }*/

    /// <summary>
    /// The <see cref="UnitSystem"/> of this <see cref="IAutocadDocument"/>.
    /// </summary>
    UnitSystem UnitSystem { get; }

    /// <summary>
    /// Returns true if this <see cref="IAutocadDocument"/>s <see cref="CloseActionType"/>
    /// has been set to save, meaning the underlying AutoCAD document will be saved
    /// when <see cref="Close"/> is called.
    /// </summary>
    bool IsSaveOnClose { get; }

    /// <summary>
    /// Opens a transaction to read or modify this <see cref="IAutocadDocument"/>
    /// and returns the result <typeparamref name="T"/>. If <paramref name="abort"/>
    /// is set to true aborts the transaction to roll back any changes - this is
    /// useful when the transaction is being used to read data from the document
    /// that requires a change to obtain it. By default, all transactions are committed.
    /// Use the optional <paramref name="saveChanges"/> parameter to save any changes
    /// back to the underlying AutoCAD document.
    /// </summary>
    public T Transaction<T>(Func<ITransactionManager, T> function, bool saveChanges = false, bool abort = false);

    /// <summary>
    /// Refreshes the screen.
    /// </summary>
    void UpdateScreen();

    /// <summary>
    /// Creates a shallow clone of this <see cref="IAutocadDocument"/>. This clone has
    /// the same underlying autocad document but is inside a new wrapper.
    /// </summary>
    IAutocadDocument ShallowClone();

    /// <summary>
    /// Gets the <see cref="IDbObject"/> by its <see cref="IObjectId"/>.
    /// </summary>
    IDbObject GetObjectById(IObjectId objectId);

    /// <summary>
    /// Shuts down this <see cref="IAutocadDocument"/> instance. This method ensures that
    /// the instance is unhooked from all subscribed database and document events.
    /// </summary>
    void Close();

    /// <summary>
    /// Regenerates the document.
    /// </summary>
    void Regenerate();
}
