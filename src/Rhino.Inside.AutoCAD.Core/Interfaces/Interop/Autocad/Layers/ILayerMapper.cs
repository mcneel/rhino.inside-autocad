namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A mapper interface which aims to handle AWI compliant <see cref="IAutocadLayer"/>s
/// with our internal host <see cref="IAutocadLayer"/>s.
/// </summary>
public interface ILayerMapper
{
    /// <summary>
    /// An event that gets raised when the <see cref="SelectedLayer"/>
    /// property is changed. Listeners can subscribe to this event to get
    /// notified whenever a layer selection change happens. This allows
    /// for responsive UI updates or other actions in response to user
    /// selections in the UI.
    /// </summary>
 //   public event EventHandler<ILayerSelectedEventArgs>? SelectedLayerChanged;

    /// <summary>
    /// An <see cref="IAutocadLayer"/> of the <see cref="IInteropService"/>'s
    /// <see cref="IAutocadDocument"/> that host <see cref="IEntity"/>s.
    /// </summary>
    /// <remarks>
    /// Typically this <see cref="IAutocadLayer"/> has derived from an external
    /// consultants file (e.g. an architect), and is not a standard AWI
    /// layer.
    /// </remarks>
    IAutocadLayer HostLayer { get; }

    /// <summary>
    /// Represents the <see cref="IAutocadLayer"/> that is currently selected
    /// from the <see cref="CompliantLayerSet"/>.
    /// </summary>
    IAutocadLayer? SelectedLayer { get; set; }

    /// <summary>
    /// The <see cref="ICompliantLayerSet"/>.
    /// </summary>
    ICompliantLayerSet CompliantLayerSet { get; }

    /// <summary>
    /// True if the <see cref="HostLayer"/> is part of the
    /// <see cref="CompliantLayerSet"/> collection, otherwise false.
    /// </summary>
    bool IsCompliant { get; }
}