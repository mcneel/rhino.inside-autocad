namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A Service that responds to AutoCAD document changes and updates Grasshopper documents accordingly.
/// </summary>
public interface IGrasshopperChangeResponder
{
    /// <summary>
    /// Updates the all the Grasshopper documents according to the specified AutoCAD document change.
    /// </summary>
    void Respond(IAutocadDocumentChange documentChange);
}