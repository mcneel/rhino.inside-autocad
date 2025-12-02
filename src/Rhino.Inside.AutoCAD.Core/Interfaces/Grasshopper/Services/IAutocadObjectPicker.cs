namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// A service to pick AutoCAD objects in the active document.
/// </summary>
public interface IAutocadObjectPicker
{
    /// <summary>
    /// Displays a prompt to pick a single object from the AutoCAD document. This method
    /// will force the autoCAD application to be focused and then display the prompt.
    /// This method returns null if no object was picked.
    /// </summary>
    IEntity? PickObject(ISelectionFilter filter, string message);

    /// <summary>
    /// Displays a prompt to pick a multiple objects from the AutoCAD document. This
    /// method will force the autoCAD application to be focused and then display the
    /// prompt. This method returns an empty list if no objects were picked.
    /// </summary>
    IList<IEntity> PickObjects(ISelectionFilter filter, string message);
}